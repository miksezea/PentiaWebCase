﻿namespace PentiaWebCase.Models
{
    public class Orderline
    {
        public int Id { get; set; }
        public string? OrderName { get; set; }
        public int OrderPrice { get; set; }
        public string? OrderDate { get; set; }
        public int SalesPersonId { get; set; }
    }
}
