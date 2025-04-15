
using System.ComponentModel;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LotTraceDataCombine
{
  public enum Result
  {
    Success,
    Failed
  }

  public class FileCombineMgr
  {
    public string ErrMsg { get; private set; } = string.Empty;
    public StringBuilder StatMsg { get; private set; } = new StringBuilder();

    private string EDUFolder { get; }
    private string EDUFolderManual { get; }
    private string MotorFolder { get; }
    private string JissouFolder { get; }
    private string JissouFolderManual { get; }
    private string OutputFile { get; }


    public FileCombineMgr(
      string eDUFolder,
      string eDUFolderManual,
      string motorFolder,
      string jissouFolder,
      string jissouFolderManual,
      string outputFile)
    {
      this.EDUFolder = eDUFolder;
      this.EDUFolderManual = eDUFolderManual;
      this.MotorFolder = motorFolder;
      this.JissouFolder = jissouFolder;
      this.JissouFolderManual = jissouFolderManual;
      this.OutputFile = outputFile;
    }

    internal Result Execute(BackgroundWorker bgWorker)
    {
      var cnter = 0;
      bgWorker.ReportProgress(++cnter, "処理開始....");

      var stt = System.Environment.TickCount;
      StatMsg.Clear();

      var result = CheckFolderExistance();
      if (result == Result.Failed) return Result.Failed;

      var outputFolder = Path.GetDirectoryName(OutputFile);
      if (!Directory.Exists(outputFolder) && outputFolder != null)
      {
        Directory.CreateDirectory(outputFolder);
      }

      // 実装ログをロード
      var stt1 = 0;

      stt1 = Environment.TickCount;

      bgWorker.ReportProgress(++cnter, "実装ログの読み込み....");
      var jissoLog = JissoLog.Load(this.JissouFolder, this.JissouFolderManual);
      bgWorker.ReportProgress(++cnter, $"....完了");
      bgWorker.ReportProgress(++cnter, $"({(Environment.TickCount - stt1) / 1000}秒)");
      bgWorker.ReportProgress(++cnter, $"実装ログ:{jissoLog.Rec.Count()}件");

      // テスト的に手動実装ログを出力する
//      TestExportManualJissouLog(jissoLog);

      // EDUログをロード
      stt1 = Environment.TickCount;
      bgWorker.ReportProgress(++cnter, "EDUログの読み込み....");
      var eduLog = EDULog.Load(this.EDUFolder, this.EDUFolderManual);
      bgWorker.ReportProgress(++cnter, "....完了");
      bgWorker.ReportProgress(++cnter, $"({(Environment.TickCount - stt1) / 1000}秒)");
      bgWorker.ReportProgress(++cnter, $"EDUログ:{eduLog.Rec.Count()}件");

      // テスト的に手動EDUログを出力する
//      TestExportManualEDULog(eduLog);


      // モーターログをロード
      stt1 = Environment.TickCount;
      bgWorker.ReportProgress(++cnter, "Motorログの読み込み....");
      var motorLog = MotorLog.Load(this.MotorFolder, bgWorker);
      bgWorker.ReportProgress(++cnter, "....完了");
      bgWorker.ReportProgress(++cnter, $"({(Environment.TickCount - stt1)/1000}秒)");
      bgWorker.ReportProgress(++cnter, $"MotorEDUSheet:{motorLog.EDURec.Count()}件");
      bgWorker.ReportProgress(++cnter, $"Motor検査Sheet:{motorLog.InspRec.Count()}件");
      bgWorker.ReportProgress(++cnter, $"Motor結合ログ:{motorLog.Rec.Count()}件");

      // データを連結
      var eduMotorRec = new Dictionary<string, (EDULogRecord, MotorEDULogRecord, MotorInspectLogRecord)>();
      {
        stt1 = Environment.TickCount;
        bgWorker.ReportProgress(++cnter, "実装ログとEDUログの紐づけ....");
        foreach (var eduRec in eduLog.Rec)
        {
          var eduQRkey = eduRec.Key;
          if (motorLog.Rec.ContainsKey(eduQRkey))
          {
            var motorLogAppend = motorLog.Rec[eduQRkey];
            eduMotorRec.Add(eduQRkey, (eduRec.Value, motorLogAppend.Item1, motorLogAppend.Item2));
          }
        }
        bgWorker.ReportProgress(++cnter, $"....完了");
        bgWorker.ReportProgress(++cnter, $"({(Environment.TickCount - stt1) / 1000}秒)");
        bgWorker.ReportProgress(++cnter, $"EDUMotor結合ログ:{eduMotorRec.Count()}件");
      }

      var combinedRec = new List<(JissoLogRecord, EDULogRecord, MotorEDULogRecord, MotorInspectLogRecord)>();
      {
        stt1 = Environment.TickCount;
        foreach (var eduRec in eduMotorRec)
        {
          var jissouSerial = eduRec.Value.Item1.JissouSerial;
          if (jissoLog.Rec.ContainsKey(jissouSerial))
          {
            var appendingJissuRec = jissoLog.Rec[jissouSerial];
            combinedRec.Add((appendingJissuRec, eduRec.Value.Item1, eduRec.Value.Item2, eduRec.Value.Item3));
          }
        }
        bgWorker.ReportProgress(++cnter, $"....完了");
        bgWorker.ReportProgress(++cnter, $"({(Environment.TickCount - stt1)/1000}秒)");
        bgWorker.ReportProgress(++cnter, $"実装EDUMotor結合ログ:{combinedRec.Count()}件");
      }

      // 結果出力
      bgWorker.ReportProgress(++cnter, $"結果出力開始");

      var fileName = $"{DateTime.Now.ToString("yyMMddHHmmss")}.csv";
      var filePath = Path.Combine(this.OutputFile, fileName);
      using (var ws = new StreamWriter(filePath, false, Encoding.GetEncoding("shift-jis")))
      {
        ws.WriteLine(
          "部品品番,部品リールロット,実装QR,個片,実装品番,実装流動日時," +
          "EDU_QR,EDU品番,EDU流動日時,EDUかんばんNo," +
          "モータライン内シリアル,モータラインNo,モータ製品シリアル," +
          "モータ品番,モータQR,モータ流動日時"
          );
        foreach (var rec in combinedRec)
        {
          var jissouLog = rec.Item1;
          var EDULog = rec.Item2;
          var MotorEDULog = rec.Item3;
          var MotorInspectionLog = rec.Item4;
          ws.Write(jissouLog.BuhinID + ",");
          ws.Write(jissouLog.BuhinLotID + ",");
          ws.Write(jissouLog.KibanID + ",");
          ws.Write(jissouLog.Kohen + ",");
          ws.Write(jissouLog.KibanID.Substring(0,4) + "-" + jissouLog.KibanID.Substring(4, 6) +",");
          ws.Write(jissouLog.SagyouNichijiFormatted.ToString() + ",");
          ws.Write(EDULog.Serial1 + ",");
          ws.Write(EDULog.Seihinban + ",");
          ws.Write(EDULog.GSSHaraidashiDT + ",");
          ws.Write(EDULog.KanbanExport + ",");
          ws.Write(MotorEDULog.LineSerial + ",");
          ws.Write(MotorEDULog.LineNo + ",");
          ws.Write(MotorInspectionLog.SerialStr + ",");
          ws.Write(MotorInspectionLog.SeihinHinban + ",");
          ws.Write(MotorInspectionLog.MotorQR + ",");
          ws.Write(MotorInspectionLog.KensaKanryouNichiji);
          ws.WriteLine();
        }
      }
      bgWorker.ReportProgress(++cnter, $"....完了");
      return Result.Success;
    }

    private void TestExportManualEDULog(EDULog eduLog)
    {
      var fileName = $"{DateTime.Now.ToString("yyMMddHHmmss")}.csv";
      var filePath = Path.Combine(this.EDUFolderManual, fileName);
      using (var ws = new StreamWriter(filePath, false, Encoding.GetEncoding("shift-jis")))
      {
        ws.WriteLine("EDUQR");
        foreach (var rec in eduLog.Rec)
        {
          var EDU_QR = rec.Key;
          ws.Write(EDU_QR);
          ws.WriteLine();
        }
      }
    }

    private void TestExportManualJissouLog(JissoLog jissoLog)
    {
      var fileName = $"{DateTime.Now.ToString("yyMMddHHmmss")}.csv";
      var filePath = Path.Combine(this.JissouFolderManual, fileName);
      using (var ws = new StreamWriter(filePath, false, Encoding.GetEncoding("shift-jis")))
      {
        ws.WriteLine("実装QR,個片");
        foreach (var rec in jissoLog.Rec.GroupBy(item => item.Value.KibanID).OrderBy(item => item.Key))
        {
          var jissouQR = rec.Key;
          var kohenList = rec.Select(item => item.Value.Kohen).Distinct().ToList();
          foreach (var kohen in kohenList.OrderBy(x => x))
          {
            ws.Write(jissouQR + ",");
            ws.Write(kohen);
            ws.WriteLine();
          }
        }
      }
    }

    private Result CheckFolderExistance()
    {
      if (!Directory.Exists(EDUFolder) && !Directory.Exists(EDUFolderManual))
      {
        ErrMsg = $"EDUログフォルダが見つかりません。";
        return Result.Failed;
      }
      if (!Directory.Exists(MotorFolder))
      {
        ErrMsg = $"モーターログフォルダが見つかりません。";
        return Result.Failed;
      }
      if (!Directory.Exists(JissouFolder) && !Directory.Exists(JissouFolderManual))
      {
        ErrMsg = $"実装ログフォルダが見つかりません。";
        return Result.Failed;
      }
      return Result.Success;
    }
  }
}