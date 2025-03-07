
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
      StatMsg.Clear();

      var result = CheckFolderExistance();
      if (result == Result.Failed) return Result.Failed;

      var outputFolder = Path.GetDirectoryName(OutputFile);
      if (!Directory.Exists(outputFolder) && outputFolder != null)
      {
        Directory.CreateDirectory(outputFolder);
      }

      // 実装ログをロード
      var jissoLog = JissoLog.Load(this.JissouFolder);

      // EDUログをロード
      var eduLog = EDULog.Load(this.EDUFolder);

      // モーターログをロード
      var motorLog = MotorLog.Load(this.MotorFolder);

      // データを連結
      var eduMotorRec = new Dictionary<string, (EDULogRecord, MotorEDULogRecord, MotorInspectLogRecord)>();
      foreach (var rec in eduLog.Rec)
      {
        var key = rec.Key;
        if (motorLog.Rec.ContainsKey(key))
        {
          var motorLogAppend = motorLog.Rec[key];
          eduMotorRec.Add(key, (rec.Value, motorLogAppend.Item1, motorLogAppend.Item2));
        }
      }

      var combinedRec = new List<(JissoLogRecord, EDULogRecord, MotorEDULogRecord, MotorInspectLogRecord)>();
      foreach (var rec in eduMotorRec)
      {
        //        var key = rec.Key;
        var jissouSerial = rec.Value.Item1.JissouSerial;
        if (jissoLog.Rec.ContainsKey(jissouSerial))
        {
          var appendingJissuRec = jissoLog.Rec[jissouSerial];
          combinedRec.Add((appendingJissuRec, rec.Value.Item1, rec.Value.Item2, rec.Value.Item3));
        }
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