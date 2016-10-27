using eCommerce.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace eCommerce.DAL
{
    public class CommerceContext : DbContext
    {

        public CommerceContext() : base("CommerceContext")
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();   
        }
    }
}