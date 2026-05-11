using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SpiroWeb.Helpers
{
    public static class ProductsCategories
    {
        public static List<string> categories = new List<string>(new string[] {
            "Mercearia",
            "Frescos",
            "Bebidas",
            "Laticínios",
            "Congelados",
            "Saudável",
            "Higiene e Beleza",
            "Limpeza",
            "Animais",
            "Casa",
            "Lifestyle",
            "Bébé",
            "Escritório e Multimédia",
            "Eletrodomésticos"
        });

        public static bool Exists(string category)
        {
            category = category.ToLower().Trim(' ');
            var decomposed = category.Normalize(NormalizationForm.FormD);
            var filtered = decomposed.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
            var normalizedCategory = new String(filtered.ToArray());

            foreach (var _category in categories)
            {
                var _categoryLowered = _category.ToLower().Trim(' ');
                var decomposed2 = _categoryLowered.Normalize(NormalizationForm.FormD);
                var filtered2 = decomposed2.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
                var _normalizedCategory = new String(filtered2.ToArray());

                if (normalizedCategory.Equals(_normalizedCategory))
                    return true;
            }

            return false;
        }
    }
}