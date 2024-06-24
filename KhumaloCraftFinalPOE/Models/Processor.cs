using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraftFinalPOE.Models
{
    public class Processor
    {
        [Key]
        public int ProcessorId { get; set; }
        public string UserId { get; set; }
        public int TransactionId { get; set; }
        public int Quantity { get; set; }
        public DateTime ProcessingDate { get; set; }

        [ForeignKey("TransactionId")]
        public Transactions Transaction { get; set; }
    }
}
