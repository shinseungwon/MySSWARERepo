using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogReader
{
    public partial class Form1 : Form
    {
        private readonly List<LogItem> Items = new List<LogItem>();

        public Form1()
        {
            InitializeComponent();

            string currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);            

            if (IsSswareDirectory(currentPath))
            {
                DirectoryTextBox.Text = currentPath;                
                DirectoryInfo directory = new DirectoryInfo(currentPath);
                Items.Clear();
                GetItems(directory);
                foreach (LogItem l in Items)
                {
                    string[] item = { l.Title, l.Date.ToString("yyyy-MM-dd hh:mm:ss"), l.Directory, l.Body };
                    ListViewItem listViewItem = new ListViewItem(item);
                    listViewItem.ToolTipText = l.Directory;
                    TitleListView.Items.Add(listViewItem);
                }
            }
        }

        private void ReloadButton_Click(object sender, EventArgs e)
        {
            if (IsSswareDirectory(DirectoryTextBox.Text))
            {
                SearchTextBox.Text = "";
                DirectoryTextBox.Text = DirectoryTextBox.Text;
                DirectoryInfo directory = new DirectoryInfo(DirectoryTextBox.Text);
                Items.Clear();
                GetItems(directory);
                foreach (LogItem l in Items)
                {
                    string[] item = { l.Title, l.Date.ToString("yyyy-MM-dd hh:mm:ss"), l.Directory, l.Body };
                    ListViewItem listViewItem = new ListViewItem(item);
                    listViewItem.ToolTipText = l.Directory;
                    TitleListView.Items.Add(listViewItem);
                }
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            string queryString = SearchTextBox.Text;
            string[] queries = queryString.Split('/');
            List<string> include = new List<string>();
            List<string> exclude = new List<string>();

            foreach (string query in queries)
            {
                if (query.StartsWith("!"))
                {
                    exclude.Add(query.Substring(1));
                }
                else
                {
                    include.Add(query);
                }
            }

            TitleListView.Items.Clear();

            foreach (LogItem l in Items)
            {
                if (FromDateTimePicker.Checked)
                {
                    DateTime dateTime = Convert.ToDateTime(l.Date);
                    if (dateTime < FromDateTimePicker.Value)
                    {
                        continue;
                    }
                }

                if (ToDateTimePicker.Checked)
                {
                    DateTime dateTime = Convert.ToDateTime(l.Date);
                    if (dateTime > ToDateTimePicker.Value)
                    {
                        continue;
                    }
                }

                if (exclude.Count > 0)
                {
                    bool isExclude = false;

                    foreach (string s in exclude)
                    {
                        if (l.Body.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) >= 0
                            || l.Title.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            isExclude = true;
                        }
                    }

                    if (isExclude)
                    {
                        continue;
                    }
                }

                if (include.Count > 0)
                {
                    int includeCount = 0;

                    foreach (string s in include)
                    {
                        if (l.Body.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) >= 0
                            || l.Title.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            includeCount++;
                        }
                    }

                    if (includeCount == 0)
                    {
                        continue;
                    }
                }

                string[] item = { l.Title, l.Date.ToString("yyyy-MM-dd hh:mm:ss"), l.Directory, l.Body };
                ListViewItem listViewItem = new ListViewItem(item);
                listViewItem.ToolTipText = l.Directory;
                TitleListView.Items.Add(listViewItem);
            }

            if (TitleListView.Items.Count > 0)
            {
                TitleListView.Items[0].Selected = true;
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    DirectoryTextBox.Text = dlg.SelectedPath;
                    SetDirectory(dlg.SelectedPath);
                }
            }
        }

        private bool IsSswareDirectory(string path)
        {
            bool isSswareDirectory = false;

            if (Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                FileInfo[] fileInfo = directoryInfo.GetFiles();

                foreach (FileInfo f in fileInfo)
                {
                    if (f.Name == ".sswarelog")
                    {
                        isSswareDirectory = true;
                        break;
                    }
                }
            }            

            return isSswareDirectory;
        }

        private void SetDirectory(string path)
        {
            if (IsSswareDirectory(path))
            {                
                DirectoryInfo directory = new DirectoryInfo(DirectoryTextBox.Text);
                Items.Clear();
                GetItems(directory);
                foreach (LogItem l in Items)
                {
                    string[] item = { l.Title, l.Date.ToString("yyyy-MM-dd hh:mm:ss"), l.Directory, l.Body };
                    ListViewItem listViewItem = new ListViewItem(item);
                    listViewItem.ToolTipText = l.Directory;
                    TitleListView.Items.Add(listViewItem);
                }
            }
            else
            {
                MessageBox.Show("Not a Ssware log directory");
            }
        }

        private void GetItems(DirectoryInfo directory)
        {
            FileInfo[] files = directory.GetFiles("*.txt");

            foreach (FileInfo f in files)
            {
                StringBuilder sb = new StringBuilder();

                using (StreamReader sr = f.OpenText())
                {
                    string text = "";

                    while ((text = sr.ReadLine()) != null)
                    {
                        sb.Append(text + "\n");
                    }
                }

                string str = sb.ToString() + "[]";
                string[] lines = str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                LogItem logItem = null;
                foreach (string s in lines)
                {
                    if (s.StartsWith("[") && s.EndsWith("]"))
                    {
                        if (logItem != null)
                        {
                            Items.Add(logItem);
                        }

                        if (s.Length > 2)
                        {
                            logItem = new LogItem();
                            logItem.Directory = f.FullName;
                            logItem.Date = Convert.ToDateTime(s.Substring(1, s.Length - 2));
                        }
                    }
                    else if (s.StartsWith("<") && s.EndsWith(">"))
                    {
                        logItem.Title = s.Substring(1, s.Length - 2);
                        logItem.Body = "";
                    }
                    else
                    {
                        logItem.Body += s;
                    }
                }
            }

            DirectoryInfo[] directories = directory.GetDirectories();

            foreach (DirectoryInfo d in directories)
            {
                GetItems(d);
            }
        }

        private void TitleLIstView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView listView = (ListView)sender;
            if (listView.SelectedItems.Count > 0)
            {
                ListViewItem listViewItem = listView.SelectedItems[0];
                BodyTextBox.Text = listViewItem.SubItems[3].Text;
            }
        }

        private void TitleListView_DoubleClick(object sender, EventArgs e)
        {
            ListView listView = (ListView)sender;
            if (listView.SelectedItems.Count > 0)
            {
                ListViewItem listViewItem = listView.SelectedItems[0];
                Process.Start(listViewItem.SubItems[2].Text);
            }
        }

        private void TitleListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == 1)
            {
                if (TitleListView.Sorting == SortOrder.Ascending)
                {
                    TitleListView.Sorting = SortOrder.Descending;
                }
                else
                {
                    TitleListView.Sorting = SortOrder.Ascending;
                }
            }

            TitleListView.ListViewItemSorter
                = new ListViewComparer(e.Column, TitleListView.Sorting);
            TitleListView.Sort();
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (ModifierKeys.HasFlag(Keys.Shift))
                {
                    e.SuppressKeyPress = true;
                    SearchButton_Click(sender, e);
                }
            }
        }

        private class LogItem
        {
            public string Directory;
            public string Title;
            public string Body;
            public DateTime Date;
        }

        class ListViewComparer : IComparer
        {
            private int col;
            private SortOrder order;

            public ListViewComparer()
            {
                col = 0; order = SortOrder.Ascending;
            }

            public ListViewComparer(int column, SortOrder order)
            {
                col = column; this.order = order;
            }

            public int Compare(object x, object y)
            {
                int returnVal = string.Compare(((ListViewItem)x).SubItems[col].Text
                    , ((ListViewItem)y).SubItems[col].Text);

                return returnVal *= order == SortOrder.Descending ? -1 : 1;
            }
        }


    }
}
