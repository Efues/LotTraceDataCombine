using System.ComponentModel;
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
      textBoxEDULogFolderManual.Text = Properties.Settings.Default.EDUFolderManual;
      textBoxMotorLogFolder.Text = Properties.Settings.Default.MotorFolder;
      textBoxJissouLogFolder.Text = Properties.Settings.Default.JissouFolder;
      textBoxJissouLogFolderManual.Text = Properties.Settings.Default.JissouFolderManual;
      textBoxOutputFile.Text = Properties.Settings.Default.OutputFile;
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      Properties.Settings.Default.MotorFolderXls = textBoxMotorLogFolder_xls.Text;
      Properties.Settings.Default.EDUFolder = textBoxEDULogFolder.Text;
      Properties.Settings.Default.EDUFolderManual = textBoxEDULogFolderManual.Text;
      Properties.Settings.Default.MotorFolder = textBoxMotorLogFolder.Text;
      Properties.Settings.Default.JissouFolder = textBoxJissouLogFolder.Text;
      Properties.Settings.Default.JissouFolderManual = textBoxJissouLogFolderManual.Text;
      Properties.Settings.Default.OutputFile = textBoxOutputFile.Text;
      Properties.Settings.Default.Save();
    }

    private void buttonExecute_Click(object sender, EventArgs e)
    {
      try
      {
        //処理が行われているときは、何もしない
        if (backgroundWorkerMain.IsBusy)
          return;

        this.textBoxStatus.Clear();
        //Buttonを無効にする
        groupBox1.Enabled = false;
        backgroundWorkerMain.WorkerReportsProgress = true;
        backgroundWorkerMain.RunWorkerAsync();
      }
      catch (Exception exp)
      {
        textBoxStatus.Text += "内部エラー";
        textBoxStatus.Text += exp.Message;
      }
    }

    private void buttonXlsConvert_Click(object sender, EventArgs e)
    {
      try
      {
        //処理が行われているときは、何もしない
        if (backgroundWorkerMotorConv.IsBusy)
          return;

        //Buttonを無効にする
        this.textBoxStatus.Clear();
        groupBox1.Enabled = false;
        backgroundWorkerMotorConv.WorkerReportsProgress = true;
        backgroundWorkerMotorConv.RunWorkerAsync();
      }
      catch (Exception exp)
      {
        textBoxStatus.Text += "内部エラー";
        textBoxStatus.Text += exp.Message;
      }
    }
    private void buttonSelectFolderJIssouLogCSVManual_Click(object sender, EventArgs e)
    {
      try
      {
        SelectFolder(textBoxJissouLogFolderManual);
      }
      catch (Exception exp)
      {
        textBoxStatus.Text += "内部エラー";
        textBoxStatus.Text += exp.Message;
      }

    }

    private void buttonSelectEDULogManual_Click(object sender, EventArgs e)
    {
      try
      {
        SelectFolder(textBoxEDULogFolderManual);
      }
      catch (Exception exp)
      {
        textBoxStatus.Text += "内部エラー";
        textBoxStatus.Text += exp.Message;
      }
    }

    private void buttonSelectEDULog_Click(object sender, EventArgs e)
    {
      try
      {
        SelectFolder(textBoxEDULogFolder);
      }
      catch (Exception exp)
      {
        textBoxStatus.Text += "内部エラー";
        textBoxStatus.Text += exp.Message;
      }
    }

    private void buttonSelectFolderMotorXls_Click(object sender, EventArgs e)
    {
      try
      {
        SelectFolder(textBoxEDULogFolder);
      }
      catch (Exception exp)
      {
        textBoxStatus.Text += "内部エラー";
        textBoxStatus.Text += exp.Message;
      }
    }

    private void buttonSelectFolderJIssouLogCSV_Click(object sender, EventArgs e)
    {
      try
      {
        SelectFolder(textBoxEDULogFolder);
      }
      catch (Exception exp)
      {
        textBoxStatus.Text += "内部エラー";
        textBoxStatus.Text += exp.Message;
      }
    }

    private void buttonSelectFolderMotorLogXlsx_Click(object sender, EventArgs e)
    {
      try
      {
        SelectFolder(textBoxEDULogFolder);
      }
      catch (Exception exp)
      {
        textBoxStatus.Text += "内部エラー";
        textBoxStatus.Text += exp.Message;
      }
    }

    private void buttonSelectFolderOutput_Click(object sender, EventArgs e)
    {
      try
      {
        SelectFolder(textBoxEDULogFolder);
      }
      catch (Exception exp)
      {
        textBoxStatus.Text += "内部エラー";
        textBoxStatus.Text += exp.Message;
      }
    }

    private static void SelectFolder(TextBox targetTextBox)
    {
      var folderBrowserDialog = new FolderBrowserDialog();
      if (Directory.Exists(targetTextBox.Text))
      {
        folderBrowserDialog.SelectedPath = targetTextBox.Text;
      }
      if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
      {
        targetTextBox.Text = folderBrowserDialog.SelectedPath;
      }
    }

    private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      if (e.UserState != null)
      {
        textBoxStatus.Text += e.UserState.ToString();
        textBoxStatus.Text += System.Environment.NewLine;
      }
    }

    private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      //Buttonを有効化
      groupBox1.Enabled = true;
    }

    private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
    {
      var bgWorker = (BackgroundWorker)sender;

      var eDUFolder = textBoxEDULogFolder.Text;
      var eDUFolderManual = textBoxEDULogFolderManual.Text;
      var motorFolder = textBoxMotorLogFolder.Text;
      var jissouFolder = textBoxJissouLogFolder.Text;
      var jissouFolderManual = textBoxJissouLogFolderManual.Text;
      var outputFile = textBoxOutputFile.Text;

      var mgr = new FileCombineMgr(eDUFolder, eDUFolderManual, motorFolder, jissouFolder, jissouFolderManual, outputFile);
      var result = mgr.Execute(bgWorker);
      if (result == Result.Failed)
      {
        MessageBox.Show(mgr.ErrMsg, Properties.Resources.Error);
        return;
      }
      else
      {
        textBoxStatus.Text += mgr.StatMsg.ToString();
      }

    }

    private void backgroundWorkerMotorConv_DoWork(object sender, DoWorkEventArgs e)
    {
      var bgWorker = (BackgroundWorker)sender;

      var inFolder = textBoxMotorLogFolder_xls.Text;
      var outFolder = textBoxMotorLogFolder.Text;

      var mgr = new XlsCombineMgr(inFolder, outFolder);
      var result = mgr.Execute(bgWorker);
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

    private void backgroundWorkerMotorConv_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {

    }

    private void backgroundWorkerMotorConv_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      //Buttonを有効化
      groupBox1.Enabled = true;
    }
  }
}
