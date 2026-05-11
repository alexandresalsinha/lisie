using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiroStockManagement
{
    public delegate void ChangeStatusBarEventHandler(object sender, string message);

    public static class GlobalVariables
    {

        public static SpiroStockManagmentDatabaseClass.Procedures SpiroStockManagmentDatabaseProcedures;
        public static MainWindow MainWindowHandle;
        public static string RecipeImagesPath, ProductImagesPath;
        public static InsertItem CurrentInsertItemDialog;
        public static bool EnterPressedIsToAddProduct = false;
        public static DateTime LastTimeDatabaseWasChangedByMe;
    }
}
