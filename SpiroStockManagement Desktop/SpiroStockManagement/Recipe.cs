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
    public partial class Recipe : Form
    {
        string PhotoImageExtension = "";
        public Recipe()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            //categories
            comboBox_Category.Items.Clear();
            foreach (string _category in GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetRecipeCategoriesDistinct())
            {
                comboBox_Category.Items.Add(_category);
            }
            //cuisine
            comboBox_Cuisine.Items.Clear();
            foreach (string _category in GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetRecipeCuisineDisctinct())
            {
                comboBox_Cuisine.Items.Add(_category);
            }

            InitializeDataGridViewColumns();
        }
        List<string> IngredientsList = new List<string>();
        SpiroStockManagmentDatabaseClass.Objects.Recipe CurrentRecipe = null;
        public void Initialize(SpiroStockManagmentDatabaseClass.Objects.Recipe recipe)
        {
            button_Save.Text = "Gravar";
            CurrentRecipe = recipe;

            textBox_Name.Text = recipe.Name;
            comboBox_Category.Text = recipe.Category;
            comboBox_Cuisine.Text = recipe.Cuisine;
            numericUpDown_TimePreparing.Value = decimal.Parse(recipe.TimePreparing.ToString());
            numericUpDown_TimeCooking.Value = decimal.Parse(recipe.TimeCooking.ToString());
            numericUpDown_TimeTotal.Value = decimal.Parse(recipe.TimeReady.ToString());
            textBox_Description.Text = recipe.Description;
            textBox_Tags.Text = recipe.Tags;
            textBox_Yield.Text = recipe.Yield;
            textBox_Commentary.Text = recipe.Commentary;
            starRatingControl_Rating.SelectedStar = recipe.Rating;

            starRatingControl_Rating.PerformLayout();
            starRatingControl_Rating.Refresh();
            //Photo
            if (recipe.Photo != null && recipe.Photo != string.Empty)
            {
                string _pathToLoadImage = GlobalVariables.RecipeImagesPath + recipe.Photo;
                PhotoImageExtension = recipe.Photo.Substring(recipe.Photo.LastIndexOf("."));
                try
                {
                    if (System.IO.File.Exists(_pathToLoadImage))
                    {
                        Image _image = GlobalProcedures.ImageFromFileNoLock(_pathToLoadImage);
      
      
                        Size _imageNewSize = GlobalProcedures.GetThumbnailSize(_image, 200);

                        Image _ResizedImage = _image.GetThumbnailImage(_imageNewSize.Width, _imageNewSize.Height, null, IntPtr.Zero);
                            pictureBox_Photo.Image = _ResizedImage;
                            pictureBox_Photo.Height = _ResizedImage.Height;
                            pictureBox_Photo.Width = _ResizedImage.Width;

                            //pictureBox_Photo.Tag = _image;
                    }
                }
                catch (Exception Ex)
                {
                    
                    throw;
                }
            }


            InitializeDataGridViewColumns();


            int _rowIndex = 0;
            dataGridView_Ingredients.Rows.Clear();
            foreach (SpiroStockManagmentDatabaseClass.Objects.RecipeIngredient _ingredient in recipe.IngredientList)
            {
                float _tempFloat = 0;
                InitialIngredientsAmountValues.Add(_rowIndex, (float.TryParse(_ingredient.Amount.ToString(), out _tempFloat) ? _tempFloat : 0));

                dataGridView_Ingredients.Rows.Insert(_rowIndex++, new string[] { _ingredient.Name, _ingredient.Amount.ToString(), _ingredient.Units, _ingredient.Information });
                
            }
            ////Steps
            //dataGridView_Steps.Columns.Clear();
            //dataGridView_Steps.Columns.Add("Passos", "Passos");
            //dataGridView_Steps.Columns[0].Width = 900;
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
            IngredientsList = _IngredientList;
            dataGridView_Ingredients.Columns.Add(_DataGridViewTextBoxColumn);


            dataGridView_Ingredients.Columns.Add("Quantidade", "Quantidade");
            dataGridView_Ingredients.Columns.Add("Unidade", "Unidade");
            dataGridView_Ingredients.Columns.Add("Informação", "Informação");

            //Steps
            dataGridView_Steps.Columns.Clear();
            dataGridView_Steps.Columns.Add("Passos", "Passos");
            dataGridView_Steps.Columns[0].Width = 900;
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            //New One
            List<SpiroStockManagmentDatabaseClass.Objects.Ingredient> _ingredientsAdded = new List<SpiroStockManagmentDatabaseClass.Objects.Ingredient>();
            if (CurrentRecipe == null)
            {
                CurrentRecipe = new SpiroStockManagmentDatabaseClass.Objects.Recipe
                {
                    Name = textBox_Name.Text,
                    Category = comboBox_Category.Text,
                    Cuisine = comboBox_Cuisine.Text,
                    TimePreparing = float.Parse(numericUpDown_TimePreparing.Value.ToString()),
                    TimeCooking = float.Parse(numericUpDown_TimeCooking.Value.ToString()),
                    TimeReady = float.Parse(numericUpDown_TimeTotal.Value.ToString()),
                    Description = textBox_Description.Text,
                    Tags = textBox_Tags.Text,
                    Yield = textBox_Yield.Text,
                    Commentary = textBox_Commentary.Text,
                    IngredientList = new List<SpiroStockManagmentDatabaseClass.Objects.RecipeIngredient>(),
                    Directions = new List<SpiroStockManagmentDatabaseClass.Objects.Step>(),
                    Rating = starRatingControl_Rating.SelectedStar
                };
                
                foreach (DataGridViewRow _row in dataGridView_Ingredients.Rows)
                {
                    if (_row.Cells[0].Value != null)
                    {
                        CurrentRecipe.IngredientList.Add(new SpiroStockManagmentDatabaseClass.Objects.RecipeIngredient
                        {
                            Name = _row.Cells[0].Value.ToString(),
                            Amount = (_row.Cells[2].Value != null) ? _row.Cells[1].Value.ToString() : "",
                            Units = (_row.Cells[2].Value != null) ? _row.Cells[2].Value.ToString() : "",
                            Information =  (_row.Cells[3].Value != null) ? _row.Cells[3].Value.ToString() : ""
                        }); 
                        //check if ingredient exists, for purposes of counting
                        //Check if ingredient exists in Ingredients database
                        if (!GlobalVariables.SpiroStockManagmentDatabaseProcedures.CheckIfIngredientExists(_row.Cells[0].Value.ToString()))
                        {
                            SpiroStockManagmentDatabaseClass.Objects.Ingredient _Ingredient = new SpiroStockManagmentDatabaseClass.Objects.Ingredient();
                            _Ingredient.Name = _row.Cells[0].Value.ToString();
                            _Ingredient.Products = new List<SpiroStockManagmentDatabaseClass.Objects.IngredientProduct>();
                            GlobalVariables.SpiroStockManagmentDatabaseProcedures.InsertEditIngredient(_Ingredient);
                            _ingredientsAdded.Add(_Ingredient);
                        }
                    }
                }
                foreach (DataGridViewRow _row in dataGridView_Steps.Rows)
                {
                    if (_row.Cells[0].Value != null)
                    {
                        CurrentRecipe.Directions.Add(new SpiroStockManagmentDatabaseClass.Objects.Step
                            {
                                Value = _row.Cells[0].Value.ToString()
                            }); 
                    }
                }
                //Photo
                int _newRecipeId = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetLastRecipeID() + 1;
                string _pathToSaveImage = "";
                if (pictureBox_Photo.Image != null)
                {
                    _pathToSaveImage = GlobalVariables.RecipeImagesPath + _newRecipeId.ToString() + PhotoImageExtension;
                    if (_pathToSaveImage != string.Empty)
                    {
                        try
                        {
                            (pictureBox_Photo.Tag as Image).Save(_pathToSaveImage);
                            //pictureBox_Photo.Image.Save(_pathToSaveImage);
                        }
                        catch (Exception ex)
                        {
                        }
                        CurrentRecipe.Photo = _newRecipeId.ToString() + PhotoImageExtension;
                    }
                }
            }
            //Update
            else
            {
                CurrentRecipe.Name = textBox_Name.Text;
                CurrentRecipe.Category = comboBox_Category.Text;
                CurrentRecipe.Cuisine = comboBox_Cuisine.Text;
                CurrentRecipe.TimePreparing = float.Parse(numericUpDown_TimePreparing.Value.ToString());
                CurrentRecipe.TimeCooking = float.Parse(numericUpDown_TimeCooking.Value.ToString());
                CurrentRecipe.TimeReady = float.Parse(numericUpDown_TimeTotal.Value.ToString());
                CurrentRecipe.Description = textBox_Description.Text;
                CurrentRecipe.Tags = textBox_Tags.Text;
                CurrentRecipe.Yield = textBox_Yield.Text;
                CurrentRecipe.Commentary = textBox_Commentary.Text;
                CurrentRecipe.Rating = starRatingControl_Rating.SelectedStar;
                CurrentRecipe.IngredientList.Clear();

                //reset to 1 of proportion
                int _rowIndex = 0;
                if (numericUpDown_QuantityProportion.Value != 1)
                {
                    numericUpDown_QuantityProportion.Value = 1;
                    foreach (DataGridViewRow _row in dataGridView_Ingredients.Rows)
                    {
                        if (_row.Cells[0].Value != null)
                        {

                            float _currentAmountDecimal = (InitialIngredientsAmountValues.ContainsKey(_rowIndex)) ? InitialIngredientsAmountValues[_rowIndex] : 0;

                            if (_currentAmountDecimal > 0)
                            {
                                _row.Cells[1].Value = _currentAmountDecimal;
                            }
                        }
                        _rowIndex++;
                    }
                }
                foreach (DataGridViewRow _row in dataGridView_Ingredients.Rows)
                {
                    if (_row.Cells[0].Value != null)
                    {
                        CurrentRecipe.IngredientList.Add(new SpiroStockManagmentDatabaseClass.Objects.RecipeIngredient
                        {
                            Name = _row.Cells[0].Value.ToString(),
                            Amount = (_row.Cells[1].Value != null) ? _row.Cells[1].Value.ToString() : "",
                            Units = (_row.Cells[2].Value != null) ? _row.Cells[2].Value.ToString() : "",
                            Information = (_row.Cells[3].Value != null) ? _row.Cells[3].Value.ToString() : ""
                        });
                        //Check if ingredient exists in Ingredients database
                        if (!GlobalVariables.SpiroStockManagmentDatabaseProcedures.CheckIfIngredientExists(_row.Cells[0].Value.ToString()))
                        {
                            SpiroStockManagmentDatabaseClass.Objects.Ingredient _Ingredient = new SpiroStockManagmentDatabaseClass.Objects.Ingredient();
                            _Ingredient.Name = _row.Cells[0].Value.ToString();
                            _Ingredient.Products = new List<SpiroStockManagmentDatabaseClass.Objects.IngredientProduct>();
                            GlobalVariables.SpiroStockManagmentDatabaseProcedures.InsertEditIngredient(_Ingredient);
                            _ingredientsAdded.Add(_Ingredient);
                        }
                    }
                }

                CurrentRecipe.Directions.Clear();
                foreach (DataGridViewRow _row in dataGridView_Steps.Rows)
                {
                    if (_row.Cells[0].Value != null)
                    {
                        CurrentRecipe.Directions.Add(new SpiroStockManagmentDatabaseClass.Objects.Step
                        {
                            Value = _row.Cells[0].Value.ToString()
                        });
                    }
                }

                string _pathToSaveImage = "";
                if (pictureBox_Photo.Image != null)
                {
                    _pathToSaveImage = GlobalVariables.RecipeImagesPath + CurrentRecipe.Id.ToString() + PhotoImageExtension;
                    try
                    {
                        (pictureBox_Photo.Image as Image).Save(_pathToSaveImage);
                        //pictureBox_Photo.Image.Save(_pathToSaveImage);
                    }
                    catch (Exception ex)
                    {
                    }
                    CurrentRecipe.Photo = CurrentRecipe.Id.ToString() + PhotoImageExtension;
                    //rename image
                    
                }
            }

            GlobalVariables.SpiroStockManagmentDatabaseProcedures.InsertEditRecipe(CurrentRecipe);
            RecipeInsertedOrEdited = true;

            //ask to associate ingredients not found to products
            if (_ingredientsAdded.Count > 0)
            {
                if (MessageBox.Show(_ingredientsAdded.Count.ToString() + " dos ingredientes introduzidos são novos. Quere associar produtos a estes ingredientes?", "Associar", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    //show the ingredients form , with just the non found ingredients
                    Ingredients _IngredientsForm = new Ingredients();
                    _IngredientsForm.Initialize(_ingredientsAdded);
                    _IngredientsForm.ShowDialog();
                }
            }

            this.Close();
        }


        public bool RecipeInsertedOrEdited = false;
        private void button_ChooseImage_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                try
                {
                    //pictureBox_Photo.Image = Image.FromFile(openFileDialog1.FileName);

                    
                    Image _image = GlobalProcedures.ImageFromFileNoLock(openFileDialog1.FileName);
                    Size _imageNewSize = GlobalProcedures.GetThumbnailSize(_image, 200);

                    Image _ResizedImage = _image.GetThumbnailImage(_imageNewSize.Width, _imageNewSize.Height, null, IntPtr.Zero);
                    pictureBox_Photo.Image = _ResizedImage;
                    pictureBox_Photo.Height = _ResizedImage.Height;
                    pictureBox_Photo.Width = _ResizedImage.Width;

                    //pictureBox_Photo.Tag = _image;

                    PhotoImageExtension = openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf("."));
                }
                catch (Exception)
                {
                }
            }
        }
        TextBox CurrentComboBox;

        private void dataGridView_Ingredients_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            string _currentColumnName = dataGridView_Ingredients.Columns[dataGridView_Ingredients.CurrentCell.ColumnIndex].HeaderText;
            if (e.Control is DataGridViewTextBoxEditingControl && _currentColumnName.ToLower() == "ingrediente")
            {
                DataGridViewTextBoxEditingControl te =
                (DataGridViewTextBoxEditingControl)e.Control;
                //CurrentComboBox = (TextBox)sender;
                te.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                te.AutoCompleteSource = AutoCompleteSource.CustomSource;
                te.AutoCompleteCustomSource.AddRange(IngredientsList.ToArray());
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {

        }

        Dictionary<int, float> InitialIngredientsAmountValues = new Dictionary<int, float>();
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

                    if(_currentAmountDecimal > 0)
                    {
                        _row.Cells[1].Value = float.Parse(numericUpDown_QuantityProportion.Value.ToString()) * _currentAmountDecimal;
                    }
                }
                _rowIndex++;
            }
        }

        private void button_SeeItBig_Click(object sender, EventArgs e)
        {
            RecipeIngredientsAndStepsSeeLarge _RecipeIngredientsAndStepsSeeLarge = new RecipeIngredientsAndStepsSeeLarge();
            _RecipeIngredientsAndStepsSeeLarge.Initialize(CurrentRecipe);
            _RecipeIngredientsAndStepsSeeLarge.WindowState = FormWindowState.Maximized;
            _RecipeIngredientsAndStepsSeeLarge.Show();
        }

        //private void dataGridView_Ingredients_CellLeave(object sender, DataGridViewCellEventArgs e)
        //{
        //    //if (CurrentComboBox != null)
        //    //{
        //    //    CurrentComboBox.SelectedIndex = CurrentComboBox.FindStringExact(CurrentComboBox.Text);
        //    //}
        //}
    }
}
