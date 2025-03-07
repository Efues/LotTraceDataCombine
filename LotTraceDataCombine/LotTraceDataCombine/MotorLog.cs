using ClosedXML.Excel;
using System.Runtime.Intrinsics.X86;

namespace LotTraceDataCombine
{
  public class MotorEDULogRecord
  {
    public string LineSerial { get; set; }
    public string LineNo { get; set; }
    public string EDUSerial { get; set; }
  }

  public class MotorInspectLogRecord
  {
    public string LineSerial { get; set; }
    public string LineNo { get; set; }
    public string SerialStr { get; set; }// モーター製品シリアル
    public string SeihinHinban { get; set; } // モーターモータ品番
    public string KensaKanryouNichiji { get; set; } // モーター流動日時

    public string MotorQR 
    { 
      get
      {
        return string.Empty;
      }
    } // モーターモータQR
  }

  public class MotorLog
  {
    private Dictionary<string, MotorEDULogRecord> EDURec { get; } = new Dictionary<string, MotorEDULogRecord>();

    private Dictionary<string, int> EDUDupCnt { get; } = new Dictionary<string, int>();

    private Dictionary<string, MotorInspectLogRecord> InspRec { get; } = new Dictionary<string, MotorInspectLogRecord>();

    private Dictionary<string, int> InspDupCnt { get; } = new Dictionary<string, int>();


    // KeyはEDUSerial
    public Dictionary<string, (MotorEDULogRecord, MotorInspectLogRecord)> Rec { get; } = new Dictionary<string, (MotorEDULogRecord, MotorInspectLogRecord)>();

    internal static MotorLog Load(string folder)
    {
      var rtn = new MotorLog();
      foreach(var subFolder in Directory.EnumerateDirectories(folder))
      foreach (var filePath in Directory.EnumerateFiles(subFolder, "*.xlsx"))
      {
        using (IXLWorkbook workbook = new XLWorkbook(filePath))
        {
          {
            IXLWorksheet eduSerialSheet = workbook.Worksheet("EDUｼﾘｱﾙ");
            var idx = 0;
            foreach (IXLRow row in eduSerialSheet.Rows())
            {
              if (idx > 99)
              {
                var adding = new MotorEDULogRecord()
                {
                  LineSerial = row.Cell("Q").Value.ToString(),
                  LineNo = row.Cell("V").Value.ToString(),
                  EDUSerial = row.Cell("W").Value.ToString(),
                };
                rtn.Append(adding);
              }
              idx++;
            }
          }
          {
            IXLWorksheet inspSerialSheet = workbook.Worksheet("検査データ");
            var idx = 0;
            foreach (IXLRow row in inspSerialSheet.Rows())
            {
              if (idx > 99)
              {
                var adding = new MotorInspectLogRecord()
                {
                  LineSerial = row.Cell("Q").Value.ToString(),
                  LineNo = row.Cell("V").Value.ToString(),
                  SerialStr = row.Cell("X").Value.ToString(),
                  SeihinHinban = row.Cell("AJ").Value.ToString() + "-" + row.Cell("AK").Value.ToString(),
                  KensaKanryouNichiji = row.Cell("S").Value.ToString()
                };
                rtn.Append(adding);
              }
              idx++;
            }
          }
        }
      }

      // EDUとInspectionを合成
      foreach (var eduRec in rtn.EDURec)
      {
        var key = eduRec.Key;
        if (rtn.InspRec.ContainsKey(key))
        {
          var newKey = eduRec.Value.EDUSerial;
          rtn.Rec.Add(newKey, (eduRec.Value, rtn.InspRec[key]));
        }
      }

      return rtn;
    }

    public void Append(MotorEDULogRecord adding)
    {
      var key = adding.LineSerial+ "_" + adding.LineNo;
      if (EDURec.ContainsKey(key))
      {
        if (EDUDupCnt.ContainsKey(key)) EDUDupCnt[key] += 1;
        else EDUDupCnt[key] = 1;
      }
      else EDURec[key] = adding;
    }

    public void Append(MotorInspectLogRecord adding)
    {
      var key = adding.LineSerial + "_" + adding.LineNo;
      if (InspRec.ContainsKey(key))
      {
        if (InspDupCnt.ContainsKey(key)) InspDupCnt[key] += 1;
        else InspDupCnt[key] = 1;
      }
      else InspRec[key] = adding;
    }

  }
}
