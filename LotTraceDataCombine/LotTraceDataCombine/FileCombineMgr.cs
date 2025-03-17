
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

    private string EDUFolder {  get; }
    private string MotorFolder { get; }
    private string JissouFolder { get; }
    private string OutputFile { get; }


    public FileCombineMgr(string eDUFolder, string motorFolder, string jissouFolder, string outputFile)
    {
      this.EDUFolder = eDUFolder;
      this.MotorFolder = motorFolder;
      this.JissouFolder = jissouFolder;
      this.OutputFile = outputFile;
    }

    internal Result Execute()
    {
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
      StatMsg.AppendLine("実装ログの読み込み....");
      var jissoLog = JissoLog.Load(this.JissouFolder);
      StatMsg.AppendLine($"....完了({(Environment.TickCount - stt1)/1000}秒)");
      StatMsg.AppendLine($"実装ログ:{jissoLog.Rec.Count()}件");

      // EDUログをロード
      stt1 = Environment.TickCount;
      StatMsg.AppendLine("EDUログの読み込み....");
      var eduLog = EDULog.Load(this.EDUFolder);
      StatMsg.AppendLine($"....完了({(Environment.TickCount - stt1)/1000}秒)");
      StatMsg.AppendLine($"EDUログ:{eduLog.Rec.Count()}件");

      // モーターログをロード
      stt1 = Environment.TickCount;
      StatMsg.AppendLine("Motorログの読み込み....");
      var motorLog = MotorLog.Load(this.MotorFolder);
      StatMsg.AppendLine($"....完了({(Environment.TickCount - stt1)/1000}秒)");
      StatMsg.AppendLine($"MotorEDUSheet:{motorLog.EDURec.Count()}件");
      StatMsg.AppendLine($"Motor検査Sheet:{motorLog.InspRec.Count()}件");
      StatMsg.AppendLine($"Motor結合ログ:{motorLog.Rec.Count()}件");

      // データを連結
      var eduMotorRec = new Dictionary<string, (EDULogRecord, MotorEDULogRecord, MotorInspectLogRecord)>();
      {
        stt1 = Environment.TickCount;
        StatMsg.AppendLine("実装ログとEDUログの紐づけ....");
        foreach (var eduRec in eduLog.Rec)
        {
          var eduQRkey = eduRec.Key;
          if (motorLog.Rec.ContainsKey(eduQRkey))
          {
            var motorLogAppend = motorLog.Rec[eduQRkey];
            eduMotorRec.Add(eduQRkey, (eduRec.Value, motorLogAppend.Item1, motorLogAppend.Item2));
          }
        }
        StatMsg.AppendLine($"....完了({(Environment.TickCount - stt1)/1000}秒)");
        StatMsg.AppendLine($"EDUMotor結合ログ:{eduMotorRec.Count()}件");
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
        StatMsg.AppendLine($"....完了({(Environment.TickCount - stt1)/1000}秒)");
        StatMsg.AppendLine($"実装EDUMotor結合ログ:{combinedRec.Count()}件");
      }

      return Result.Success;
    }

    private Result CheckFolderExistance()
    {
      if (!Directory.Exists(EDUFolder))
      {
        ErrMsg = $"EDUログフォルダ{EDUFolder}が見つかりません。";
        return Result.Failed;
      }
      if (!Directory.Exists(MotorFolder))
      {
        ErrMsg = $"モーターログフォルダ{MotorFolder}が見つかりません。";
        return Result.Failed;
      }
      if (!Directory.Exists(JissouFolder))
      {
        ErrMsg = $"実装ログフォルダ{JissouFolder}が見つかりません。";
        return Result.Failed;
      }
      return Result.Success;
    }
  }
}