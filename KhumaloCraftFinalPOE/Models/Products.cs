using System.ComponentModel.DataAnnotations;

namespace KhumaloCraftFinalPOE.Models
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public bool Availability { get; set; }
        public string ImageUrl { get; set; }
    }
}
