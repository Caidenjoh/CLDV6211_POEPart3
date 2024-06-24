using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraftFinalPOE.Models
{
    public class OrderHistory
    {
        [Key]
        public int OrderHistoryId { get; set; }
        public string UserId { get; set; }
        public int TransactionId { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Products Products { get; set; }
    }
}
