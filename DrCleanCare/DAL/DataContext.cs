using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace DrCleanCare.DAL
{
    public class DataContext : DbContext
    {
        public DataContext() :
            base("name=DrCleanCareConnectionString")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .Map(m =>
            {
                m.ToTable("UserRoles");
                m.MapLeftKey("UserId");
                m.MapRightKey("RoleId");
            });
        }

        // Tables mapping
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<OrderType> OrderTypes { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockInput> StockInputs { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialImport> MaterialImports { get; set; }
        public DbSet<MaterialExport> MaterialExports { get; set; }
        public DbSet<MaterialInStock> MaterialInStocks { get; set; }
        public DbSet<ProductInStock> ProductInStocks { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}