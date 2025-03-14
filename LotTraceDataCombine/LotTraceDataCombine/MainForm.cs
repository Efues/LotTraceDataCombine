using System.Text;

namespace LotTraceDataCombine
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      textBoxMotorLogFolder_xls.Text = Properties.Settings.Default.MotorFolderXls;
      textBoxEDULogFolder.Text = Properties.Settings.Default.EDUFolder;
      textBoxMotorLogFolder.Text = Properties.Settings.Default.MotorFolder;
      textBoxJissouLogFolder.Text = Properties.Settings.Default.JissouFolder;
      textBoxOutputFile.Text = Properties.Settings.Default.OutputFile;
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      Properties.Settings.Default.MotorFolderXls = textBoxMotorLogFolder_xls.Text;
      Properties.Settings.Default.EDUFolder = textBoxEDULogFolder.Text;
      Properties.Settings.Default.MotorFolder = textBoxMotorLogFolder.Text;
      Properties.Settings.Default.JissouFolder = textBoxJissouLogFolder.Text;
      Properties.Settings.Default.OutputFile = textBoxOutputFile.Text;
      Properties.Settings.Default.Save();

    }

    private void buttonExecute_Click(object sender, EventArgs e)
    {
      try
      {
        var eDUFolder = textBoxEDULogFolder.Text;
        var motorFolder = textBoxMotorLogFolder.Text;
        var jissouFolder = textBoxJissouLogFolder.Text;
        var outputFile = textBoxOutputFile.Text;

        var mgr = new FileCombineMgr(eDUFolder, motorFolder, jissouFolder, outputFile);
        var result = mgr.Execute();
        if (result == Result.Failed)
        {
          MessageBox.Show(mgr.ErrMsg, Properties.Resources.Error);
          return;
        }
        else
        {
          textBoxStatus.Text = mgr.StatMsg.ToString();
        }
      }
      catch (Exception exp)
      {
      }
    }

    private void buttonXlsConvert_Click(object sender, EventArgs e)
    {
      try
      {
        var inFolder = textBoxMotorLogFolder_xls.Text;
        var outFolder = textBoxMotorLogFolder.Text;

        var mgr = new XlsCombineMgr(inFolder, outFolder);
        var result = mgr.Execute();
        if (result == Result.Failed)
        {
          MessageBox.Show(mgr.ErrMsg, Properties.Resources.Error);
          return;
        }
        else
        {
          // ê¨å˜ÇµÇΩèÍçáÅAÅA
        }
      }
      catch (Exception exp)
      {
      }
    }
  }
}
