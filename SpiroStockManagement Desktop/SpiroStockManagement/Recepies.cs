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
using EXControls;

namespace SpiroStockManagement
{
    public partial class Recepies : UserControl
    {
        public Recepies()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            InitializeCategoriesAndCuisine();
            
            //Select all
            treeView_Categories.SelectedNode = treeView_Categories.Nodes[0];
            CurrentRecipeList = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllRecepies().ToList();
            RefreshRecepies();
        }

        void InitializeCategoriesAndCuisine()
        {
            //Categories  
            treeView_Categories.Nodes["Categorias"].Nodes.Clear();
            foreach (string _category in GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetRecipeCategoriesDistinct())
            {
                treeView_Categories.Nodes["Categorias"].Nodes.Add(_category);
            }

            //Cuisine
            treeView_Categories.Nodes["Cozinhas"].Nodes.Clear();
            foreach (string _cuisine in GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetRecipeCuisineDisctinct())
            {
                treeView_Categories.Nodes["Cozinhas"].Nodes.Add(_cuisine);
            }

            treeView_Categories.Nodes[1].ExpandAll();
            treeView_Categories.Nodes[2].ExpandAll();
        }

        List<XElement> CurrentRecipeList = null;
        private void treeView_Categories_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //if (e.Node.Parent == null && e.Node.Text == "Todas")
            //{
            //    CurrentRecipeList = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllRecepies().ToList();
            //    RefreshRecepies();
            //}
            //if (e.Node.Parent != null && e.Node.Parent.Text == "Categorias")
            //{
            //    CurrentRecipeList = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetCategoryRecepies(e.Node.Text).ToList();
            //    RefreshRecepies();
            //}
            //if (e.Node.Parent != null && e.Node.Parent.Text == "Cozinhas")
            //{
            //    CurrentRecipeList = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetCuisineRecepies(e.Node.Text).ToList();
            //    RefreshRecepies();
            //}
            UpdateCurrentRecipeList();
        }

        void UpdateCurrentRecipeList()
        {
            if (treeView_Categories.SelectedNode.Parent == null && treeView_Categories.SelectedNode.Text == "Todas")
            {
                CurrentRecipeList = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetAllRecepies().ToList();
                RefreshRecepies();
            }
            if (treeView_Categories.SelectedNode.Parent != null && treeView_Categories.SelectedNode.Parent.Text == "Categorias")
            {
                CurrentRecipeList = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetCategoryRecepies(treeView_Categories.SelectedNode.Text).ToList();
                RefreshRecepies();
            }
            if (treeView_Categories.SelectedNode.Parent != null && treeView_Categories.SelectedNode.Parent.Text == "Cozinhas")
            {
                CurrentRecipeList = GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetCuisineRecepies(treeView_Categories.SelectedNode.Text).ToList();
                RefreshRecepies();
            }
        }

        void RefreshRecepies()
        {
            listView_Recepies.Items.Clear();
            listView_Recepies.BeginUpdate();
            foreach (XElement _XRecipe in CurrentRecipeList)
            {
                SpiroStockManagmentDatabaseClass.Objects.Recipe _currentRecipe = new SpiroStockManagmentDatabaseClass.Objects.Recipe();
                _currentRecipe = (SpiroStockManagmentDatabaseClass.Objects.Recipe)SpiroStockManagmentDatabaseClass.XmlSerializerExtension.DeSerializerToObject(_XRecipe, _currentRecipe);

                EXListViewItem item = new EXListViewItem();
                EXControlListViewSubItem cs = new EXControlListViewSubItem();
                item.Tag = _currentRecipe;
                ListViewRecipe _ListViewRecipe = new ListViewRecipe();
                _ListViewRecipe.Initialize(GlobalVariables.SpiroStockManagmentDatabaseProcedures.GetRecipe(_currentRecipe.Id), item);
                _ListViewRecipe.OnListViewItemRecipeDoubleClick += new EventHandler(_ListViewRecipe_OnListViewItemRecipeDoubleClick);
                _ListViewRecipe.OnListViewItemRecipeClick += new EventHandler(_ListViewRecipe_OnListViewItemRecipeClick);
                _ListViewRecipe.OnListViewItemRecipeLeftClick += new EventHandler(_ListViewRecipe_OnListViewItemRecipeLeftClick);
                item.SubItems.Add(cs);
                listView_Recepies.AddControlToSubItem(_ListViewRecipe, cs);
                listView_Recepies.Items.Add(item);

            }
            listView_Recepies.EndUpdate();
        }

        void _ListViewRecipe_OnListViewItemRecipeLeftClick(object sender, EventArgs e)
        {
            listView_Recepies.SelectedItems.Clear();
            listView_Recepies.Select();
            (sender as ListViewRecipe).CurrentListViewItem.Selected = true;

            contextMenuStrip_Recepies.Show(listView_Recepies, listView_Recepies.PointToClient(System.Windows.Forms.Cursor.Position));
        }

        void _ListViewRecipe_OnListViewItemRecipeClick(object sender, EventArgs e)
        {
            listView_Recepies.SelectedItems.Clear();
            listView_Recepies.Select();
            (sender as ListViewRecipe).CurrentListViewItem.Selected = true;
        }

        void _ListViewRecipe_OnListViewItemRecipeDoubleClick(object sender, EventArgs e)
        {
            Recipe _RecipeForm = new Recipe();
            _RecipeForm.Initialize((SpiroStockManagmentDatabaseClass.Objects.Recipe)(sender as ListViewRecipe).CurrentRecipe);
            _RecipeForm.FormClosed += new FormClosedEventHandler(_RecipeForm_FormClosed);
            _RecipeForm.Show();
            _RecipeForm.WindowState = FormWindowState.Maximized;

            listView_Recepies.SelectedItems.Clear();
            listView_Recepies.Select();
            (sender as ListViewRecipe).CurrentListViewItem.Selected = true;
        }

        private void listView_Recepies_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView_Recepies.SelectedItems.Count > 0)
            {
                if (listView_Recepies.SelectedItems[0].Tag != null)
                {
                    Recipe _RecipeForm = new Recipe();
                    _RecipeForm.Initialize((SpiroStockManagmentDatabaseClass.Objects.Recipe)listView_Recepies.SelectedItems[0].Tag);
                    _RecipeForm.FormClosed += new FormClosedEventHandler(_RecipeForm_FormClosed);
                    _RecipeForm.Show();
                    _RecipeForm.WindowState = FormWindowState.Maximized;
                }
            }
        }

        void _RecipeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if ((sender as Recipe).RecipeInsertedOrEdited)
            {
                InitializeCategoriesAndCuisine();
                UpdateCurrentRecipeList();
                //RefreshRecepies();
            }
        }

        private void toolStripButton_New_Click(object sender, EventArgs e)
        {
            Recipe _RecipeForm = new Recipe();
            _RecipeForm.Initialize();
            _RecipeForm.FormClosed += new FormClosedEventHandler(_RecipeForm_FormClosed);
            _RecipeForm.Show();
            _RecipeForm.WindowState = FormWindowState.Maximized;
        }

        private void apagarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Tem a certeza que quer apagar o/s produtos/s seleccionados?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (ListViewItem _ListViewItem in listView_Recepies.SelectedItems)
                {

                    GlobalVariables.SpiroStockManagmentDatabaseProcedures.DeleteRecipe(((SpiroStockManagmentDatabaseClass.Objects.Recipe)_ListViewItem.Tag).Id);
                    InitializeCategoriesAndCuisine();
                    UpdateCurrentRecipeList();
                }
            }
        }

        private void treeView_Categories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateCurrentRecipeList();
        }

        private void toolStripButton_Ingridients_Click(object sender, EventArgs e)
        {
            Ingredients _IngredientsForm = new Ingredients();
            _IngredientsForm.Initialize();
            _IngredientsForm.Show();
        }

        private void listView_Recepies_KeyUp(object sender, KeyEventArgs e)
        {
            if ( (char)e.KeyCode == '\r')
            {
                if (listView_Recepies.SelectedItems.Count > 0)
                {
                    Recipe _RecipeForm = new Recipe();
                    _RecipeForm.Initialize((SpiroStockManagmentDatabaseClass.Objects.Recipe)listView_Recepies.SelectedItems[0].Tag);
                    _RecipeForm.FormClosed += new FormClosedEventHandler(_RecipeForm_FormClosed);
                    _RecipeForm.Show();
                    _RecipeForm.WindowState = FormWindowState.Maximized;
                }
            }
        }

        private void adicionarÁListaDeComprasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView_Recepies.SelectedItems.Count > 0)
            {
                SpiroStockManagmentDatabaseClass.Objects.Recipe _currentRecipe = (listView_Recepies.SelectedItems[0].Tag as SpiroStockManagmentDatabaseClass.Objects.Recipe);

                AddRecipeIngredientsToList _AddRecipeIngredientsToList = new AddRecipeIngredientsToList();
                _AddRecipeIngredientsToList.Initialize(_currentRecipe);
                _AddRecipeIngredientsToList.ShowDialog();
            }
        }
    }
}
