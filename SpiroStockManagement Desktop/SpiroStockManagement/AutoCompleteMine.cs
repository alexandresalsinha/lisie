using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpiroStockManagement
{
    public partial class AutoCompleteMine : UserControl
    {
        public event EventHandler ProductedSelectedChanged;
        public string TextInputted
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public bool AutoCompleteControlVisibility
        {
            get { return listBox1.Visible; }
        }

        int _SelectedProductId = -1;
        public int SelectedProductId
        {
            get 
            {
                return _SelectedProductId;
            }
            set
            {
                _SelectedProductId = value;
            }
        }

        KeyedAutoCompleteStringCollection acsc = new KeyedAutoCompleteStringCollection();
        public AutoCompleteMine()
        {
            InitializeComponent();

            
        }
        public void Initialize(List<SpiroStockManagmentDatabaseClass.Objects.AutoCompleteProductData> autocompletedata)
        {
            textBox1.AutoCompleteCustomSource = acsc;
            textBox1.AutoCompleteMode = AutoCompleteMode.None;
            textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            foreach (SpiroStockManagmentDatabaseClass.Objects.AutoCompleteProductData _item in autocompletedata)
            {
                acsc.Add(_item.ProductName, _item.ProductId);
            }
        }
        void textBox1_TextChanged(object sender, System.EventArgs e)
        {
            listBox1.Items.Clear();
            if (textBox1.Text.Length == 0)
            {
                hideResults();
                return;
            }

            

            foreach (String s in textBox1.AutoCompleteCustomSource)
            {
                if (s.ToLower().Contains(textBox1.Text.ToLower()))
                {
                    //Console.WriteLine("Found text in: " + s);
                    listBox1.Items.Add(s);
                }
            }
            if (listBox1.Items.Count > 0)
            {
                listBox1.Visible = true;
                //listBox1.Location = new Point(this.Location.X, this.Location.Y + this.Height);
            }
            else
            {
                listBox1.Visible = false;
            }

            //bug fix, when a barcode is entered the listbox with the results appears,´it should not
            if (listBox1.Items.Count == 1 && listBox1.Items[0].ToString().ToLower() == textBox1.Text.ToLower())
            {
                listBox1.Visible = false;
            }
        }

        void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //int _selectedId = acsc.Lookup(listBox1.Items[listBox1.SelectedIndex].ToString());
            //textBox1.Text = listBox1.Items[listBox1.SelectedIndex].ToString();
            //hideResults();
        }


        void hideResults()
        {
            listBox1.Visible = false;
        }

        private void listBox1_Leave(object sender, EventArgs e)
        {
            hideResults();
        }

        private void AutoCompleteMine_Load(object sender, EventArgs e)
        {
            //this.Parent.Controls.Add(listBox1);
            this.ParentForm.Controls.Add(listBox1);
            this.textBox1.Width = this.Width;
            this.listBox1.Width = this.Width;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
           if (e.KeyCode == Keys.Down)
            {
                if (listBox1.Visible == false)
                {
                    listBox1.Visible = true;
                }
                
                if (listBox1.SelectedIndex + 1 <= listBox1.Items.Count - 1 && listBox1.SelectedIndex > -1)
                {
                    listBox1.SelectedIndex++;
                }
                if (listBox1.SelectedIndex == -1)
                {
                    listBox1.SelectedIndex = 0;
                }
            }
            if (e.KeyCode == Keys.Up)
            {
                if (listBox1.SelectedIndex == -1)
                {
                    listBox1.SelectedIndex = 0;
                }
                if ((listBox1.SelectedIndex - 1 <= listBox1.Items.Count - 1) && listBox1.SelectedIndex > 0)
                {
                    listBox1.SelectedIndex--;
                }
            }
            if (e.KeyCode == Keys.Enter)
            {
                if (listBox1.SelectedIndex > -1)
                {
                    int _selectedId = acsc.Lookup(listBox1.Items[listBox1.SelectedIndex].ToString());
                    textBox1.Text = listBox1.Items[listBox1.SelectedIndex].ToString();
                    textBox1.SelectionStart = 0;
                    textBox1.SelectionLength = textBox1.Text.Length;
                    SelectedProductId = _selectedId;
                    //GlobalVariables.EnterPressedIsToAddProduct = true;
                    if (ProductedSelectedChanged != null) ProductedSelectedChanged(this, new EventArgs()); 
                }
                
                hideResults();
            }
            if (e.KeyCode == Keys.Escape)
            {
                hideResults();
            }
        }

        private void AutoCompleteMine_Resize(object sender, EventArgs e)
        {
            this.textBox1.Width = this.Width;
            this.listBox1.Width = this.Width;
        }

        private void listBox1_VisibleChanged(object sender, EventArgs e)
        {
            if (listBox1.Visible)
            {
                Point _pT =  GlobalProcedures.GetControlLocationInRelationToForm(this);
                listBox1.Location = new Point(_pT.X, _pT.Y + this.Height);
                listBox1.BringToFront();
            }
        }

        //bool isToProccessKey = true;
        //private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    if (System.Text.RegularExpressions.Regex.IsMatch(((char)e.KeyValue).ToString(), @"\d"))
        //    {
        //        isToProccessKey = false;
        //    }
        //}

        //private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (isToProccessKey == false)
        //    {
        //        isToProccessKey = true;
        //        e.Handled = true;
        //    }
        //}

        private void listBox1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                int _selectedId = acsc.Lookup(listBox1.Items[listBox1.SelectedIndex].ToString());
                textBox1.Text = listBox1.Items[listBox1.SelectedIndex].ToString();
                textBox1.SelectionStart = 0;
                textBox1.SelectionLength = textBox1.Text.Length;
                SelectedProductId = _selectedId;
                if (ProductedSelectedChanged != null) ProductedSelectedChanged(this, new EventArgs());
            }

            hideResults();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            hideResults();
        }

    }

    class KeyedAutoCompleteStringCollection : AutoCompleteStringCollection
    {

        private readonly Dictionary<string, int> keyedValues =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public void Add(string value, int key)
        {
            base.Add(value);
            keyedValues.Add(value, key); // intentionally backwards
        }

        public int Lookup(string value)
        {
            int key;
            if (keyedValues.TryGetValue(value, out key))
            {
                return key;
            }
            else
            {
                return -1;
            }
        }

    }
}
