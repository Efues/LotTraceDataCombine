using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text;

namespace LotTraceDataCombine
{
  public class EDULogRecord
  {
    public EDULogRecord() 
    {
      Kanban = String.Empty;
      Seihinban = String.Empty;
      Serial1 = String.Empty;
      GSSHaraidashiDT = String.Empty;
    }
    public string Kanban { get; set; } // EDUーかんばん管理Noで利用
    public string KanbanExport // EDUーかんばん管理No出力する文字列
    {
      get
      {
        if(Kanban != null && Kanban.Length > 97)
        {
          return Kanban.Substring(92, 4);
        }
        return String.Empty;
      }  
    } 
    public string Seihinban { get; set; } // EDUーEDU品番
//    public string ShuuyousuuRenban { get; set; }
//    public string Shuuyousuu { get; set; }
    public string Serial1 { get; set; } // EDUーEDU QR
//    public string Serial2  { get; set; }
//    public string TsuikaKirokuKoumoku { get; set; }
//    public string GSSTanmatsu { get; set; }
//    public string SagyoushaID { get; set; }
    public string GSSHaraidashiDT { get; set; } // EDUー流動日時

    public string JissouSerial
    {
      get
      {
        if(Serial1.Length < 23) return string.Empty;
        return Serial1.Substring(12, 11);
      }
    }
  }

  public class EDULog
  {
    // keyはEDU QR
    public Dictionary<string, EDULogRecord> Rec { get; } = new Dictionary<string, EDULogRecord>();
    public Dictionary<string, EDULogRecord> RecJissouKey { get; } = new Dictionary<string, EDULogRecord>();

    private Dictionary<string, int> DupCnt { get; } = new Dictionary<string, int>();

    internal static EDULog Load(string folder, string folderManual)
    {
      var rtn = new EDULog();

      if (Directory.Exists(folder))
      {
        foreach (var filePath in Directory.EnumerateFiles(folder))
        {
          using (IXLWorkbook workbook = new XLWorkbook(filePath))
          {
            IXLWorksheet sheet = workbook.Worksheet(1); // インデックスで取得
            var idx = 0;
            foreach (IXLRow row in sheet.Rows())
            {
              if (idx != 0)
              {
                var adding = new EDULogRecord()
                {
                  Kanban = row.Cell(1).Value.ToString(),
                  Seihinban = row.Cell(2).Value.ToString(),
                  //                ShuuyousuuRenban = row.Cell(3).Value.ToString(),
                  //                Shuuyousuu = row.Cell(4).Value.ToString(),
                  Serial1 = row.Cell(5).Value.ToString(),
                  //                Serial2 = row.Cell(6).Value.ToString(),
                  //                TsuikaKirokuKoumoku = row.Cell(7).Value.ToString(),
                  //                GSSTanmatsu = row.Cell(8).Value.ToString(),
                  //                SagyoushaID = row.Cell(9).Value.ToString(),
                  GSSHaraidashiDT = row.Cell(10).Value.ToString(),
                };
                rtn.Append(adding);
              }
              idx++;
            }
          }
        }
      }

      if (Directory.Exists(folderManual))
      {
        foreach (var filePath in Directory.EnumerateFiles(folderManual))
        {
          using (var rs = new StreamReader(filePath, Encoding.GetEncoding("shift-jis")))
          {
            var idx = 0;
            while (!rs.EndOfStream)
            {
              var line = rs.ReadLine();
              if (idx != 0)
              {
                if (line == null) continue;
                var items = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (items.Length >= 1)
                {
                  var adding = new EDULogRecord()
                  {
                    Serial1 = items[0],
                  };
                  rtn.Append(adding);
                }
              }
              idx++;
            }
          }
        }
      }
      return rtn;
    }
    public void Append(EDULogRecord adding)
    {
      var key = adding.Serial1;
      if (Rec.ContainsKey(key))
      {
        if (DupCnt.ContainsKey(key)) DupCnt[key] += 1;
        else DupCnt[key] = 1;
      }
      else
      {
        Rec[key] = adding;
        RecJissouKey[adding.JissouSerial] = adding;
      }
    }
  }
}
