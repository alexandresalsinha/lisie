using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EXControls;

namespace SpiroStockManagement
{
    public partial class tests : Form
    {
       

        public tests()
        {
            InitializeComponent();
            autoCompleteMine1.Initialize(GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAutocompleteTextboxDate());
        }

    }
}
