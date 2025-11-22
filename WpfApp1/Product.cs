using System;

namespace WpfApp1
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Specification { get; set; }
        public string ImageUrl { get; set; }
        public int Stock { get; set; }
        public bool IsAvailable => Stock > 0;
    }
}