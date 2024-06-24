using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KhumaloCraftFinalPOE.Models;

namespace KhumaloCraftFinalPOE.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<KhumaloCraftFinalPOE.Models.OrderHistory> OrderHistory { get; set; } = default!;
        public DbSet<KhumaloCraftFinalPOE.Models.Processor> Processor { get; set; } = default!;
        public DbSet<KhumaloCraftFinalPOE.Models.Products> Products { get; set; } = default!;
        public DbSet<KhumaloCraftFinalPOE.Models.Transactions> Transactions { get; set; } = default!;
    }
}
