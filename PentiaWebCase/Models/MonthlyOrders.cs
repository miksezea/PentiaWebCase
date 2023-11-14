namespace PentiaWebCase.Models
{
    public class MonthlyOrders
    {
        public string? Year { get; set; }
        public string? Month { get; set; }
        public int OrderCount { get; set; }

        public MonthlyOrders(string? year, string? month, int orderCount)
        {
            Year = year;
            Month = month;
            OrderCount = orderCount;
        }
    }
}
