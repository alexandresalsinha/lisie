using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpiroWeb.Helpers
{
    public static class TextTools
    {
        public static double ParsePriceLocal(string price)
        {
            //if find more then on ',' , remove first one
            int freq = price.Count(f => (f == ','));
            if (freq > 1)
            {
                price = price.Remove(price.IndexOf(','), 1);
            }

            int freq2 = price.Count(f => (f == '.'));
            if (freq2 > 1)
            {
                price = price.Remove(price.IndexOf('.'), 1);
            }

            var _priceRatio = Math.Round(double.Parse(price.Replace("€", "").Replace(",", ".").Trim()), 2);
            return _priceRatio;
        }
        public static double ParsePriceProduction(string price)
        {
            //if find more then on ',' , remove first one
            int freq = price.Count(f => (f == '.'));
            if (freq > 1)
            {
                price = price.Remove(price.IndexOf('.'), 1);
            }
            int freq2 = price.Count(f => (f == ','));
            if (freq2 > 1)
            {
                price = price.Remove(price.IndexOf(','), 1);
            }
            var _priceRatio = Math.Round(double.Parse(price.Replace("€", "").Replace(".", ",").Trim()), 2);
            return _priceRatio;
        }

        public static double ParsePrice(string price)
        {
            //if find more then on ',' , remove first one
            var _priceText = price.Replace("€", "").Replace(",", ".").Trim();
            if (_priceText.Count(f => (f == '.')) > 1)
            {
                _priceText = Regex.Replace(_priceText, @"\.(?=[^.]*\.)", ",");
            }
            //_newPrice = double.Parse(_priceText);
            var _price = Math.Round(double.Parse(_priceText, System.Globalization.CultureInfo.InvariantCulture), 2);
            return _price;
        }

        public static int ComputeLevenshteinDistance(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;

            int sourceWordCount = source.Length;
            int targetWordCount = target.Length;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceWordCount; i++)
            {
                for (int j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
        }

        public static double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
        }


        public static bool SearchInText(string baseString, string searchString)
        {
            baseString = baseString.ToLower().Trim();

            var decomposed = searchString.Normalize(NormalizationForm.FormD);
            var filtered = decomposed.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
            var _normalizedQuery = new String(filtered.ToArray());

            string[] _searchWords = searchString.ToLower().Trim(' ').Split(' ');
            string[] _searchWordsNormalized = _normalizedQuery.ToLower().Trim(' ').Split(' ');

            var _exists = _searchWords.All(z => baseString.Contains(z) || _searchWordsNormalized.All(c => baseString.Contains(c)));
            return _exists;
        }

        public static bool IsBarcodeOfWeightType(string barcode)
        {
            if (barcode.Length < 6)
            {
                return false;
            }
            else
            {
                return barcode.Substring(barcode.Length - 6) == "000000";
            }
        }
    }
}