using ClosedXML.Excel;

namespace LotTraceDataCombine
{
  public class EDULogRecord
  {
    public string Kanban { get; set; } // EDUーかんばん管理Noで利用
    public string Seihinban { get; set; } // EDUーEDU品番
//    public string ShuuyousuuRenban { get; set; }
//    public string Shuuyousuu { get; set; }
    public string Serial1 { get; set; } // EDUーEDU QR
//    public string Serial2  { get; set; }
//    public string TsuikaKirokuKoumoku { get; set; }
//    public string GSSTanmatsu { get; set; }
//    public string SagyoushaID { get; set; }
    public string GSSHaraidashiDT { get; set; } // EDUー流動日時

    public string KanbanKanriNo // EDUーかんばん管理No
    {
      get
      {
        if (Kanban.Length <= 97) return string.Empty;
        return Kanban.Substring(93, 4);
      }
    }

    public string JissouSerial
    {
      get
      {
        if(Serial1.Length < 23) return string.Empty;
        return Serial1.Substring(13, 11);
      }
    }
  }

  public class EDULog
  {
    public Dictionary<string, EDULogRecord> Rec { get; } = new Dictionary<string, EDULogRecord>();

    private Dictionary<string, int> DupCnt { get; } = new Dictionary<string, int>();

    internal static EDULog Load(string folder)
    {
      var rtn = new EDULog();
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
      else Rec[key] = adding;
    }
  }
}
