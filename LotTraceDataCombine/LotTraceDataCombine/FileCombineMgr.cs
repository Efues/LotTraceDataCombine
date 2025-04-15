
using DocumentFormat.OpenXml.Drawing.Diagrams;
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

    private string HinbanMappingFilePath { get; }
    private string EDUFolder { get; }
    private string EDUFolderManual { get; }
    private string MotorFolder { get; }
    private string JissouFolder { get; }
    private string JissouFolderManual { get; }
    private string OutputFile { get; }


    public FileCombineMgr(
      string hinbanMappingFilePath,
      string eDUFolder,
      string eDUFolderManual,
      string motorFolder,
      string jissouFolder,
      string jissouFolderManual,
      string outputFile)
    {
      this.HinbanMappingFilePath = hinbanMappingFilePath;
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
      try
      {
        bgWorker.ReportProgress(++cnter, "処理開始....");

        var stt = Environment.TickCount;

        // 品番マッピングファイルの確認
        var hinbanMappingTable = new Dictionary<string, string>();
        {
          bgWorker.ReportProgress(++cnter, "品番マッピングファイル読み込み");
          var result = LoadHinbanMapping(bgWorker, ref cnter, hinbanMappingTable);
          if (result == Result.Failed) return Result.Failed;
        }
        bgWorker.ReportProgress(++cnter, $"....完了");

        /*
        {
          var result = CheckFolderExistance();
          if (result == Result.Failed) return Result.Failed;
        }
        */

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
        bgWorker.ReportProgress(++cnter, $"({(Environment.TickCount - stt1) / 1000}秒)");
//        bgWorker.ReportProgress(++cnter, $"MotorEDUSheet:{motorLog.EDURec.Count()}件");
//        bgWorker.ReportProgress(++cnter, $"Motor検査Sheet:{motorLog.InspRec.Count()}件");
//        bgWorker.ReportProgress(++cnter, $"Motor結合ログ:{motorLog.Rec.Count()}件");

        // データを連結
        bgWorker.ReportProgress(++cnter, "データ紐づけ....");
        stt1 = Environment.TickCount;
        var combinedRec = ConbineRows(jissoLog, eduLog, motorLog);
        bgWorker.ReportProgress(++cnter, $"....完了");
        bgWorker.ReportProgress(++cnter, $"({(Environment.TickCount - stt1) / 1000}秒)");

        // 結果出力
        bgWorker.ReportProgress(++cnter, $"結果出力開始");
        ExportFile(combinedRec, hinbanMappingTable);
        bgWorker.ReportProgress(++cnter, $"....完了");
        return Result.Success;
      }
      catch (Exception ex)
      {
        bgWorker.ReportProgress(0, "内部エラー");
        bgWorker.ReportProgress(0, ex.Message);
        return Result.Failed;
      }
    }

    private static List<(JissoLogRecord, EDULogRecord?, MotorEDULogRecord?, MotorInspectLogRecord?)>
      ConbineRows(JissoLog jissoLog, EDULog eduLog, MotorLog motorLog)
    {
      var combinedRec = new List<(JissoLogRecord, EDULogRecord?, MotorEDULogRecord?, MotorInspectLogRecord?)>();
      // 実装ログデータがある場合
      if (jissoLog.Rec.Count > 0)
      {
        var idx = 0;
        foreach (var jissouRec in jissoLog.Rec)
        {
          idx++;
          JissoLogRecord addingJissouRec = jissouRec.Value;
          EDULogRecord addingEduLogRec = null;
          MotorEDULogRecord addingMotorEduLogRec = null;
          MotorInspectLogRecord addingMotorInspectLogRec = null;

          var jissouSerialKey = jissouRec.Key;

          // eduを検索
          var hit = eduLog.RecJissouKey.FirstOrDefault(item => item.Key == jissouSerialKey);
          if (hit.Key != null)
          {
            addingEduLogRec = hit.Value;

            // モータログを検索
            var eduQRkey = hit.Value.Serial1;
            if (motorLog.Rec.ContainsKey(eduQRkey))
            {
              var motorLogAppend = motorLog.Rec[eduQRkey];
              addingMotorEduLogRec = motorLogAppend.Item1;
              addingMotorInspectLogRec = motorLogAppend.Item2;
            }
          }
          else
          {
            // モーターログを検索
            if (motorLog.RecJissouKey.ContainsKey(jissouSerialKey))
            {
              var motorLogAppend = motorLog.RecJissouKey[jissouSerialKey];
              addingMotorEduLogRec = motorLogAppend.Item1;
              addingMotorInspectLogRec = motorLogAppend.Item2;
            }
          }
          combinedRec.Add((addingJissouRec, addingEduLogRec, addingMotorEduLogRec, addingMotorInspectLogRec));
        }
      }
      // 実装ログデータがない場合
      else
      {
        foreach (var eduRec in eduLog.Rec)
        {
          JissoLogRecord addingJissouRec = null;
          EDULogRecord addingEduLogRec = eduRec.Value;
          MotorEDULogRecord addingMotorEduLogRec = null;
          MotorInspectLogRecord addingMotorInspectLogRec = null;

          var eduQRkey = eduRec.Key;
          if (motorLog.Rec.ContainsKey(eduQRkey))
          {
            var motorLogAppend = motorLog.Rec[eduQRkey];
            addingMotorEduLogRec = motorLogAppend.Item1;
            addingMotorInspectLogRec = motorLogAppend.Item2;
          }
          combinedRec.Add((addingJissouRec, addingEduLogRec, addingMotorEduLogRec, addingMotorInspectLogRec));
        }
      }
      return combinedRec;
    }

    private void ExportFile(
      List<(JissoLogRecord, EDULogRecord?, MotorEDULogRecord?, MotorInspectLogRecord?)> combinedRec,
      Dictionary<string, string> hinbanMappingTable
      )
    {
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
          if (jissouLog != null)
          {
            ws.Write(jissouLog.BuhinID + ",");
            ws.Write(jissouLog.BuhinLotID + ",");
            ws.Write(jissouLog.KibanID + ",");
            ws.Write(jissouLog.Kohen + ",");
            ws.Write(jissouLog.KibanID.Substring(0, 4) + "-" + jissouLog.KibanID.Substring(4, 6) + ",");
            ws.Write(jissouLog.SagyouNichijiFormatted.ToString() + ",");
          }
          else
          {
            ws.Write(",,,,,,");
          }
          if (EDULog != null)
          {
            ws.Write(EDULog.Serial1 + ",");
            ws.Write(EDULog.Seihinban + ",");
            ws.Write(EDULog.GSSHaraidashiDT + ",");
            ws.Write(EDULog.KanbanExport + ",");
          }
          else
          {
            // モーターログからEDU情報を取得できる場合
            if (MotorEDULog != null)
            {
              ws.Write(MotorEDULog.EDUSerial + ",");
              ws.Write(MotorEDULog.EDUHinban + ",");
              ws.Write(",,");
            }
            else
            {
              ws.Write(",,,,");
            }
          }
          if (MotorEDULog != null)
          {
            ws.Write(MotorEDULog.LineSerial + ",");
            ws.Write(MotorEDULog.LineNo + ",");
          }
          else
          {
            ws.Write(",,");
          }
          if (MotorInspectionLog != null)
          {
            ws.Write(MotorInspectionLog.SerialStr + ",");
            ws.Write(MotorInspectionLog.SeihinHinban + ",");
            ws.Write(MotorInspectionLog.GetMotorQR(hinbanMappingTable) + ",");
            ws.Write(MotorInspectionLog.KensaKanryouNichiji);
          }
          else
          {
            ws.Write(",,,,");
          }
          ws.WriteLine();
        }
      }
    }

    private Result LoadHinbanMapping(BackgroundWorker bgWorker, ref int cnter, Dictionary<string, string> hinbanMappingTable)
    {
      if (!File.Exists(HinbanMappingFilePath))
      {
        ErrMsg = $"品番マッピングファイル{HinbanMappingFilePath}が見つかりません。";
        return Result.Failed;
      }

      using (var rs = new StreamReader(HinbanMappingFilePath, Encoding.GetEncoding("shift-jis")))
      {
        while (!rs.EndOfStream)
        {
          var item = rs.ReadLine().Split(',');
          if (item.Length > 1)
          {
            var key = item[0].Trim('"');
            var value = item[1].Trim('"');
            if (!hinbanMappingTable.ContainsKey(key))
            {
              hinbanMappingTable[key] = value;
              bgWorker.ReportProgress(++cnter, $"{key}/{value}");
            }
          }
        }
      }
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

    /*
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
    */
  }
}