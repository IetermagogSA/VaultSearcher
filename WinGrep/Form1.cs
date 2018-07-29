namespace WinGrep
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Data;
	using System.IO;
	using System.Security;
	using System.Text.RegularExpressions;
	using System.Threading;
	using System.Net;
	using System.ServiceProcess;
	using System.IO.Compression;
	using Microsoft.SqlServer.Management.Smo;
	using SevenZip;
	using System.Linq;
	using System.Text;
	using System.Reflection;
	using System.Collections.Generic;

	/// <summary>
	///    Summary description for FormWinGrep.
	/// </summary>
	public class FormWinGrep : System.Windows.Forms.Form
	{
		/// <summary>
		///    Required designer variable.
		/// </summary>
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.Label lblResults;
		private System.Windows.Forms.TextBox txtResults;
		private System.Windows.Forms.Label lblSearchText;
		private System.Windows.Forms.TextBox txtSearchText;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.Label lblDir;
		private System.Windows.Forms.TextBox txtOutDir;
		private System.Windows.Forms.CheckBox ckRecursive;
		private System.Windows.Forms.CheckBox ckIgnoreCase;
		//The search thread
		private Thread m_searchthread = null;
		private CheckBox chkRestoreSQL;
		private CheckBox chkRunReports;
		private ComboBox cbSQLInstance;
		private Label lblInstanceName;
		private string outputDirectory;
		private Label label1;
		private ComboBox cboApplicationName;
		private CheckBox chkListResults;
		private DataGridView gvResults;
		private BindingSource bindingSourceResults;
		private IContainer components;
		private BindingSource searchResultInfoBindingSource;
		private Button btnDownload;

		//List<SearchResultInfo> myFilesList = new List<SearchResultInfo>();
		List<VaultSearcher.SearchResultInfo> myFilesList = new List<VaultSearcher.SearchResultInfo>();
		List<VaultSearcher.SearchResultInfo> mySortedList = new List<VaultSearcher.SearchResultInfo>();

		string sqlInstance;

        string zipFileExt = ".zip";
        string applicationName = "Vetmaster";

        private Label label2;
		private Button btnReset;
        private Button btnPreviewText;
        private Label label3;
        private TextBox txtPreviewText;
        private DataGridViewTextBoxColumn SearchTerm;
        private DataGridViewTextBoxColumn Files;
        private DataGridViewTextBoxColumn FileSize;
        private DataGridViewTextBoxColumn CreationTime;
        private DataGridViewCheckBoxColumn Select;

        //ArrayList keeping the Files
        ArrayList m_arrFiles = new ArrayList();

		public FormWinGrep()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		delegate void SetTextCallback(string text, bool addNewLine, bool isPreviewText);

		private void SetText(string text, bool addNewLine, bool isPreviewText)
		{
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
			if (this.txtResults.InvokeRequired || this.txtPreviewText.InvokeRequired)
			{
				SetTextCallback d = new SetTextCallback(SetText);
				this.Invoke(d, new object[] { text, addNewLine, isPreviewText });
			}
			else
			{
                if (addNewLine)
                {
                    if (isPreviewText)
                        this.txtPreviewText.Text += text + Environment.NewLine;
                    else
                        this.txtResults.Text += text;
                }
                else
                {
                    if (isPreviewText)
                        this.txtPreviewText.Text += text + Environment.NewLine + Environment.NewLine;
                    else
                        this.txtResults.Text += text + Environment.NewLine;
                }
			}
		}

		/// <summary>
		///    Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if(disposing)
				{
					// Release the added managed resources
					btnBrowse.Dispose();
					lblResults.Dispose();
					txtResults.Dispose();
					lblSearchText.Dispose();
					txtSearchText.Dispose();
					//lblFiles.Dispose();
					//txtFiles.Dispose();
					btnSearch.Dispose();
					ckRecursive.Dispose();
					lblDir.Dispose();
					txtOutDir.Dispose();
				}
			}
			finally
			{
				// Call Dispose on your base class.
				base.Dispose(disposing);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWinGrep));
            this.txtResults = new System.Windows.Forms.TextBox();
            this.lblResults = new System.Windows.Forms.Label();
            this.lblDir = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtSearchText = new System.Windows.Forms.TextBox();
            this.lblSearchText = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtOutDir = new System.Windows.Forms.TextBox();
            this.ckRecursive = new System.Windows.Forms.CheckBox();
            this.ckIgnoreCase = new System.Windows.Forms.CheckBox();
            this.chkRestoreSQL = new System.Windows.Forms.CheckBox();
            this.chkRunReports = new System.Windows.Forms.CheckBox();
            this.cbSQLInstance = new System.Windows.Forms.ComboBox();
            this.lblInstanceName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboApplicationName = new System.Windows.Forms.ComboBox();
            this.chkListResults = new System.Windows.Forms.CheckBox();
            this.gvResults = new System.Windows.Forms.DataGridView();
            this.SearchTerm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Files = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreationTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.bindingSourceResults = new System.Windows.Forms.BindingSource(this.components);
            this.searchResultInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnDownload = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnPreviewText = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPreviewText = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.gvResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchResultInfoBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // txtResults
            // 
            this.txtResults.BackColor = System.Drawing.SystemColors.Info;
            this.txtResults.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.txtResults.Enabled = false;
            this.txtResults.Location = new System.Drawing.Point(742, 19);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ReadOnly = true;
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResults.Size = new System.Drawing.Size(299, 153);
            this.txtResults.TabIndex = 9;
            this.txtResults.WordWrap = false;
            // 
            // lblResults
            // 
            this.lblResults.Location = new System.Drawing.Point(8, 235);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(56, 12);
            this.lblResults.TabIndex = 10;
            this.lblResults.Text = "Results";
            // 
            // lblDir
            // 
            this.lblDir.Location = new System.Drawing.Point(8, 179);
            this.lblDir.Name = "lblDir";
            this.lblDir.Size = new System.Drawing.Size(84, 12);
            this.lblDir.TabIndex = 0;
            this.lblDir.Text = "Output Directory";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(452, 194);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(28, 20);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "...";
            this.btnBrowse.Click += new System.EventHandler(this.btnDir_Click);
            // 
            // txtSearchText
            // 
            this.txtSearchText.BackColor = System.Drawing.SystemColors.Window;
            this.txtSearchText.Location = new System.Drawing.Point(230, 19);
            this.txtSearchText.Multiline = true;
            this.txtSearchText.Name = "txtSearchText";
            this.txtSearchText.Size = new System.Drawing.Size(250, 153);
            this.txtSearchText.TabIndex = 0;
            this.txtSearchText.TextChanged += new System.EventHandler(this.txtSearchText_TextChanged);
            this.txtSearchText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchText_KeyDown);
            // 
            // lblSearchText
            // 
            this.lblSearchText.Location = new System.Drawing.Point(227, 4);
            this.lblSearchText.Name = "lblSearchText";
            this.lblSearchText.Size = new System.Drawing.Size(196, 12);
            this.lblSearchText.TabIndex = 6;
            this.lblSearchText.Text = "Search Words (Each on seperate line)";
            // 
            // btnSearch
            // 
            this.btnSearch.Enabled = false;
            this.btnSearch.Location = new System.Drawing.Point(509, 194);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(79, 38);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtOutDir
            // 
            this.txtOutDir.BackColor = System.Drawing.SystemColors.Window;
            this.txtOutDir.Enabled = false;
            this.txtOutDir.Location = new System.Drawing.Point(8, 194);
            this.txtOutDir.Name = "txtOutDir";
            this.txtOutDir.Size = new System.Drawing.Size(436, 20);
            this.txtOutDir.TabIndex = 1;
            this.txtOutDir.Text = "C:\\";
            this.txtOutDir.TextChanged += new System.EventHandler(this.txtDir_TextChanged);
            this.txtOutDir.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDir_KeyDown);
            // 
            // ckRecursive
            // 
            this.ckRecursive.Checked = true;
            this.ckRecursive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckRecursive.Location = new System.Drawing.Point(8, 12);
            this.ckRecursive.Name = "ckRecursive";
            this.ckRecursive.Size = new System.Drawing.Size(84, 25);
            this.ckRecursive.TabIndex = 13;
            this.ckRecursive.Text = "Recursive";
            // 
            // ckIgnoreCase
            // 
            this.ckIgnoreCase.Checked = true;
            this.ckIgnoreCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckIgnoreCase.Location = new System.Drawing.Point(110, 12);
            this.ckIgnoreCase.Name = "ckIgnoreCase";
            this.ckIgnoreCase.Size = new System.Drawing.Size(94, 25);
            this.ckIgnoreCase.TabIndex = 15;
            this.ckIgnoreCase.Text = "Ignore Case";
            // 
            // chkRestoreSQL
            // 
            this.chkRestoreSQL.Location = new System.Drawing.Point(8, 39);
            this.chkRestoreSQL.Name = "chkRestoreSQL";
            this.chkRestoreSQL.Size = new System.Drawing.Size(96, 25);
            this.chkRestoreSQL.TabIndex = 15;
            this.chkRestoreSQL.Text = "Restore to sql";
            this.chkRestoreSQL.CheckedChanged += new System.EventHandler(this.chkRestoreSQL_CheckedChanged);
            // 
            // chkRunReports
            // 
            this.chkRunReports.Enabled = false;
            this.chkRunReports.Location = new System.Drawing.Point(110, 39);
            this.chkRunReports.Name = "chkRunReports";
            this.chkRunReports.Size = new System.Drawing.Size(94, 25);
            this.chkRunReports.TabIndex = 15;
            this.chkRunReports.Text = "Run Reports";
            // 
            // cbSQLInstance
            // 
            this.cbSQLInstance.Enabled = false;
            this.cbSQLInstance.FormattingEnabled = true;
            this.cbSQLInstance.Items.AddRange(new object[] {
            "SQL2005",
            "SQLEXPRESS",
            "SQL2008",
            "SQL2014"});
            this.cbSQLInstance.Location = new System.Drawing.Point(8, 109);
            this.cbSQLInstance.Name = "cbSQLInstance";
            this.cbSQLInstance.Size = new System.Drawing.Size(139, 21);
            this.cbSQLInstance.TabIndex = 16;
            this.cbSQLInstance.Text = "SQL2005";
            // 
            // lblInstanceName
            // 
            this.lblInstanceName.Location = new System.Drawing.Point(8, 93);
            this.lblInstanceName.Name = "lblInstanceName";
            this.lblInstanceName.Size = new System.Drawing.Size(139, 12);
            this.lblInstanceName.TabIndex = 0;
            this.lblInstanceName.Text = "SQL Instance Name";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Application Name";
            // 
            // cboApplicationName
            // 
            this.cboApplicationName.FormattingEnabled = true;
            this.cboApplicationName.Items.AddRange(new object[] {
            "Vetmaster",
            "Estatemaster",
            "Groomaster",
            "Cyclemaster"});
            this.cboApplicationName.Location = new System.Drawing.Point(8, 151);
            this.cboApplicationName.Name = "cboApplicationName";
            this.cboApplicationName.Size = new System.Drawing.Size(139, 21);
            this.cboApplicationName.TabIndex = 16;
            this.cboApplicationName.Text = "Vetmaster";
            // 
            // chkListResults
            // 
            this.chkListResults.Checked = true;
            this.chkListResults.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkListResults.Location = new System.Drawing.Point(8, 65);
            this.chkListResults.Name = "chkListResults";
            this.chkListResults.Size = new System.Drawing.Size(186, 25);
            this.chkListResults.TabIndex = 15;
            this.chkListResults.Text = "Select from a list of backup files";
            this.chkListResults.CheckedChanged += new System.EventHandler(this.chkListResults_CheckedChanged);
            // 
            // gvResults
            // 
            this.gvResults.AllowUserToAddRows = false;
            this.gvResults.AllowUserToDeleteRows = false;
            this.gvResults.AllowUserToOrderColumns = true;
            this.gvResults.AutoGenerateColumns = false;
            this.gvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SearchTerm,
            this.Files,
            this.FileSize,
            this.CreationTime,
            this.Select});
            this.gvResults.DataSource = this.bindingSourceResults;
            this.gvResults.Location = new System.Drawing.Point(8, 250);
            this.gvResults.Name = "gvResults";
            this.gvResults.Size = new System.Drawing.Size(610, 150);
            this.gvResults.TabIndex = 17;
            this.gvResults.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvResults_CellValueChanged);
            // 
            // SearchTerm
            // 
            this.SearchTerm.DataPropertyName = "SearchTerm";
            this.SearchTerm.FillWeight = 30F;
            this.SearchTerm.HeaderText = "SearchTerm";
            this.SearchTerm.Name = "SearchTerm";
            this.SearchTerm.Width = 90;
            // 
            // Files
            // 
            this.Files.DataPropertyName = "FileName";
            this.Files.HeaderText = "Files";
            this.Files.Name = "Files";
            this.Files.Width = 53;
            // 
            // FileSize
            // 
            this.FileSize.DataPropertyName = "FileSize";
            this.FileSize.HeaderText = "File Size (MB)";
            this.FileSize.Name = "FileSize";
            this.FileSize.Width = 96;
            // 
            // CreationTime
            // 
            this.CreationTime.DataPropertyName = "CreationTime";
            this.CreationTime.FillWeight = 70F;
            this.CreationTime.HeaderText = "CreationTime";
            this.CreationTime.Name = "CreationTime";
            this.CreationTime.Width = 94;
            // 
            // Select
            // 
            this.Select.DataPropertyName = "Selected";
            this.Select.FillWeight = 30F;
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            this.Select.Width = 43;
            // 
            // searchResultInfoBindingSource
            // 
            this.searchResultInfoBindingSource.DataSource = typeof(WinGrep.FormWinGrep.SearchResultInfo);
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(433, 406);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(175, 30);
            this.btnDownload.TabIndex = 3;
            this.btnDownload.Text = "Download Selected Files";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(739, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(196, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "Progress";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(961, 194);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(80, 40);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Reset";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnPreviewText
            // 
            this.btnPreviewText.Location = new System.Drawing.Point(634, 303);
            this.btnPreviewText.Name = "btnPreviewText";
            this.btnPreviewText.Size = new System.Drawing.Size(80, 40);
            this.btnPreviewText.TabIndex = 4;
            this.btnPreviewText.Text = "Preview Text File";
            this.btnPreviewText.Click += new System.EventHandler(this.btnPreviewText_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(739, 235);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(196, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Preview Text";
            // 
            // txtPreviewText
            // 
            this.txtPreviewText.BackColor = System.Drawing.SystemColors.Info;
            this.txtPreviewText.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.txtPreviewText.Location = new System.Drawing.Point(742, 250);
            this.txtPreviewText.Multiline = true;
            this.txtPreviewText.Name = "txtPreviewText";
            this.txtPreviewText.ReadOnly = true;
            this.txtPreviewText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtPreviewText.Size = new System.Drawing.Size(299, 153);
            this.txtPreviewText.TabIndex = 9;
            this.txtPreviewText.WordWrap = false;
            // 
            // FormWinGrep
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1082, 454);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.gvResults);
            this.Controls.Add(this.cboApplicationName);
            this.Controls.Add(this.cbSQLInstance);
            this.Controls.Add(this.chkListResults);
            this.Controls.Add(this.chkRestoreSQL);
            this.Controls.Add(this.chkRunReports);
            this.Controls.Add(this.ckIgnoreCase);
            this.Controls.Add(this.ckRecursive);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.lblResults);
            this.Controls.Add(this.txtPreviewText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblSearchText);
            this.Controls.Add(this.txtSearchText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPreviewText);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.lblInstanceName);
            this.Controls.Add(this.lblDir);
            this.Controls.Add(this.txtOutDir);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormWinGrep";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Vault Search";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormWinGrep_FormClosed);
            this.Load += new System.EventHandler(this.FormWinGrep_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchResultInfoBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		protected void SearchThread()
		{
			//if(m_searchthread == null)
			//{
			//	//Start Searching Thread
			//	m_searchthread = new Thread(new ThreadStart(Search));
			//	m_searchthread.IsBackground = true;
			//	m_searchthread.Start();
			//	btnSearch.Text = "Stop";
			//}
			//else //bSearching != null
			//{
			//	//Stop Thread
			//	m_searchthread.Abort();
			//	txtResults.Text = "User Requested Search Abortion!";
			//}
		}

		protected void txtFiles_KeyDown (object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode==Keys.Enter && btnSearch.Enabled==true)
			{
				SearchThread();
			}
		}

		protected void txtDir_KeyDown (object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode==Keys.Enter && btnSearch.Enabled==true)
			{
				SearchThread();
			}
		}

		protected void txtSearchText_KeyDown (object sender, System.Windows.Forms.KeyEventArgs e)
		{
			//if(e.KeyCode==Keys.Enter && btnSearch.Enabled==true)
			//{
			//	SearchThread();
			//}
		}

		//Build the list of Files
		protected void GetFiles(String strDir, String strExt, bool bRecursive)
		{
			//search pattern can include the wild characters '*' and '?'
			string[] fileList = Directory.GetFiles(strDir, strExt);
			for(int i=0; i<fileList.Length; i++)
			{
				if(File.Exists(fileList[i]))
					m_arrFiles.Add(fileList[i]);
			}
			if(bRecursive==true)
			{
				//Get recursively from subdirectories
				string[] dirList = Directory.GetDirectories(strDir);
				for(int i=0; i<dirList.Length; i++)
				{
					GetFiles(dirList[i], strExt, true);
				}
			}
		}

		private void downloadFiles(VaultSearcher.SearchResultInfo searchResults, bool textFileOnly)
		{
			string fileToDownload = searchResults.FileName;

			for(int i = 0; i < 2; i++) // Loop twice - once to download zip file and another for the text file
			{
                if (textFileOnly)
                    ++i; // Increment the counter to skip to the text file if that is the only one we want

				fileToDownload = i == 0 ? fileToDownload : fileToDownload.Replace(zipFileExt, ".txt");

				string serverpath = "ftp://<<ipaddress>>/vault/" + fileToDownload;
				FtpWebRequest myRequest = (FtpWebRequest)WebRequest.Create(serverpath);

				myRequest.KeepAlive = true;
				myRequest.UsePassive = true;
				myRequest.UseBinary = true;

				myRequest.Method = WebRequestMethods.Ftp.DownloadFile;

                myRequest.Credentials = new NetworkCredential("vetmaster.co.za", "<<password>>");
                switch (applicationName)
                {
                    case "Vetmaster":
                        myRequest.Credentials = new NetworkCredential("vetmaster.co.za",  "<<password>>");
                        break;
                    case "Estatemaster":
                        myRequest.Credentials = new NetworkCredential("EstateFTP", " <<password>>");
                        break;
                    case "Groomaster":
                        myRequest.Credentials = new NetworkCredential("GrooMasterFTP", " <<password>>");
                        break;
                    case "Cyclemaster":
                        myRequest.Credentials = new NetworkCredential("CycleMasterFTP", " <<password>>");
                        break;
                }


				using (FtpWebResponse response = (FtpWebResponse)myRequest.GetResponse())
				using (Stream responseStream = response.GetResponseStream())
				using (FileStream ws = new FileStream(outputDirectory + "\\" + fileToDownload, FileMode.Create))
				{
					byte[] buffer = new byte[2048];
					int bytesRead = responseStream.Read(buffer, 0, buffer.Length);

					while (bytesRead > 0)
					{
						ws.Write(buffer, 0, bytesRead);
						bytesRead = responseStream.Read(buffer, 0, buffer.Length);
					}
				}
			}
		}

		protected void RestoreSQLDatabase(string filename)
		{
			try
			{
				string fullBackupName = string.Concat(outputDirectory, "\\", filename.Replace(zipFileExt,".BAK"));

				Restore rstDatabase = new Restore();
				rstDatabase.Action = RestoreActionType.Database;
				//rstDatabase.Database = filename.Replace(".zip", ".BAK");
				rstDatabase.Database = "Vetmaster";

				BackupDeviceItem bkpDevice = new BackupDeviceItem(fullBackupName, DeviceType.File);

				rstDatabase.Devices.Add(bkpDevice);
				rstDatabase.ReplaceDatabase = true;

				Server srvSQL = new Server(String.Concat("localhost\\", sqlInstance));
				srvSQL.KillAllProcesses(rstDatabase.Database);

				rstDatabase.SqlRestore(srvSQL);
			}
			catch(Exception ex)
			{
				MessageBox.Show("Error: " + ex.Message);
			}
		}

		protected void UnzipDatabases(string zipFile)
		{
			try
			{
				string fullarchive = string.Concat(outputDirectory, "\\", zipFile);
				//ZipFile.ExtractToDirectory(fullarchive, outDir);
				if (IntPtr.Size == 8) //x64
				{
					SevenZip.SevenZipExtractor.SetLibraryPath(@"C:\Program Files\7-Zip\7z.dll");
				}
				else //x86
				{
                    //SevenZip.SevenZipCompressor.SetLibraryPath(@"C:\Program Files (x86)\7-Zip\7z.dll");
                    SevenZip.SevenZipCompressor.SetLibraryPath(@"C:\Program Files\7-Zip\7z.dll");
                }
                //SevenZip.SevenZipCompressor.SetLibraryPath(
                //Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"7z.dll"));
                using (var file = new SevenZipExtractor(fullarchive))
                {
                    file.ExtractArchive(outputDirectory);
                }
            }
			catch(Exception ex)
			{
				MessageBox.Show("Error: ", ex.Message);
			}
		}

		public string ConvertToCSV(IQueryable query, string replacementDelimiter)
		{
				// Create the csv by looping through each row and then each field in each row
				// seperating the columns by commas

				// String builder for our header row
				StringBuilder header = new StringBuilder();

				// Get the properties (aka columns) to set in the header row
				PropertyInfo[] rowPropertyInfos = null;
				rowPropertyInfos = query.ElementType.GetProperties();

				// Setup header row 
				foreach (PropertyInfo info in rowPropertyInfos)
				{
					if (info.CanRead)
					{
						header.Append(info.Name + ",");
					}
				}

				// New row
				header.Append("\r\n");

				// String builder for our data rows
				StringBuilder data = new StringBuilder();

				// Setup data rows
				foreach (var myObject in query)
				{

					// Loop through fields in each row seperating each by commas and replacing 
					// any commas in each field name with replacement delimiter
					foreach (PropertyInfo info in rowPropertyInfos)
					{
						if (info.CanRead)
						{

							// Get the fields value and then replace any commas with the replacement delimeter
							string tmp = Convert.ToString(info.GetValue(myObject, null));
							if (!String.IsNullOrEmpty(tmp))
							{
								if(info.Name == "Period" && tmp.Length == 6)
								{
									tmp = tmp.Insert(tmp.IndexOf('/',0) + 1, "0");
								}
								tmp.Replace(",", replacementDelimiter);
							}
							data.Append(tmp + ",");
						}
					}

					// New row
					data.Append("\r\n");
				}

				// Check the data results... if they are empty then return an empty string
				// otherwise append the data to the header
				string result = data.ToString();
				if (string.IsNullOrEmpty(result) == false)
				{
					header.Append(result);
					return header.ToString();
				}
				else
				{
					return string.Empty;
				}
		}

		//interface IqueryResult
		//{
		//    string Period { get; }
		//    string Category { get; }
		//    string Description { get; }
		//    decimal Quantity { get; }
		//    decimal TotalSellingPriceInclVAT { get; }
		//}

		//public class queryResult : IqueryResult
		//{
		//    public queryResult(string period, string category, string description, decimal quantity, decimal totalSellingPriceIncl)
		//    {
		//        Period = period;
		//        Category = category;
		//        Description = description;
		//        Quantity = quantity;
		//        TotalSellingPriceInclVAT = totalSellingPriceIncl;
		//    }

		//    public string Period { get; private set; }
		//    public string Category { get; private set; }
		//    public string Description { get; private set; }
		//    public decimal Quantity { get; private set; }
		//    public decimal TotalSellingPriceInclVAT { get; private set; }
		//}

		public class queryResult
		{
			public string Period { get; set; }
			public string Category { get; set; }
			public string Description { get; set; }
			public decimal Quantity { get; set; }
			public decimal TotalSellingPriceInclVAT { get; set; }
		}

		//public List<queryResult> runQuery()
		//{
		//    Linq.DataClassesDataContext myDB = new Linq.DataClassesDataContext();
		//    string companyInfo = (from ci in myDB.CompanyInfos
		//                          select ci.Name).FirstOrDefault();

		//    var results = (from q in (from sd in myDB.SaleDetails
		//                              join i in myDB.Inventories on sd.InventoryID equals i.ID
		//                              join ic in myDB.InventoryCategories on i.CategoryID equals ic.ID
		//                              where sd.Quantity > 0 && ic.IsService == false &&
		//                              !ic.Name.Contains("consult") &&
		//                              !ic.Name.Contains("consumable") &&
		//                              !ic.Name.Contains("drug") &&
		//                              !ic.Name.Contains("ethical") &&
		//                              !ic.Name.Contains("fee") &&
		//                              !ic.Name.Contains("labarator") &&
		//                              !ic.Name.Contains("service")
		//                              select new { sd, i, ic })
		//                   group q by new { q.ic.Name, q.i.Description, q.sd.SaleDate.Year, q.sd.SaleDate.Month } into g
		//                   select new queryResult
		//                   {
		//                       Period = String.Concat(g.Key.Year, "/", g.Key.Month.ToString("d2")),
		//                       Category = g.Key.Name,
		//                       Description = g.Key.Description,
		//                       Quantity = g.Sum(s => s.sd.Quantity),
		//                       TotalSellingPriceInclVAT = g.Sum(s => s.sd.Quantity * s.sd.SellingPriceInclusive)
		//                   }).OrderBy(i => i.Description).ThenBy(i => i.Period).ToList();

		//    return results;
		//}

		protected void RunGPKQuery()
		{
			Linq.DataClassesDataContext myDB = new Linq.DataClassesDataContext();
			string companyInfo = (from ci in myDB.CompanyInfos
								  select ci.Name).FirstOrDefault();

			DateTime currentDate = DateTime.Now;
			DateTime fromDate = new DateTime(currentDate.Year, currentDate.AddMonths(-1).Month, 1);
			DateTime toDate = new DateTime(currentDate.Year, currentDate.Month, 1);

			string fullPath = string.Concat(outputDirectory, "\\Sales Reports - ", companyInfo, ".csv");

			var results = (from q in (from sd in myDB.SaleDetails
									  join i in myDB.Inventories on sd.InventoryID equals i.ID
									  join ic in myDB.InventoryCategories on i.CategoryID equals ic.ID
									  where sd.Quantity > 0 && ic.IsService == false &&
									  !ic.Name.Contains("consult") &&
									  !ic.Name.Contains("consumable") &&
									  !ic.Name.Contains("drug") &&
									  !ic.Name.Contains("ethical") &&
									  !ic.Name.Contains("fee") &&
									  !ic.Name.Contains("labarator") &&
									  !ic.Name.Contains("service") &&
									  sd.SaleDate >= fromDate && sd.SaleDate <= toDate
									  select new { sd, i, ic })
						   group q by new { q.ic.Name, q.i.Description, q.sd.SaleDate.Year, q.sd.SaleDate.Month } into g
						   select new 
						   {
							   Period = String.Concat(g.Key.Year, "/", g.Key.Month),
							   Category = g.Key.Name,
							   Description = g.Key.Description,
							   Quantity = g.Sum(s => s.sd.Quantity),
							   TotalSellingPriceInclVAT = g.Sum(s => s.sd.Quantity * s.sd.SellingPriceInclusive)
						   }).OrderBy(i => i.Description).ThenBy(i => i.Period);

			string queryText = ConvertToCSV(results, ",");
			using (StreamWriter sw = new StreamWriter(fullPath))
			{
				sw.WriteLine(queryText);
			}
		}

		public class SearchResultInfo
		{
			public SearchResultInfo(FileInfo myFile, string term)
			{
				SearchTerm = term;
				FileName = myFile.Name;
				CreationTime = myFile.CreationTime;
                FileSize = myFile.Length / 1048576; // File size in MB
                Selected = false;
			}

			private string _searchterm;
			public string SearchTerm { get { return _searchterm; } set { _searchterm = value; } }

			private string _fileName;
			public string FileName
			{
				get { return _fileName; }
				set { _fileName = value; }
			}

            private double _fileSize;
            public double FileSize {
                get { return _fileSize; }
                set { _fileSize = value; }
            }

            private DateTime _creationTime;
			public DateTime CreationTime
			{
				get { return _creationTime; }
				set { _creationTime = value; }
			}

			private Boolean _selected;
			public Boolean Selected
			{
				get { return _selected; }
				set { _selected = value; }
			}
		}

		public void downloadThread()
		{
			foreach (VaultSearcher.SearchResultInfo searchResult in mySortedList)
			{
				if (!searchResult.Selected) // Skip the files we did not select
					continue;

				outputDirectory = string.Concat(txtOutDir.Text, "\\", searchResult.SearchTerm);
				if (!Directory.Exists(outputDirectory))
				{
					Directory.CreateDirectory(outputDirectory);
				}

				SetText("Downloading files...", true, false);
				downloadFiles(searchResult, false);
				SetText("DONE!", false, false);

				// Restore the database and then run the query on it
				if (chkRestoreSQL.Checked)
				{
					if (searchResult.FileName.Contains(zipFileExt))
					{
						SetText("Unzipping...", true, false);
						UnzipDatabases(searchResult.FileName);
						SetText("DONE!",false, false);

						SetText("Restoring database...", true, false);
						RestoreSQLDatabase(searchResult.FileName);
						SetText("DONE!", false, false);

						if (chkRunReports.Checked)
						{
							SetText("Running report queries...", true, false);
							RunGPKQuery();
							SetText("DONE!", false, false);
						}
					}
				}
			}
		}

        private void saveSettings()
        {
            // Save the user's settings
            Properties.Settings.Default.OutputDirectory = txtOutDir.Text;
            Properties.Settings.Default.SQLInstanceName = cbSQLInstance.Text;
            Properties.Settings.Default.ApplicationName = cboApplicationName.Text;
            Properties.Settings.Default.Save();
        }

		protected void btnSearch_Click (object sender, System.EventArgs e)
		{
            sqlInstance = cbSQLInstance.Text;
			btnSearch.Enabled = false;
			txtResults.Clear();
			outputDirectory = txtOutDir.Text;

			if (!Directory.Exists(outputDirectory))
			{
				MessageBox.Show("Output directory does not exist!");
				return;
			}
			else
			{
				if (btnSearch.Text == "Cancel")
					return;
				else
				{
					btnSearch.Text = "Cancel";

                    applicationName = cboApplicationName.Text;
					string searchDir = @"C:\inetpub\wwwroot\vetmaster\vault"; // default directory
					switch (applicationName)
					{
						case "Vetmaster":
							searchDir = @"C:\inetpub\wwwroot\vetmaster\vault";
							break;
						case "Estatemaster":
							searchDir = @"C:\inetpub\ftproot\EstateFTP\vault";
							break;
						case "Groomaster":
							searchDir = @"C:\inetpub\ftproot\GrooMasterFTP\vault";
							break;
						case "Cyclemaster":
							searchDir = @"C:\inetpub\ftproot\CycleMasterFTP\vault";
							break;
					}

                    // Determine the kind of zipped backup file
                    zipFileExt = ".zip";
                    switch (applicationName)
                    {
                        case "Vetmaster":
                            zipFileExt = ".zip";
                            break;
                        case "Estatemaster":
                            zipFileExt = ".7z";
                            break;
                        case "Groomaster":
                            zipFileExt = ".zip";
                            break;
                        case "Cyclemaster":
                            zipFileExt = ".zip";
                            break;
                    }

                    // Get the files
                    SetText("Searching for results...", true, false);
					VaultSearcher.VaultSearcherSoapClient myVaultSearcher = new VaultSearcher.VaultSearcherSoapClient();
					try
					{
                        myFilesList = myVaultSearcher.Search(txtSearchText.Text, searchDir, ckRecursive.Checked, ckIgnoreCase.Checked, chkListResults.Checked, zipFileExt).Cast<VaultSearcher.SearchResultInfo>().ToList();
					}
					catch (ArgumentNullException ex)
					{
						SetText(Environment.NewLine + "NO RESULTS FOUND!",true, false);
                        setSearchButton(true);
                        return;
					}
					mySortedList = myFilesList.OrderBy(o=>o.SearchTerm).ThenByDescending(o => o.CreationTime).ToList();
					SetText("DONE!", false, false);

                    setSearchButton(true);

					if (chkListResults.Checked)
					{
						bindingSourceResults.DataSource = mySortedList;
						gvResults.AutoGenerateColumns = false;
					}
					else
					{
						downloadRestoreSelected();
					}
                }
			}
		}

        private void setSearchButton(bool enable)
        {
            if (enable)
            {
                btnSearch.Text = "Search";
                btnSearch.Enabled = true;
            }
            else
            {
                btnSearch.Enabled = false;
            }
        }

		public void downloadRestoreSelected()
		{
			BackgroundWorker bw = new BackgroundWorker();
			bw.DoWork += new DoWorkEventHandler(bw_DoWork);
			bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

			bw.RunWorkerAsync();

		}

		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			downloadThread();
		}

		private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
            setSearchButton(true);

			SetText("All Done!", true, false);
		}

		protected void VerifySearchBtn()
		{
			if(txtOutDir.Text != "" && txtSearchText.Text != "")
			{
				btnSearch.Enabled = true;
			}
			else
				btnSearch.Enabled = false;
		}

		protected void txtSearchText_TextChanged (object sender, System.EventArgs e)
		{
			VerifySearchBtn();
		}

		protected void txtDir_TextChanged (object sender, System.EventArgs e)
		{
			VerifySearchBtn();
		}

		protected void btnDir_Click (object sender, System.EventArgs e)
		{
			FolderBrowserDialog fldg = new FolderBrowserDialog();
			if(fldg.ShowDialog() == DialogResult.OK)
			{
				txtOutDir.Text = fldg.SelectedPath;
			}
		}

		private void ckJustFiles_Click(object sender, System.EventArgs e)
		{
			//if(ckJustFiles.Checked == true)
			//{
			//	ckLineNumbers.Enabled = false;
			//	ckCountLines.Enabled = false;
			//}
			//else
			//{
			//	ckLineNumbers.Enabled = true;
			//	ckCountLines.Enabled = true;
			//}
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main(string[] args) 
		{
			Application.Run(new FormWinGrep());
		}

		private void chkRestoreSQL_CheckedChanged(object sender, EventArgs e)
		{
			if (chkRestoreSQL.Checked)
			{
				cbSQLInstance.Enabled = true;
				chkRunReports.Enabled = true;
			}
			else
			{
				cbSQLInstance.Enabled = false;
				chkRunReports.Enabled = false;
			}
		}

		private void FormWinGrep_Load(object sender, EventArgs e)
		{
            cbSQLInstance.Text = Properties.Settings.Default.SQLInstanceName;
            cboApplicationName.Text = Properties.Settings.Default.ApplicationName;
            txtOutDir.Text = Properties.Settings.Default.OutputDirectory;
		}

		private void chkListResults_CheckedChanged(object sender, EventArgs e)
		{
			if(chkListResults.Checked)
			{
				gvResults.Visible = true;
				btnDownload.Visible = true;
			}
			else
			{
				gvResults.Visible = false;
				btnDownload.Visible = false;
			}
		}

		private void btnDownload_Click(object sender, EventArgs e)
		{
            if (mySortedList.Count > 0)
            {
                btnDownload.Enabled = false;
                gvResults.EndEdit();
                bindingSourceResults.EndEdit();

                downloadRestoreSelected();
                btnDownload.Enabled = true;
            }
            else
            {
                MessageBox.Show("No items have been selected for download!");
            }
		}

		private void gvResults_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 3 && e.RowIndex > -1)
				mySortedList[e.RowIndex].Selected = (bool)gvResults.Rows[e.RowIndex].Cells[3].Value;
		}

		private void btnReset_Click(object sender, EventArgs e)
		{
			ClearInputs();
		}

		private void ClearInputs()
		{
            setSearchButton(false);
			txtResults.Clear();
			txtSearchText.Clear();
            txtPreviewText.Clear();
		}

        private void downloadPreviewTextFile()
        {

            BackgroundWorker bw_PreviewTextFile = new BackgroundWorker();
            bw_PreviewTextFile.DoWork += new DoWorkEventHandler(bw_PreviewTextFile_DoWork);
            bw_PreviewTextFile.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_PreviewTextFile_RunWorkerCompleted);

            bw_PreviewTextFile.RunWorkerAsync();

        }

        private void bw_PreviewTextFile_DoWork(object sender, DoWorkEventArgs e)
        {
            previewTextThread();
        }

        private void bw_PreviewTextFile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //setSearchButton(true);

            //SetText("All Done!", true);
        }

        private void btnPreviewText_Click(object sender, EventArgs e)
        {
            if (mySortedList.Count > 0)
            {
                txtPreviewText.Clear();

                btnPreviewText.Enabled = false;
                gvResults.EndEdit();
                bindingSourceResults.EndEdit();

                downloadPreviewTextFile();
                btnPreviewText.Enabled = true;
            }
            else
            {
                MessageBox.Show("No items have been selected for download!");
            }
        }

        private void previewTextThread()
        {
            foreach (VaultSearcher.SearchResultInfo searchResult in mySortedList)
            {
                if (!searchResult.Selected) // Skip the files we did not select
                    continue;

                outputDirectory = string.Concat(txtOutDir.Text, "\\", searchResult.SearchTerm);
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                downloadFiles(searchResult, true);

                // After downloading the files we want, restore them
                string fileToPreview = outputDirectory + "\\" + searchResult.FileName.Replace(zipFileExt, ".txt");

                string text = String.Empty;
                using (StreamReader sr = new StreamReader(fileToPreview))
                {
                    text = sr.ReadToEnd();
                }
                SetText(text, true, true);
            }
            }

        private void FormWinGrep_FormClosed(object sender, FormClosedEventArgs e)
        {
            saveSettings();
        }
    }
}

