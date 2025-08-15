using Microsoft.EntityFrameworkCore;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess
{
    public class DataContext : DbContext
    {
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerPreference> CustomerPreferences { get; set; }

        public DataContext() { }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(b =>
            {
                b.HasKey(x => x.Id);
            });

            modelBuilder.Entity<CustomerPreference>(b =>
            {
                b.HasKey(x => new { x.CustomerId, x.PreferenceId });

                b.HasOne(x => x.Customer)
                 .WithMany(c => c.CustomerPreferences)
                 .HasForeignKey(x => x.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.Property(x => x.PreferenceId).IsRequired();
                b.HasIndex(x => x.PreferenceId);
            });

            modelBuilder.Entity<PromoCode>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.PreferenceId).IsRequired();
            });
        }
    }
}