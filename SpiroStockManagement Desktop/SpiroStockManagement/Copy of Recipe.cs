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
        }
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
            if (recipe.Photo != string.Empty)
            {
                string _pathToLoadImage = GlobalVariables.RecipeImagesPath + recipe.Photo;
                PhotoImageExtension = recipe.Photo.Substring(recipe.Photo.LastIndexOf("."));
                try
                {
                    if (System.IO.File.Exists(_pathToLoadImage))
                    {
                        Image _image = Image.FromFile(_pathToLoadImage);
                        Size _imageNewSize = GlobalProcedures.GetThumbnailSize(_image, 200);
                        _image = _image.GetThumbnailImage(_imageNewSize.Width, _imageNewSize.Height, null, IntPtr.Zero);
                        pictureBox_Photo.Image = _image;
                        pictureBox_Photo.Height = _image.Height;
                        pictureBox_Photo.Width = _image.Width;
                    }
                }
                catch (Exception Ex)
                {
                    
                    throw;
                }
            }

            ////ingredients
            int _rowIndex = 0;
            dataGridView_Ingredients.Rows.Clear();
            foreach (SpiroStockManagmentDatabaseClass.Objects.RecipeIngredient _ingredient in recipe.IngredientList)
            {
                dataGridView_Ingredients.Rows.Insert(_rowIndex++, new string[] { _ingredient.Name, _ingredient.Amount.ToString(), _ingredient.Units, _ingredient.Information });
            }
            //Steps
            _rowIndex = 0;
            dataGridView_Steps.Rows.Clear();
            foreach (SpiroStockManagmentDatabaseClass.Objects.Step _step in recipe.Directions)
            {
                dataGridView_Steps.Rows.Insert(_rowIndex++, new string[] { _step.Value });
            }
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
                        if (GlobalVariables.SpiroStockManagmentDatabaseProcedures.CheckIfIngredientExists(_row.Cells[0].Value.ToString()))
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
                    _pathToSaveImage = Application.StartupPath + "\\RecipeImages\\" + _newRecipeId.ToString() + PhotoImageExtension;
                    if (_pathToSaveImage != string.Empty)
                    {
                        try
                        {
                            pictureBox_Photo.Image.Save(_pathToSaveImage);
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
                foreach (DataGridViewRow _row in dataGridView_Ingredients.Rows)
                {
                    if (_row.Cells[0].Value != null)
                    {
                        CurrentRecipe.IngredientList.Add(new SpiroStockManagmentDatabaseClass.Objects.RecipeIngredient
                        {
                            Name = _row.Cells[0].Value.ToString(),
                            Amount = (_row.Cells[2].Value != null) ? _row.Cells[1].Value.ToString() : "",
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
                    _pathToSaveImage = Application.StartupPath + "\\RecipeImages\\" + CurrentRecipe.Id.ToString() + PhotoImageExtension;
                    try
                    {
                        pictureBox_Photo.Image.Save(_pathToSaveImage);
                    }
                    catch (Exception ex)
                    {
                    }
                    CurrentRecipe.Photo = CurrentRecipe.Id.ToString() + PhotoImageExtension;
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
                    pictureBox_Photo.Image = Image.FromFile(openFileDialog1.FileName);
                    PhotoImageExtension = openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf("."));
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
