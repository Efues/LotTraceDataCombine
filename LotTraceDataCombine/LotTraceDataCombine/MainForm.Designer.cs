namespace LotTraceDataCombine
{
  partial class Form1
  {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      splitContainer1 = new SplitContainer();
      dataGridView = new DataGridView();
      numericUpDownRowCnt = new NumericUpDown();
      buttonOpenFolder = new Button();
      textBoxFileResult = new TextBox();
      label9 = new Label();
      label8 = new Label();
      textBoxStatus = new TextBox();
      groupBox1 = new GroupBox();
      buttonSelectEDULogManual = new Button();
      textBoxEDULogFolderManual = new TextBox();
      label7 = new Label();
      buttonSelectFolderJIssouLogCSVManual = new Button();
      textBoxJissouLogFolderManual = new TextBox();
      label6 = new Label();
      buttonSelectFolderMotorXls = new Button();
      textBoxMotorLogFolder_xls = new TextBox();
      label5 = new Label();
      buttonXlsConvert = new Button();
      buttonExecute = new Button();
      buttonSelectFolderOutput = new Button();
      textBoxOutputFile = new TextBox();
      label4 = new Label();
      buttonSelectFolderJIssouLogCSV = new Button();
      textBoxJissouLogFolder = new TextBox();
      label3 = new Label();
      buttonSelectFolderMotorLogXlsx = new Button();
      textBoxMotorLogFolder = new TextBox();
      label2 = new Label();
      buttonSelectEDULog = new Button();
      textBoxEDULogFolder = new TextBox();
      label1 = new Label();
      groupBox2 = new GroupBox();
      backgroundWorkerMain = new System.ComponentModel.BackgroundWorker();
      backgroundWorkerMotorConv = new System.ComponentModel.BackgroundWorker();
      ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
      splitContainer1.Panel1.SuspendLayout();
      splitContainer1.Panel2.SuspendLayout();
      splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
      ((System.ComponentModel.ISupportInitialize)numericUpDownRowCnt).BeginInit();
      groupBox1.SuspendLayout();
      groupBox2.SuspendLayout();
      SuspendLayout();
      // 
      // splitContainer1
      // 
      resources.ApplyResources(splitContainer1, "splitContainer1");
      splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      splitContainer1.Panel1.Controls.Add(dataGridView);
      splitContainer1.Panel1.Controls.Add(numericUpDownRowCnt);
      splitContainer1.Panel1.Controls.Add(buttonOpenFolder);
      splitContainer1.Panel1.Controls.Add(textBoxFileResult);
      splitContainer1.Panel1.Controls.Add(label9);
      splitContainer1.Panel1.Controls.Add(label8);
      // 
      // splitContainer1.Panel2
      // 
      splitContainer1.Panel2.Controls.Add(textBoxStatus);
      // 
      // dataGridView
      // 
      resources.ApplyResources(dataGridView, "dataGridView");
      dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      dataGridView.Name = "dataGridView";
      // 
      // numericUpDownRowCnt
      // 
      resources.ApplyResources(numericUpDownRowCnt, "numericUpDownRowCnt");
      numericUpDownRowCnt.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
      numericUpDownRowCnt.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
      numericUpDownRowCnt.Name = "numericUpDownRowCnt";
      numericUpDownRowCnt.Value = new decimal(new int[] { 100, 0, 0, 0 });
      // 
      // buttonOpenFolder
      // 
      resources.ApplyResources(buttonOpenFolder, "buttonOpenFolder");
      buttonOpenFolder.Name = "buttonOpenFolder";
      buttonOpenFolder.UseVisualStyleBackColor = true;
      buttonOpenFolder.Click += buttonOpenFolder_Click;
      // 
      // textBoxFileResult
      // 
      resources.ApplyResources(textBoxFileResult, "textBoxFileResult");
      textBoxFileResult.Name = "textBoxFileResult";
      // 
      // label9
      // 
      resources.ApplyResources(label9, "label9");
      label9.Name = "label9";
      // 
      // label8
      // 
      resources.ApplyResources(label8, "label8");
      label8.Name = "label8";
      // 
      // textBoxStatus
      // 
      resources.ApplyResources(textBoxStatus, "textBoxStatus");
      textBoxStatus.Name = "textBoxStatus";
      // 
      // groupBox1
      // 
      resources.ApplyResources(groupBox1, "groupBox1");
      groupBox1.Controls.Add(buttonSelectEDULogManual);
      groupBox1.Controls.Add(textBoxEDULogFolderManual);
      groupBox1.Controls.Add(label7);
      groupBox1.Controls.Add(buttonSelectFolderJIssouLogCSVManual);
      groupBox1.Controls.Add(textBoxJissouLogFolderManual);
      groupBox1.Controls.Add(label6);
      groupBox1.Controls.Add(buttonSelectFolderMotorXls);
      groupBox1.Controls.Add(textBoxMotorLogFolder_xls);
      groupBox1.Controls.Add(label5);
      groupBox1.Controls.Add(buttonXlsConvert);
      groupBox1.Controls.Add(buttonExecute);
      groupBox1.Controls.Add(buttonSelectFolderOutput);
      groupBox1.Controls.Add(textBoxOutputFile);
      groupBox1.Controls.Add(label4);
      groupBox1.Controls.Add(buttonSelectFolderJIssouLogCSV);
      groupBox1.Controls.Add(textBoxJissouLogFolder);
      groupBox1.Controls.Add(label3);
      groupBox1.Controls.Add(buttonSelectFolderMotorLogXlsx);
      groupBox1.Controls.Add(textBoxMotorLogFolder);
      groupBox1.Controls.Add(label2);
      groupBox1.Controls.Add(buttonSelectEDULog);
      groupBox1.Controls.Add(textBoxEDULogFolder);
      groupBox1.Controls.Add(label1);
      groupBox1.Name = "groupBox1";
      groupBox1.TabStop = false;
      // 
      // buttonSelectEDULogManual
      // 
      resources.ApplyResources(buttonSelectEDULogManual, "buttonSelectEDULogManual");
      buttonSelectEDULogManual.Name = "buttonSelectEDULogManual";
      buttonSelectEDULogManual.UseVisualStyleBackColor = true;
      buttonSelectEDULogManual.Click += buttonSelectEDULogManual_Click;
      // 
      // textBoxEDULogFolderManual
      // 
      resources.ApplyResources(textBoxEDULogFolderManual, "textBoxEDULogFolderManual");
      textBoxEDULogFolderManual.Name = "textBoxEDULogFolderManual";
      // 
      // label7
      // 
      resources.ApplyResources(label7, "label7");
      label7.Name = "label7";
      // 
      // buttonSelectFolderJIssouLogCSVManual
      // 
      resources.ApplyResources(buttonSelectFolderJIssouLogCSVManual, "buttonSelectFolderJIssouLogCSVManual");
      buttonSelectFolderJIssouLogCSVManual.Name = "buttonSelectFolderJIssouLogCSVManual";
      buttonSelectFolderJIssouLogCSVManual.UseVisualStyleBackColor = true;
      buttonSelectFolderJIssouLogCSVManual.Click += buttonSelectFolderJIssouLogCSVManual_Click;
      // 
      // textBoxJissouLogFolderManual
      // 
      resources.ApplyResources(textBoxJissouLogFolderManual, "textBoxJissouLogFolderManual");
      textBoxJissouLogFolderManual.Name = "textBoxJissouLogFolderManual";
      // 
      // label6
      // 
      resources.ApplyResources(label6, "label6");
      label6.Name = "label6";
      // 
      // buttonSelectFolderMotorXls
      // 
      resources.ApplyResources(buttonSelectFolderMotorXls, "buttonSelectFolderMotorXls");
      buttonSelectFolderMotorXls.Name = "buttonSelectFolderMotorXls";
      buttonSelectFolderMotorXls.UseVisualStyleBackColor = true;
      buttonSelectFolderMotorXls.Click += buttonSelectFolderMotorXls_Click;
      // 
      // textBoxMotorLogFolder_xls
      // 
      resources.ApplyResources(textBoxMotorLogFolder_xls, "textBoxMotorLogFolder_xls");
      textBoxMotorLogFolder_xls.Name = "textBoxMotorLogFolder_xls";
      // 
      // label5
      // 
      resources.ApplyResources(label5, "label5");
      label5.Name = "label5";
      // 
      // buttonXlsConvert
      // 
      resources.ApplyResources(buttonXlsConvert, "buttonXlsConvert");
      buttonXlsConvert.Name = "buttonXlsConvert";
      buttonXlsConvert.UseVisualStyleBackColor = true;
      buttonXlsConvert.Click += buttonXlsConvert_Click;
      // 
      // buttonExecute
      // 
      resources.ApplyResources(buttonExecute, "buttonExecute");
      buttonExecute.Name = "buttonExecute";
      buttonExecute.UseVisualStyleBackColor = true;
      buttonExecute.Click += buttonExecute_Click;
      // 
      // buttonSelectFolderOutput
      // 
      resources.ApplyResources(buttonSelectFolderOutput, "buttonSelectFolderOutput");
      buttonSelectFolderOutput.Name = "buttonSelectFolderOutput";
      buttonSelectFolderOutput.UseVisualStyleBackColor = true;
      buttonSelectFolderOutput.Click += buttonSelectFolderOutput_Click;
      // 
      // textBoxOutputFile
      // 
      resources.ApplyResources(textBoxOutputFile, "textBoxOutputFile");
      textBoxOutputFile.Name = "textBoxOutputFile";
      // 
      // label4
      // 
      resources.ApplyResources(label4, "label4");
      label4.Name = "label4";
      // 
      // buttonSelectFolderJIssouLogCSV
      // 
      resources.ApplyResources(buttonSelectFolderJIssouLogCSV, "buttonSelectFolderJIssouLogCSV");
      buttonSelectFolderJIssouLogCSV.Name = "buttonSelectFolderJIssouLogCSV";
      buttonSelectFolderJIssouLogCSV.UseVisualStyleBackColor = true;
      buttonSelectFolderJIssouLogCSV.Click += buttonSelectFolderJIssouLogCSV_Click;
      // 
      // textBoxJissouLogFolder
      // 
      resources.ApplyResources(textBoxJissouLogFolder, "textBoxJissouLogFolder");
      textBoxJissouLogFolder.Name = "textBoxJissouLogFolder";
      // 
      // label3
      // 
      resources.ApplyResources(label3, "label3");
      label3.Name = "label3";
      // 
      // buttonSelectFolderMotorLogXlsx
      // 
      resources.ApplyResources(buttonSelectFolderMotorLogXlsx, "buttonSelectFolderMotorLogXlsx");
      buttonSelectFolderMotorLogXlsx.Name = "buttonSelectFolderMotorLogXlsx";
      buttonSelectFolderMotorLogXlsx.UseVisualStyleBackColor = true;
      buttonSelectFolderMotorLogXlsx.Click += buttonSelectFolderMotorLogXlsx_Click;
      // 
      // textBoxMotorLogFolder
      // 
      resources.ApplyResources(textBoxMotorLogFolder, "textBoxMotorLogFolder");
      textBoxMotorLogFolder.Name = "textBoxMotorLogFolder";
      // 
      // label2
      // 
      resources.ApplyResources(label2, "label2");
      label2.Name = "label2";
      // 
      // buttonSelectEDULog
      // 
      resources.ApplyResources(buttonSelectEDULog, "buttonSelectEDULog");
      buttonSelectEDULog.Name = "buttonSelectEDULog";
      buttonSelectEDULog.UseVisualStyleBackColor = true;
      buttonSelectEDULog.Click += buttonSelectEDULog_Click;
      // 
      // textBoxEDULogFolder
      // 
      resources.ApplyResources(textBoxEDULogFolder, "textBoxEDULogFolder");
      textBoxEDULogFolder.Name = "textBoxEDULogFolder";
      // 
      // label1
      // 
      resources.ApplyResources(label1, "label1");
      label1.Name = "label1";
      // 
      // groupBox2
      // 
      resources.ApplyResources(groupBox2, "groupBox2");
      groupBox2.Controls.Add(splitContainer1);
      groupBox2.Name = "groupBox2";
      groupBox2.TabStop = false;
      // 
      // backgroundWorkerMain
      // 
      backgroundWorkerMain.DoWork += backgroundWorker1_DoWork;
      backgroundWorkerMain.ProgressChanged += backgroundWorker1_ProgressChanged;
      backgroundWorkerMain.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
      // 
      // backgroundWorkerMotorConv
      // 
      backgroundWorkerMotorConv.DoWork += backgroundWorkerMotorConv_DoWork;
      backgroundWorkerMotorConv.ProgressChanged += backgroundWorkerMotorConv_ProgressChanged;
      backgroundWorkerMotorConv.RunWorkerCompleted += backgroundWorkerMotorConv_RunWorkerCompleted;
      // 
      // Form1
      // 
      resources.ApplyResources(this, "$this");
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(groupBox2);
      Controls.Add(groupBox1);
      Name = "Form1";
      FormClosing += Form1_FormClosing;
      Load += Form1_Load;
      splitContainer1.Panel1.ResumeLayout(false);
      splitContainer1.Panel1.PerformLayout();
      splitContainer1.Panel2.ResumeLayout(false);
      splitContainer1.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
      splitContainer1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
      ((System.ComponentModel.ISupportInitialize)numericUpDownRowCnt).EndInit();
      groupBox1.ResumeLayout(false);
      groupBox1.PerformLayout();
      groupBox2.ResumeLayout(false);
      ResumeLayout(false);
    }

    #endregion

    private GroupBox groupBox1;
    private Button buttonSelectFolderOutput;
    private TextBox textBoxOutputFile;
    private Label label4;
    private Button buttonSelectFolderJIssouLogCSV;
    private TextBox textBoxJissouLogFolder;
    private Label label3;
    private Button buttonSelectFolderMotorLogXlsx;
    private TextBox textBoxMotorLogFolder;
    private Label label2;
    private Button buttonSelectEDULog;
    private TextBox textBoxEDULogFolder;
    private Label label1;
    private GroupBox groupBox2;
    private Button buttonExecute;
    private Button buttonSelectFolderMotorXls;
    private TextBox textBoxMotorLogFolder_xls;
    private Label label5;
    private Button buttonXlsConvert;
    private TextBox textBoxStatus;
    private System.ComponentModel.BackgroundWorker backgroundWorkerMain;
    private SplitContainer splitContainer1;
    private Button buttonSelectEDULogManual;
    private TextBox textBoxEDULogFolderManual;
    private Label label7;
    private Button buttonSelectFolderJIssouLogCSVManual;
    private TextBox textBoxJissouLogFolderManual;
    private Label label6;
    private System.ComponentModel.BackgroundWorker backgroundWorkerMotorConv;
    private DataGridView dataGridView;
    private NumericUpDown numericUpDownRowCnt;
    private Button buttonOpenFolder;
    private TextBox textBoxFileResult;
    private Label label9;
    private Label label8;
  }
}
