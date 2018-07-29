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
		private System.Windows.Forms.Label lblFiles;
		private System.Windows.Forms.TextBox txtFiles;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.Label lblDir;
		private System.Windows.Forms.TextBox txtDir;
		private System.Windows.Forms.Label lblCurFile;
		private System.Windows.Forms.TextBox txtCurFile;
		private System.Windows.Forms.CheckBox ckCountLines;
		private System.Windows.Forms.CheckBox ckRecursive;
		private System.Windows.Forms.CheckBox ckLineNumbers;
		private System.Windows.Forms.CheckBox ckIgnoreCase;
		private System.Windows.Forms.CheckBox ckJustFiles;
		//The search thread
		private Thread m_searchthread = null;
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
					lblFiles.Dispose();
					txtFiles.Dispose();
					btnSearch.Dispose();
					ckRecursive.Dispose();
					lblDir.Dispose();
					txtDir.Dispose();
					lblCurFile.Dispose();
					txtCurFile.Dispose();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormWinGrep));
			this.txtResults = new System.Windows.Forms.TextBox();
			this.lblResults = new System.Windows.Forms.Label();
			this.txtFiles = new System.Windows.Forms.TextBox();
			this.lblDir = new System.Windows.Forms.Label();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.txtSearchText = new System.Windows.Forms.TextBox();
			this.lblFiles = new System.Windows.Forms.Label();
			this.lblSearchText = new System.Windows.Forms.Label();
			this.btnSearch = new System.Windows.Forms.Button();
			this.txtDir = new System.Windows.Forms.TextBox();
			this.ckCountLines = new System.Windows.Forms.CheckBox();
			this.ckCountLines.Checked = true;
			this.lblCurFile = new System.Windows.Forms.Label();
			this.txtCurFile = new System.Windows.Forms.TextBox();
			this.ckRecursive = new System.Windows.Forms.CheckBox();
			this.ckRecursive.Checked = true;
			this.ckLineNumbers = new System.Windows.Forms.CheckBox();
			this.ckLineNumbers.Checked = true;
			this.ckIgnoreCase = new System.Windows.Forms.CheckBox();
			this.ckJustFiles = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// txtResults
			// 
			this.txtResults.BackColor = System.Drawing.SystemColors.Info;
			this.txtResults.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.txtResults.Location = new System.Drawing.Point(12, 204);
			this.txtResults.Multiline = true;
			this.txtResults.Name = "txtResults";
			this.txtResults.ReadOnly = true;
			this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtResults.Size = new System.Drawing.Size(472, 216);
			this.txtResults.TabIndex = 9;
			this.txtResults.Text = "";
			this.txtResults.WordWrap = false;
			// 
			// lblResults
			// 
			this.lblResults.Location = new System.Drawing.Point(12, 192);
			this.lblResults.Name = "lblResults";
			this.lblResults.Size = new System.Drawing.Size(56, 12);
			this.lblResults.TabIndex = 10;
			this.lblResults.Text = "Results";
			// 
			// txtFiles
			// 
			this.txtFiles.BackColor = System.Drawing.SystemColors.Window;
			this.txtFiles.Location = new System.Drawing.Point(12, 68);
			this.txtFiles.Name = "txtFiles";
			this.txtFiles.Size = new System.Drawing.Size(180, 20);
			this.txtFiles.TabIndex = 5;
			this.txtFiles.Text = "";
			this.txtFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFiles_KeyDown);
			// 
			// lblDir
			// 
			this.lblDir.Location = new System.Drawing.Point(12, 12);
			this.lblDir.Name = "lblDir";
			this.lblDir.Size = new System.Drawing.Size(60, 12);
			this.lblDir.TabIndex = 0;
			this.lblDir.Text = "Directory";
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(456, 24);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(28, 20);
			this.btnBrowse.TabIndex = 2;
			this.btnBrowse.Text = "...";
			this.btnBrowse.Click += new System.EventHandler(this.btnDir_Click);
			// 
			// txtSearchText
			// 
			this.txtSearchText.BackColor = System.Drawing.SystemColors.Window;
			this.txtSearchText.Location = new System.Drawing.Point(204, 68);
			this.txtSearchText.Name = "txtSearchText";
			this.txtSearchText.Size = new System.Drawing.Size(280, 20);
			this.txtSearchText.TabIndex = 7;
			this.txtSearchText.Text = "";
			this.txtSearchText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchText_KeyDown);
			this.txtSearchText.TextChanged += new System.EventHandler(this.txtSearchText_TextChanged);
			// 
			// lblFiles
			// 
			this.lblFiles.Location = new System.Drawing.Point(12, 56);
			this.lblFiles.Name = "lblFiles";
			this.lblFiles.Size = new System.Drawing.Size(84, 12);
			this.lblFiles.TabIndex = 4;
			this.lblFiles.Text = "Files";
			// 
			// lblSearchText
			// 
			this.lblSearchText.Location = new System.Drawing.Point(204, 56);
			this.lblSearchText.Name = "lblSearchText";
			this.lblSearchText.Size = new System.Drawing.Size(196, 12);
			this.lblSearchText.TabIndex = 6;
			this.lblSearchText.Text = "Search Pattern (Regular Expression)";
			// 
			// btnSearch
			// 
			this.btnSearch.Enabled = false;
			this.btnSearch.Location = new System.Drawing.Point(424, 116);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Size = new System.Drawing.Size(60, 24);
			this.btnSearch.TabIndex = 8;
			this.btnSearch.Text = "Search";
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// txtDir
			// 
			this.txtDir.BackColor = System.Drawing.SystemColors.Window;
			this.txtDir.Location = new System.Drawing.Point(12, 24);
			this.txtDir.Name = "txtDir";
			this.txtDir.Size = new System.Drawing.Size(436, 20);
			this.txtDir.TabIndex = 1;
			this.txtDir.Text = "";
			this.txtDir.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDir_KeyDown);
			this.txtDir.TextChanged += new System.EventHandler(this.txtDir_TextChanged);
			// 
			// ckCountLines
			// 
			this.ckCountLines.Location = new System.Drawing.Point(12, 124);
			this.ckCountLines.Name = "ckCountLines";
			this.ckCountLines.Size = new System.Drawing.Size(84, 16);
			this.ckCountLines.TabIndex = 3;
			this.ckCountLines.Text = "Count Lines";
			// 
			// lblCurFile
			// 
			this.lblCurFile.Location = new System.Drawing.Point(12, 152);
			this.lblCurFile.Name = "lblCurFile";
			this.lblCurFile.Size = new System.Drawing.Size(84, 12);
			this.lblCurFile.TabIndex = 11;
			this.lblCurFile.Text = "Current File";
			// 
			// txtCurFile
			// 
			this.txtCurFile.BackColor = System.Drawing.SystemColors.Info;
			this.txtCurFile.Location = new System.Drawing.Point(12, 164);
			this.txtCurFile.Name = "txtCurFile";
			this.txtCurFile.ReadOnly = true;
			this.txtCurFile.Size = new System.Drawing.Size(472, 20);
			this.txtCurFile.TabIndex = 12;
			this.txtCurFile.Text = "";
			// 
			// ckRecursive
			// 
			this.ckRecursive.Location = new System.Drawing.Point(12, 100);
			this.ckRecursive.Name = "ckRecursive";
			this.ckRecursive.Size = new System.Drawing.Size(84, 16);
			this.ckRecursive.TabIndex = 13;
			this.ckRecursive.Text = "Recursive";
			// 
			// ckLineNumbers
			// 
			this.ckLineNumbers.Location = new System.Drawing.Point(104, 100);
			this.ckLineNumbers.Name = "ckLineNumbers";
			this.ckLineNumbers.Size = new System.Drawing.Size(96, 16);
			this.ckLineNumbers.TabIndex = 14;
			this.ckLineNumbers.Text = "Line Numbers";
			// 
			// ckIgnoreCase
			// 
			this.ckIgnoreCase.Location = new System.Drawing.Point(104, 124);
			this.ckIgnoreCase.Name = "ckIgnoreCase";
			this.ckIgnoreCase.Size = new System.Drawing.Size(96, 16);
			this.ckIgnoreCase.TabIndex = 15;
			this.ckIgnoreCase.Text = "Ignore Case";
			// 
			// ckJustFiles
			// 
			this.ckJustFiles.Location = new System.Drawing.Point(208, 100);
			this.ckJustFiles.Name = "ckJustFiles";
			this.ckJustFiles.Size = new System.Drawing.Size(84, 16);
			this.ckJustFiles.TabIndex = 16;
			this.ckJustFiles.Text = "Just Files";
			this.ckJustFiles.Click += new System.EventHandler(this.ckJustFiles_Click);
			// 
			// FormWinGrep
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(494, 432);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.ckJustFiles,
																		  this.ckIgnoreCase,
																		  this.ckLineNumbers,
																		  this.ckRecursive,
																		  this.lblCurFile,
																		  this.txtCurFile,
																		  this.btnBrowse,
																		  this.lblResults,
																		  this.txtResults,
																		  this.lblSearchText,
																		  this.txtSearchText,
																		  this.lblFiles,
																		  this.txtFiles,
																		  this.btnSearch,
																		  this.ckCountLines,
																		  this.lblDir,
																		  this.txtDir});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "FormWinGrep";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Win Grep";
			this.ResumeLayout(false);
		}

		protected void SearchThread()
		{
			if(m_searchthread == null)
			{
				//Start Searching Thread
				m_searchthread = new Thread(new ThreadStart(Search));
				m_searchthread.IsBackground = true;
				m_searchthread.Start();
				btnSearch.Text = "Stop";
			}
			else //bSearching != null
			{
				//Stop Thread
				m_searchthread.Abort();
				txtCurFile.Text = "";
				txtResults.Text = "User Requested Search Abortion!";
			}
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
			if(e.KeyCode==Keys.Enter && btnSearch.Enabled==true)
			{
				SearchThread();
			}
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

		//Search Function
		protected void Search()
		{
			try
			{
				String strDir = txtDir.Text;
				//Check First if the Selected Directory exists
				if(!Directory.Exists(strDir))
					MessageBox.Show("Directory doesn't exist!", "Win Grep Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				else
				{
					Cursor = Cursors.WaitCursor;
					txtResults.Text = "";
					//Initialize the Flags
					bool bRecursive = ckRecursive.Checked;
					bool bIgnoreCase = ckIgnoreCase.Checked;
					bool bJustFiles = ckJustFiles.Checked;
					bool bLineNumbers;
					if(bJustFiles == true)
						bLineNumbers = false;
					else
						bLineNumbers = ckLineNumbers.Checked;
					bool bCountLines;
					if(bJustFiles == true)
						bCountLines = false;
					else
						bCountLines = ckCountLines.Checked;
					//File Extension
					String strExt = txtFiles.Text;
					//First empty the list
					m_arrFiles.Clear();
					//Create recursively a list with all the files complying with the criteria
					String[] astrExt = strExt.Split(new Char[] {','});
					for(int i=0; i<astrExt.Length; i++)
					{
						//Eliminate white spaces
						astrExt[i] = astrExt[i].Trim();
						GetFiles(strDir, astrExt[i], bRecursive);
					}
					//Now all the Files are in the ArrayList, open each one
					//iteratively and look for the search string
					String strSearch = txtSearchText.Text;
					String strResults = "";
					String strLine;
					int iLine, iCount;
					bool bEmpty = true;
					IEnumerator enm = m_arrFiles.GetEnumerator();
					while(enm.MoveNext())
					{
						try
						{
							txtCurFile.Text = (string)enm.Current;
							StreamReader sr = File.OpenText((string)enm.Current);
							iLine = 0;
							iCount = 0;
							bool bFirst = true;
							while((strLine = sr.ReadLine())!=null)
							{
								iLine++;
								//Using Regular Expressions as a real Grep
								Match mtch;
								if(bIgnoreCase == true)
									mtch = Regex.Match(strLine, strSearch, RegexOptions.IgnoreCase);
								else
									mtch = Regex.Match(strLine, strSearch);
								if(mtch.Success == true)
								{
									bEmpty = false;
									iCount++;
									if(bFirst == true)
									{
										if(bJustFiles == true)
										{
											strResults += (string)enm.Current + "\r\n";
											break;
										}
										else
											strResults += (string)enm.Current + ":\r\n";
										bFirst = false;
									}
									//Add the Line to Results string
									if(bLineNumbers == true)
										strResults += "  " + iLine + ": " + strLine + "\r\n";
									else
										strResults += "  " + strLine + "\r\n";
								}	
							}
							sr.Close();
							if(bFirst == false)
							{
								if(bCountLines == true)
									strResults += "  " + iCount + " Lines Matched\r\n";
								strResults += "\r\n";
							}
						}
						catch(SecurityException)
						{
							strResults += "\r\n" + (string)enm.Current + ": Security Exception\r\n\r\n";	
						}
						catch(FileNotFoundException)
						{
							strResults += "\r\n" + (string)enm.Current + ": File Not Found Exception\r\n";
						}
					}
					txtCurFile.Text = "";
					if(bEmpty == true)
						txtResults.Text = "No matches found!";
					else
						txtResults.Text = strResults;
				}
			}
			finally
			{
				m_searchthread = null;
				if(Cursor == Cursors.WaitCursor)
					Cursor = Cursors.Arrow;
				btnSearch.Text = "Start";
			}
		}

		protected void btnSearch_Click (object sender, System.EventArgs e)
		{
			SearchThread();
		}

		protected void VerifySearchBtn()
		{
			if(txtDir.Text != "" && txtSearchText.Text != "")
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
			OpenFileDialog fdlg = new OpenFileDialog();
			fdlg.Title = "Select a file";
			fdlg.InitialDirectory = Directory.GetCurrentDirectory();
			fdlg.Filter = "All files (*.*)|*.*";
			if(fdlg.ShowDialog() == DialogResult.OK)
			{
				String strFile = fdlg.FileName;
				//File Extension
				String strExt;
				//Get the Directory and file extension
				txtDir.Text = Path.GetDirectoryName(strFile);
				strExt = Path.GetExtension(strFile);
				txtFiles.Text = "*" + strExt;
			}
		}

		private void ckJustFiles_Click(object sender, System.EventArgs e)
		{
			if(ckJustFiles.Checked == true)
			{
				ckLineNumbers.Enabled = false;
				ckCountLines.Enabled = false;
			}
			else
			{
				ckLineNumbers.Enabled = true;
				ckCountLines.Enabled = true;
			}
		}

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args) 
        {
            Application.Run(new FormWinGrep());
        }
    }
}

