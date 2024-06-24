using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCraftFinalPOE.Models
{
    public class Transactions
    {
        [Key]
        public int TransactionId { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }

        [ForeignKey("ProductId")]
        public Products Products { get; set; }
    }
}
