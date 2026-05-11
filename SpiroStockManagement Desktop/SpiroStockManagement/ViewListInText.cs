using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Printing;

namespace SpiroStockManagement
{
    public partial class ViewListInText : Form
    {
        public ViewListInText()
        {
            InitializeComponent();
        }

        public void Initialize(List<string> text)
        {
            textBox_Text.Text = "";
            foreach (string _string in text)
            {
                textBox_Text.Text += _string + "\r\n";
            }
        }

        private string strToPrint = "";
        private void button1_Click(object sender, EventArgs e)
        {
            strToPrint = textBox_Text.Text.ToString();
            System.Drawing.Printing.PrintDocument f = new System.Drawing.Printing.PrintDocument();
            PrintDialog theDialog = new PrintDialog();
            System.Drawing.Printing.PrintDocument thePrintDocument = new System.Drawing.Printing.PrintDocument();
            theDialog.Document = thePrintDocument;
            theDialog.ShowDialog();
            
            thePrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(thePrintDocument_PrintPage);
            thePrintDocument.Print();
        }

        void thePrintDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int linesPerPage = 0;
            int charsOnPage = 0;

            e.Graphics.MeasureString(this.strToPrint, this.Font, e.MarginBounds.Size, StringFormat.GenericTypographic, out charsOnPage, out linesPerPage);
            e.Graphics.DrawString(this.strToPrint, this.Font, Brushes.Black, e.MarginBounds, StringFormat.GenericTypographic);
            this.strToPrint = this.strToPrint.Substring(charsOnPage);
            e.HasMorePages = (this.strToPrint.Length > 0);

        }
    }
}
