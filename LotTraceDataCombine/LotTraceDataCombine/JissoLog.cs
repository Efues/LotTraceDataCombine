using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using DocumentFormat.OpenXml.Drawing;
using System.Globalization;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace LotTraceDataCombine
{
  public class JissoLogRecord
  {
    public JissoLogRecord() 
    {
      KibanID = String.Empty;
      BuhinID = String.Empty;
      BuhinLotID = String.Empty;
      SagyouNichiji = String.Empty;
      OffsetNo = 0;
    }

    [Index(0)]
    public string KibanID { get; set; } // 実装-実装QR
//    [Index(1)]
//    public string KibanDataMei { get; set; }
    [Index(2)]
    public string BuhinID { get; set; }// 部品-部品品番
    [Index(3)]
    public string BuhinLotID { get; set; } // 部品-部品リールロット
//    [Index(4)]
//    public string ReelID { get; set; }
//    [Index(5)]
//    public string PatternMei { get; set; }
//    [Index(6)]
//    public string YBan { get; set; }
//    [Index(7)]
//    public string MachineMei { get; set; }
    [Index(8)]
    public string SagyouNichiji { get; set; } // 実装ー流動日時
    public string SagyouNichijiFormatted 
    {
      get
      {
        if(DateTime.TryParse(SagyouNichiji,out var rslt))
        {
          return rslt.ToString("yyyy/MM/dd HH:mm:ss");
        }
        return SagyouNichiji;
      }
    } 
                                              
    //    [Index(9)]
    //    public string Spare { get; set; }
    [Index(10)]
    public int OffsetNo { get; set; }
    public string Kohen 
    {
      get
      {
        return (OffsetNo + 1).ToString("X");
      }
    } // 実装-個片

    public string JissouSerial
    {
      get
      {
        if (KibanID.Length < 22) return string.Empty;
        return KibanID.Substring(12, 10) + Kohen;
      }
    }

    public string JissouHinban // 実装ー実装品番
    {
      get
      {
        if (KibanID.Length < 10) return string.Empty;
        return KibanID.Substring(0, 10);
      }
    }
  }

  public class JissoLog
  {
    // keyは実装シリアル
    public Dictionary<string, JissoLogRecord> Rec { get; } = new Dictionary<string, JissoLogRecord>();

    private Dictionary<string, int> DupCnt { get; } = new Dictionary<string, int>();

    internal static JissoLog Load(string folder, string folderManual)
    {
      var rtn = new JissoLog();

      if (Directory.Exists(folder))
      {
        Load(folder, rtn);
      }

      if (Directory.Exists(folderManual))
      {
        LoadManual(folderManual,rtn);
      }
      return rtn;
    }

    private static void LoadManual(string folderManual, JissoLog rtn)
    {
      // 手動実装ログをロード
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

              if (items.Length == 2)
              {
                var offset = Convert.ToInt32(items[1], 16) - 1;
                var adding = new JissoLogRecord()
                {
                  KibanID = items[0],
                  OffsetNo = offset
                };
                rtn.Append(adding);
              }
              if (items.Length == 1)
              {
                for (var i = 0; i <=11; i++)
                {
                  var adding = new JissoLogRecord()
                  {
                    KibanID = items[0],
                    OffsetNo = i
                  };
                  rtn.Append(adding);
                }
              }
            }
            idx++;
          }
        }
      }
    }

    private static void Load(string folder, JissoLog rtn)
    {
      // 通常の実装ログをロード
      var config = new CsvConfiguration(CultureInfo.InvariantCulture)
      {
        ShouldSkipRecord = args => args.Row.Parser.RawRow < 7
      };
      foreach (var filePath in Directory.EnumerateFiles(folder))
      {
        using (var rs = new StreamReader(filePath, Encoding.GetEncoding("shift-jis")))
        using (var csv = new CsvReader(rs, config))
        {
          foreach (var rec in csv.GetRecords<JissoLogRecord>())
          {
            rtn.Append(rec);
          }
        }
      }
    }

    public void Append(JissoLogRecord adding)
    {
      var key = adding.JissouSerial;
      if (Rec.ContainsKey(key))
      {
        if (DupCnt.ContainsKey(key)) DupCnt[key] += 1;
        else DupCnt[key] = 1;
      }
      else Rec[key] = adding;
    }
  }
}
