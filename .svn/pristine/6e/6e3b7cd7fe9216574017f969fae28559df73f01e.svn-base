using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;

namespace VaultSearcher
{
    /// <summary>
    /// Summary description for VaultSearcher
    /// </summary>
    [WebService(Namespace = "http://libralex.co.za/VaultSearcher")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class VaultSearcher : System.Web.Services.WebService
    {

        ArrayList m_arrFiles = new ArrayList();

        public class SearchResultInfo
        {
            public SearchResultInfo(FileInfo myFile, string term)
            {
                SearchTerm = term;
                FileName = myFile.Name;
                FileSize = myFile.Length / 1048576; // Size in MB
                CreationTime = myFile.CreationTime;
                Selected = false;
            }

            public SearchResultInfo()
            {
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
            public double FileSize
            {
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

            //Search Function
        [WebMethod]
        public List<SearchResultInfo> Search(string searchText, string searchDir, bool recursive, bool ignoreCase, bool returnMany, string zipFileExt)
        {
            try
            {
                //String strDir = txtDir.Text;
                string strDir = searchDir;
                //Check First if the Selected Directory exists
                if (!Directory.Exists(strDir))
                    return null;
                else
                {
                    //Initialize the Flags
                    bool bRecursive = recursive;
                    bool bIgnoreCase = ignoreCase;
                    //File Extension
                    String strExt = "*.txt";//txtFiles.Text;
                    //First empty the list
                    m_arrFiles.Clear();
                    //Create recursively a list with all the files complying with the criteria
                    String[] astrExt = strExt.Split(new Char[] { ',' });
                    for (int i = 0; i < astrExt.Length; i++)
                    {
                        //Eliminate white spaces
                        astrExt[i] = astrExt[i].Trim();
                        GetFiles(strDir, astrExt[i], bRecursive);
                    }
                    //Now all the Files are in the ArrayList, open each one
                    //iteratively and look for the search string

                    string[] strSearch = searchText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    List<SearchResultInfo> finalResults = new List<SearchResultInfo>();

                    //ArrayList strFinalResults = new ArrayList();

                    ArrayList strResults = new ArrayList();

                    String strLine;
                    int iLine, iCount;
                    bool bEmpty = true;
                    for (int i = 0; i < strSearch.Length; i++)
                    {
                        // Create new searchResult object
                        //SearchResultInfo searchResult = new SearchResultInfo();

                        IEnumerator enm = m_arrFiles.GetEnumerator();
                        while (enm.MoveNext())
                        {
                            try
                            {
                                StreamReader sr = File.OpenText((string)enm.Current);
                                iLine = 0;
                                iCount = 0;
                                bool bFirst = true;
                                while ((strLine = sr.ReadLine()) != null)
                                {
                                    iLine++;
                                    //Using Regular Expressions as a real Grep
                                    Match mtch;
                                    if (bIgnoreCase == true)
                                        mtch = Regex.Match(strLine, strSearch[i], RegexOptions.IgnoreCase);
                                    else
                                        mtch = Regex.Match(strLine, strSearch[i]);
                                    if (mtch.Success == true)
                                    {
                                        bEmpty = false;
                                        iCount++;
                                        if (bFirst == true)
                                        {
                                            //if (bJustFiles == true)
                                            //{
                                            //searchResult.SearchTerm = strSearch[i];
                                            strResults.Add((string)enm.Current);
                                            //break;
                                            //}
                                            //else
                                            //    strResults.Add((string)enm.Current);
                                            bFirst = false;
                                        }
                                    }
                                }
                                sr.Close();
                            }

                            catch (SecurityException)
                            {
                                //strResults += "\r\n" + (string)enm.Current + ": Security Exception\r\n\r\n";
                            }
                            catch (FileNotFoundException)
                            {
                                //strResults += "\r\n" + (string)enm.Current + ": File Not Found Exception\r\n";
                            }
                        }

                        if (bEmpty == true)
                            //SetText("No matches found");
                            return null;
                        else
                        {
                            //ArrayList strFinalResults = new ArrayList();

                            bool hasSearchItemFound = false;
                            strResults.Reverse();

                            // Search to see if the backup file exists. If not, remove from the list
                            foreach (string s in strResults)
                            {
                                if (hasSearchItemFound)
                                    break;
                                else
                                {
                                    string backupFileName = s.Replace(".txt", zipFileExt);
                                    if (!File.Exists(backupFileName))
                                        continue;
                                    else
                                    {
                                        FileInfo zipFile = new FileInfo(backupFileName);
                                        SearchResultInfo searchResult = new SearchResultInfo(zipFile, strSearch[i]);

                                        // hasSearchItem will break the loop if an item has been found - this is only needed if we want to return 1 result
                                        // if a list of files needs to be returned for the user to see, then do not set hasSearchItemFound to true to break the loop
                                        if (!returnMany)
                                        {
                                            hasSearchItemFound = true;
                                            searchResult.Selected = true; // if we only want 1 item to be returned, then we are going to download automatically
                                        }

                                        finalResults.Add(searchResult);
                                    }
                                }
                            }
                        }
                    }
                    //return strFinalResults;
                    //finalResults = finalResults.OrderBy(o => o.SearchTerm).ThenBy(o => o.CreationTime).ToList();
                    return finalResults.OrderBy(o => o.SearchTerm).ThenBy(o => o.CreationTime).ToList();
                }
            }
            finally
            {
                //m_searchthread = null;
                //if (Cursor == Cursors.WaitCursor)
                //    Cursor = Cursors.Arrow;
            }
        }

        //Build the list of Files
        protected void GetFiles(String strDir, String strExt, bool bRecursive)
        {
            //search pattern can include the wild characters '*' and '?'
            string[] fileList = Directory.GetFiles(strDir, strExt);
            for (int i = 0; i < fileList.Length; i++)
            {
                if (File.Exists(fileList[i]))
                    m_arrFiles.Add(fileList[i]);
            }
            if (bRecursive == true)
            {
                //Get recursively from subdirectories
                string[] dirList = Directory.GetDirectories(strDir);
                for (int i = 0; i < dirList.Length; i++)
                {
                    GetFiles(dirList[i], strExt, true);
                }
            }
        }

    }
}
