
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