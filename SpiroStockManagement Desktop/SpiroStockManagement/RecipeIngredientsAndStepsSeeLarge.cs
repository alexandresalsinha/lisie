using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.IO;

namespace SpiroStockManagement
{
    public partial class RecipeIngredientsAndStepsSeeLarge : Form
    {
        public RecipeIngredientsAndStepsSeeLarge()
        {
            InitializeComponent();
        }
        Dictionary<int, float> InitialIngredientsAmountValues = new Dictionary<int, float>();
        public void Initialize(SpiroStockManagmentDatabaseClass.Objects.Recipe recipe)
        {
            InitializeDataGridViewColumns();

            this.Text = "Receita : " + recipe.Name;

            int _rowIndex = 0;
            dataGridView_Ingredients.Rows.Clear();
            foreach (SpiroStockManagmentDatabaseClass.Objects.RecipeIngredient _ingredient in recipe.IngredientList)
            {
                float _tempFloat = 0;
                InitialIngredientsAmountValues.Add(_rowIndex, (float.TryParse(_ingredient.Amount.ToString(), out _tempFloat) ? _tempFloat : 0));

                dataGridView_Ingredients.Rows.Insert(_rowIndex++, new string[] { _ingredient.Name, _ingredient.Amount.ToString(), _ingredient.Units, _ingredient.Information });
            }
            ////Steps
            _rowIndex = 0;
            dataGridView_Steps.Rows.Clear();
            foreach (SpiroStockManagmentDatabaseClass.Objects.Step _step in recipe.Directions)
            {
                dataGridView_Steps.Rows.Insert(_rowIndex++, new string[] { _step.Value });
            }
        }

        void InitializeDataGridViewColumns()
        {
            ////ingredients
            dataGridView_Ingredients.Columns.Clear();
            DataGridViewComboBoxColumn _dgvCbC = new DataGridViewComboBoxColumn();
            //_dgvCbC.AutoComplete = true;
            _dgvCbC.HeaderText = "Ingrediente";
            _dgvCbC.ReadOnly = false;
            DataGridViewTextBoxColumn _DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            _DataGridViewTextBoxColumn.HeaderText = "Ingrediente";

            List<string> _IngredientList = new List<string>();
            foreach (XElement _XIngredient in GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllIngredients())
            {
                SpiroStockManagmentDatabaseClass.Objects.Ingredient _currentIngredient = new SpiroStockManagmentDatabaseClass.Objects.Ingredient();
                _currentIngredient = (SpiroStockManagmentDatabaseClass.Objects.Ingredient)SpiroStockManagmentDatabaseClass.XmlSerializerExtension.DeSerializerToObject(_XIngredient, _currentIngredient);
                _IngredientList.Add(_currentIngredient.Name);
            }
            dataGridView_Ingredients.Columns.Add(_DataGridViewTextBoxColumn);


            dataGridView_Ingredients.Columns.Add("Quantidade", "Quantidade");
            dataGridView_Ingredients.Columns.Add("Unidade", "Unidade");
            dataGridView_Ingredients.Columns.Add("Informação", "Informação");

            //Steps
            dataGridView_Steps.Columns.Clear();
            dataGridView_Steps.Columns.Add("Passos", "Passos");
            dataGridView_Steps.Columns[0].Width = 900;
        }

        private void numericUpDown_QuantityProportion_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_QuantityProportion.Value > 1 && numericUpDown_QuantityProportion.Value < 2)
            {
                numericUpDown_QuantityProportion.Increment = 1;
                numericUpDown_QuantityProportion.Value = 2;
            }
            if (numericUpDown_QuantityProportion.Value == 1)
            {
                numericUpDown_QuantityProportion.Increment = decimal.Parse("0,1");
            }
            int _rowIndex = 0;
            foreach (DataGridViewRow _row in dataGridView_Ingredients.Rows)
            {
                if (_row.Cells[0].Value != null)
                {

                    float _currentAmountDecimal = (InitialIngredientsAmountValues.ContainsKey(_rowIndex)) ? InitialIngredientsAmountValues[_rowIndex] : 0;

                    if (_currentAmountDecimal > 0)
                    {
                        _row.Cells[1].Value = float.Parse(numericUpDown_QuantityProportion.Value.ToString()) * _currentAmountDecimal;
                    }
                }
                _rowIndex++;
            }
        }
    }
}
