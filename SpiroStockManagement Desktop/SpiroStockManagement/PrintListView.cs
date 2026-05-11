using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpiroStockManagement
{
    public partial class PrintListView : Form
    {
        public PrintListView()
        {
            InitializeComponent();
            listView_toPrint.OwnerDraw = true;
        }

        public void Initialize(ListView listView)
        {
            listView_toPrint.Items.Clear();
            listView_toPrint.BeginUpdate();
            listView_toPrint.Columns.Clear();
            listView_toPrint.Groups.Clear();
            
            Dictionary<string, int> _DictionaryGroupsIndex = new Dictionary<string, int>();
            int _categoriesCountIndex = 0;

            //groups
            foreach (ListViewGroup _ListViewGroup in listView.Groups)
            {
                listView_toPrint.Groups.Add(new ListViewGroup { Header = _ListViewGroup.Header, HeaderAlignment = _ListViewGroup.HeaderAlignment });
                _DictionaryGroupsIndex.Add(_ListViewGroup.Header, _categoriesCountIndex++);
            }
            //columns
            foreach (ColumnHeader _ColumnHeader in listView.Columns)
            {
                listView_toPrint.Columns.Add(new ColumnHeader { Text = "  " + _ColumnHeader.Text, Width = _ColumnHeader.Width });
            }

            //items
            foreach (ListViewItem _ListViewItem in listView.Items)
            {
                ListViewItem _newListViewItem = new ListViewItem();
                _newListViewItem.Text = _ListViewItem.Text;
                int _temp = 0;
                foreach (System.Windows.Forms.ListViewItem.ListViewSubItem _subItem in _ListViewItem.SubItems)
                {
                    if (_temp != 0)
                    {
                        _newListViewItem.SubItems.Add(_subItem.Text);
                    }
                    else
                        ++_temp;
                }
                if (_ListViewItem.Group != null)
                {
                    if (_DictionaryGroupsIndex.ContainsKey(_ListViewItem.Group.Header)) // True
                        _newListViewItem.Group = listView_toPrint.Groups[_DictionaryGroupsIndex[_ListViewItem.Group.Header]]; 
                }
                _newListViewItem.Checked = true;
                listView_toPrint.Items.Add(_newListViewItem);
            }
            listView_toPrint.EndUpdate();
            listView_toPrint.ShowGroups = listView.ShowGroups;
        }

        private void button_Print_Click(object sender, EventArgs e)
        {
            //remove the unchecked items
            foreach (ListViewItem _ListViewItem in listView_toPrint.Items)
            {
                if (_ListViewItem.Checked == false)
                {
                    listView_toPrint.Items.Remove(_ListViewItem);
                }
            }
            //remove unused groups
            for (int i = 0; i < listView_toPrint.Groups.Count; i++)
            {
                if (listView_toPrint.Groups[i].Items.Count == 0)
                 {
                   listView_toPrint.Groups.RemoveAt(i--);
                }
            }
            //remove unused columns
            int _deletedColumns = 0;
            foreach (KeyValuePair<int, bool> _keyValue in SelectedColumnCheckboxes)
            {
                if (_keyValue.Value == false)
                {
                    listView_toPrint.Columns.RemoveAt(_keyValue.Key - _deletedColumns);
                    foreach (ListViewItem _ListViewItem in listView_toPrint.Items)
                    {
                        _ListViewItem.SubItems.RemoveAt(_keyValue.Key - _deletedColumns);
                    }
                    //listView_toPrint.Columns.RemoveAt(_keyValue.Key);
                    _deletedColumns++;
                }
            }
            //for (int i = 0; i < listView_toPrint.Columns.Count; i++)
            //{
            //    if (SelectedColumnCheckboxes[listView_toPrint.Columns[i].Index] == false)
            //    {
            //        listView_toPrint.Columns.RemoveAt(i--);
            //    }
            //}

            //update checkbox dictionary
            SelectedColumnCheckboxes.Clear();
            for (int i = 0; i < listView_toPrint.Columns.Count; i++)
            {
                SelectedColumnCheckboxes.Add(i, true);
            }

            listViewPrinter1.PrintPreview();
        }

        bool ColumnHeaderAlreadyDrawned = false;
        Dictionary<int, bool> SelectedColumnCheckboxes = new Dictionary<int, bool>();
        private void listView_toPrint_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            CheckBox cck = new CheckBox
            {
                Text = "",
                Visible = true,
                Tag = e.ColumnIndex,
                Checked = true
            };
            if (!ColumnHeaderAlreadyDrawned)
            {
                if(!SelectedColumnCheckboxes.ContainsKey(e.ColumnIndex))
                    SelectedColumnCheckboxes.Add(e.ColumnIndex, true);
            }
            else
            {
                cck.Checked = SelectedColumnCheckboxes[e.ColumnIndex];
            }

            cck.CheckedChanged += new EventHandler(cck_CheckedChanged);
            listView_toPrint.SuspendLayout();
            e.DrawBackground();
            cck.BackColor = e.BackColor;
            cck.UseVisualStyleBackColor = true;
            cck.SetBounds(e.Bounds.X, e.Bounds.Y, cck.GetPreferredSize(new Size(e.Bounds.Width, e.Bounds.Height)).Width, cck.GetPreferredSize(new Size(e.Bounds.Width, e.Bounds.Height)).Width);
            cck.Size = new Size(cck.GetPreferredSize(new Size(e.Bounds.Width - 1, e.Bounds.Height)).Width + 1, e.Bounds.Height);
            cck.Location = new Point(e.Bounds.Location.X + 1, 0);
            //Padding myPadding = new Padding();
            //myPadding.All = 3;

            //cck.Margin = myPadding;
            listView_toPrint.Controls.Add(cck);
            cck.Show();
            cck.BringToFront();
            e.DrawText(TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding);
            listView_toPrint.ResumeLayout(true);
            if (e.ColumnIndex == listView_toPrint.Columns.Count - 1) ColumnHeaderAlreadyDrawned = true;
        }

        void cck_CheckedChanged(object sender, EventArgs e)
        {
            SelectedColumnCheckboxes[(int)(sender as CheckBox).Tag] = (sender as CheckBox).Checked;
            if ((int)(sender as CheckBox).Tag == 0)
            {
                foreach (ListViewItem _item in listView_toPrint.Items)
                {
                    _item.Checked = (sender as CheckBox).Checked;
                }
            }
        }

        private void listView_toPrint_DrawItem(object sender, DrawListViewItemEventArgs e)
        {

            e.DrawDefault = true;
        }

        private void listView_toPrint_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }
    }
}
