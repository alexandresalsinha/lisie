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
    public partial class ListViewRecipe : UserControl
    {
        public ListViewRecipe()
        {
            InitializeComponent();
        }
        public SpiroStockManagmentDatabaseClass.Objects.Recipe CurrentRecipe;
        public ListViewItem CurrentListViewItem;

        public void Initialize(SpiroStockManagmentDatabaseClass.Objects.Recipe recipe, ListViewItem listViewItem)
        {
            CurrentRecipe = recipe;
            CurrentListViewItem = listViewItem;

            if (System.IO.File.Exists(GlobalVariables.RecipeImagesPath + recipe.Photo))
            {
                Image _image = GlobalProcedures.ImageFromFileNoLock(GlobalVariables.RecipeImagesPath + recipe.Photo);
                Size _imageNewSize = GlobalProcedures.GetThumbnailSize(_image, 120);
                _image = _image.GetThumbnailImage(_imageNewSize.Width, _imageNewSize.Height, null, IntPtr.Zero);
                pictureBox_photo.Image = _image; 
            }

            //image.GetThumbnailImage(64, 64, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
            //pictureBox_photo.Image = GlobalProcedures.ThumbnailImage(GlobalVariables.RecipeImagesPath + recipe.Photo, 120, 120);
            label_Name.Text = recipe.Name;
            label_Description.Text = recipe.Description;
            starRatingControl1.SelectedStar = recipe.Rating;
        }

        public event EventHandler OnListViewItemRecipeDoubleClick;
        public event EventHandler OnListViewItemRecipeClick;
        public event EventHandler OnListViewItemRecipeLeftClick; 
        private void pictureBox_photo_DoubleClick(object sender, EventArgs e)
        {
            if (OnListViewItemRecipeDoubleClick != null)
            {
                OnListViewItemRecipeDoubleClick(this, new EventArgs());
                //SelectItem();
            }
        }

        public void SelectItem()
        {
            this.BackColor = Color.Navy;
        }

        public void DeSelectItem()
        {
            this.BackColor = System.Drawing.SystemColors.Window;
        }

        private void pictureBox_photo_Click(object sender, EventArgs e)
        {
            if (OnListViewItemRecipeClick != null)
            {
                OnListViewItemRecipeClick(this, new EventArgs());
            }
        }

        private void pictureBox_photo_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (OnListViewItemRecipeLeftClick != null)
                {
                    OnListViewItemRecipeLeftClick(this, new EventArgs());
                }
            }
        }
    }
}
