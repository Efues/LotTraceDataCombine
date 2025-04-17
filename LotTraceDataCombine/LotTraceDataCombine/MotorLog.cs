using ClosedXML.Excel;
using System.ComponentModel;
using System.Runtime.Intrinsics.X86;

namespace LotTraceDataCombine
{
  public class MotorEDULogRecord
  {
    public string LineSerial { get; set; }
    public string LineNo { get; set; }
    public string EDUSerial { get; set; }

    public string JissouSerial
    {
      get
      {
        if (EDUSerial.Length < 23) return string.Empty;
        return EDUSerial.Substring(12, 11);
      }
    }
    public string EDUHinban
    {
      get
      {
        if (EDUSerial.Length < 10) return string.Empty;
        return EDUSerial.Substring(0, 6) + "-" + EDUSerial.Substring(6, 4);
      }
    }
  }

  public class MotorInspectLogRecord
  {
    public string LineSerial { get; set; }
    public string LineNo { get; set; }
    public string SerialStr { get; set; }// モーター製品シリアル
    internal int? SeihinHinban1 { private get; set; } // モーターモータ品番
    internal int? SeihinHinban2 { private get; set; } // モーターモータ品番
    public string SeihinHinban 
    {
      get
      {
        var rtn = String.Empty;
        if (SeihinHinban1 != null && SeihinHinban2 != null)
        {
          rtn += 
            SeihinHinban1.Value.ToString("D6") + "-" + 
            SeihinHinban2.Value.ToString("D4");
        }
        return rtn;
      }
    }

    public string KensaKanryouNichiji { get; set; } // モーター流動日時

    public string GetMotorQR(Dictionary<string, string> hinbanMappingTable)
    { 
      var rtn = String.Empty;
      var hit = hinbanMappingTable.Where(item => SeihinHinban.StartsWith(item.Key));
      if (hit.Any()&& SerialStr != null && SerialStr.Length >= 11) 
      {
        rtn = hit.First().Value;
        rtn += KensaKanryouNichiji.Substring(2, 2);
        rtn += SerialStr.Substring(0, 2);
        rtn += SerialStr.Substring(3, 2);
        rtn += "      "; // 空白6
        rtn += SerialStr.Substring(5, 6); // 空白6
        rtn += "       "; // 空白7
      }
      return rtn;
    } 
  }

  public class MotorLog
  {
    public Dictionary<string, MotorEDULogRecord> EDURec { get; } = new Dictionary<string, MotorEDULogRecord>();

    private Dictionary<string, int> EDUDupCnt { get; } = new Dictionary<string, int>();

    public Dictionary<string, MotorInspectLogRecord> InspRec { get; } = new Dictionary<string, MotorInspectLogRecord>();

    private Dictionary<string, int> InspDupCnt { get; } = new Dictionary<string, int>();


    // KeyはEDUSerial
    public Dictionary<string, (MotorEDULogRecord, MotorInspectLogRecord)> Rec { get; } 
      = new Dictionary<string, (MotorEDULogRecord, MotorInspectLogRecord)>();

    // 実装シリアルがキーの場合
    public Dictionary<string, (MotorEDULogRecord, MotorInspectLogRecord)> RecJissouKey { get; } = new Dictionary<string, (MotorEDULogRecord, MotorInspectLogRecord)>();

    internal static MotorLog Load(string folder, BackgroundWorker bgWorker)
    {
      var rtn = new MotorLog();
      if (!Directory.Exists(folder))return rtn;
      var fileList = Directory.EnumerateFiles(folder, "*.xlsx").ToList();
      var fileCnt = fileList.Count;
      var fileIdx = 0;
      foreach (var filePath in fileList)
      {
        fileIdx++;
        bgWorker.ReportProgress(fileIdx, ($"{fileIdx}/{fileCnt}"));

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
                int? seihinHinban1 = null;
                int? seihinHinban2 = null;

                if (int.TryParse(row.Cell("AJ").Value.ToString(), out var rslt1))
                {
                  seihinHinban1 = rslt1;
                }
                if (int.TryParse(row.Cell("AK").Value.ToString(), out var rslt2))
                {
                  seihinHinban2 = rslt2;
                }

                var adding = new MotorInspectLogRecord()
                {
                  LineSerial = row.Cell("Q").Value.ToString(),
                  LineNo = row.Cell("V").Value.ToString(),
                  SerialStr = row.Cell("X").Value.ToString(),
                  SeihinHinban1 = seihinHinban1,
                  SeihinHinban2 = seihinHinban2,
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
          var addingInstRec = rtn.InspRec[key];
          if (!rtn.Rec.ContainsKey(newKey)) {
            rtn.Rec.Add(newKey, (eduRec.Value, addingInstRec));
          }

          var jissouKey = eduRec.Value.JissouSerial;
          if (!rtn.RecJissouKey.ContainsKey(jissouKey)) {
            rtn.RecJissouKey.Add(jissouKey, (eduRec.Value, addingInstRec));
          }
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
