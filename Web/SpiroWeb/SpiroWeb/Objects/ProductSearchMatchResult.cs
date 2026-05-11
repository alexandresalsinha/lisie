namespace SpiroWeb.Objects
{
    public class ProductSearchMatchResult
    {
        public string Name { get; set; }
        public string Search { get; set; }
        public string Url { get; set; }
        public string Brand { get; set; }
        public string Weight { get; set; }
        public string Price { get; set; }

        public float PriceLiteral { get; set; }

        public string PriceWeight { get; set; }

        public float PriceWeightLiteral { get; set; }

        public string StoreName { get; set; }
        public string StoreColor { get; set; }
        public int StoreId { get; set; }
        public string ImageUrl { get; set; }

        public string Category { get; set; }

        public bool IsSeperator { get; set; }
        public string SeparatorTitle { get; set; }
        public string ViewableUrl { get; set; }

        public double EqualsPercentage { get; set; }
        public string EqualsPercentageText { get; set; }
        public double TextEqualsPercentage { get; set; }
        public string TextTogetherEqualsPercentageText { get; set; }

        public double TextTogetherEqualsPercentage { get; set; }
        public double TxPoTxTogetherPo { get; set; }
        public int ImageEqualsPercentage { get; set; }
        public float ImageEqualsPercentage2 { get; set; }

        public float ImagesTogetherPercentage { get; set; }


        public double ImageTextEqualsPercentage { get; set; }
        public double ImageTogetherTextEqualsPercentage { get; set; }
        public double FinalWeight { get; set; }
        public double FinalWeight2 { get; set; }
        public double TxAllTogetherPo { get; set; }
        public double Last2Avg { get; set; }
        public double TxAllTogetherPlus { get; set; }
        public int SortedWeight { get; set; }

    }
}