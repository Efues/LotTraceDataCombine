using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace LotTraceDataCombine
{
  public class JissoLogRecord
  {
    [Index(0)]
    public string KibanID { get; set; } // 実装-実装QR
//    [Index(1)]
//    public string KibanDataMei { get; set; }
    [Index(2)]
    public string BuhinID { get; set; } // 部品-部品品番
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

    internal static JissoLog Load(string folder)
    {
      var rtn = new JissoLog();
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
      return rtn;
    }
    public void Append(JissoLogRecord adding)
    {
      var key = adding.KibanID + adding.Kohen;//;(adding.OffsetNo + 1);
      if (Rec.ContainsKey(key))
      {
        if (DupCnt.ContainsKey(key)) DupCnt[key] += 1;
        else DupCnt[key] = 1;
      }
      else Rec[key] = adding;
    }
  }
}
