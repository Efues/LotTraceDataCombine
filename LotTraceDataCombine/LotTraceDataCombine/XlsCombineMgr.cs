
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace LotTraceDataCombine
{
  public class XlsCombineMgr
  {
    public string ErrMsg { get; private set; } = string.Empty;

    private string InFolder { get; }
    private string OutFolder { get; }


    public XlsCombineMgr(string inFolder, string outFolder)
    {
      this.InFolder = inFolder;
      this.OutFolder = outFolder;
    }

    internal Result Execute(System.ComponentModel.BackgroundWorker bgWorker)
    {
      if (!Directory.Exists(OutFolder) && OutFolder != null)
      {
        Directory.CreateDirectory(OutFolder);
      }

      var result = CheckFolderExistance();
      if (result == Result.Failed) return Result.Failed;

      var excelApp = new Excel.Application();
      Excel.Workbook workbook = null;

      try
      {
        var cnter = 0;
        bgWorker.ReportProgress(++cnter, "処理開始....");
        var stt = Environment.TickCount;
        foreach (var subFolder in Directory.EnumerateDirectories(InFolder))
        {
          var subFolderName = Path.GetFileName(subFolder);
          bgWorker.ReportProgress(++cnter, subFolderName);

          var fileList = Directory.EnumerateFiles(subFolder, "*.xls").ToList();
          var fileCnt = fileList.Count();
          var fileIdx = 0;
          foreach (var filePath in fileList)
          {
            fileIdx++;
            bgWorker.ReportProgress(++cnter, $"{fileIdx}/{fileCnt}");

            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var newName = subFolderName + "_" + fileName + ".xlsx";
            var outFilePath = Path.Combine(OutFolder, newName);

            if (File.Exists(outFilePath)) File.Delete(outFilePath);

            // .xlsファイルを開く
            workbook = excelApp.Workbooks.Open(filePath);

            // .xlsx形式で保存
            workbook.SaveAs(outFilePath, Excel.XlFileFormat.xlOpenXMLWorkbook);
          }
        }
        bgWorker.ReportProgress(++cnter, $"....完了({(Environment.TickCount - stt) / 1000}秒)");
      }
      catch (Exception ex)
      {
        bgWorker.ReportProgress(0, "内部エラー");
        bgWorker.ReportProgress(0, ex.Message);
        Console.WriteLine(ex.Message);
      }
      finally
      {
        if (workbook != null)
        {
          workbook.Close(false);
          excelApp.Quit();
        }

        // COMオブジェクトの解放
        if (workbook != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
        if (excelApp != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
      }
      return Result.Success;
    }

    private Result CheckFolderExistance()
    {
      if (!Directory.Exists(InFolder))
      {
        ErrMsg = $"Motorログフォルダ{InFolder}が見つかりません。";
        return Result.Failed;
      }
      if (OutFolder == null || !Directory.Exists(OutFolder))
      {
        ErrMsg = $"Motorログフォルダ{OutFolder}が見つかりません。";
        return Result.Failed;
      }
      return Result.Success;
    }
  }
}