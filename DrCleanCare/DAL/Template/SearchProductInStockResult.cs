namespace DrCleanCare.DAL
{
    public class SearchProductInStockResult
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int StockId { get; set; }

        public string StockName { get; set; }

        public int UnitInStock { get; set; }
    }
}