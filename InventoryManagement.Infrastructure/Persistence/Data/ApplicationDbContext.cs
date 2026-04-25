using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.Infrastructure.Persistence.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }  
        public DbSet<StockMovement> StockMovements { get; set; }    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // -*-*-*-* Category
            modelBuilder.Entity<Category>().Property(c => c.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Category>().HasMany(c => c.Products).WithOne(p => p.Category).HasForeignKey(p => p.CategoryId);
            //- *-*-*-* Product 
            modelBuilder.Entity<Product>().Property(p => p.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Product>().Property(p => p.Description).HasMaxLength(1000);
            modelBuilder.Entity<Product>().Property(p => p.SKU).IsRequired().HasMaxLength(100);

            modelBuilder.Entity<Product>().HasIndex(p => p.SKU).IsUnique();
            modelBuilder.Entity<Product>().Property(p => p.MinimumStock).IsRequired();
            modelBuilder.Entity<Product>().ToTable(t =>
            {
                t.HasCheckConstraint("CK_Product_MinimumStock_GreaterThanZero", "[MinimumStock] >= 0 ");
                t.HasCheckConstraint("CK_Product_SellingPrice_GreaterThanZero", "[SellingPrice] > 0 ");
                t.HasCheckConstraint("CK_Product_PurchasePrice_GreaterThanZero", "[PurchasePrice] > 0 ");
            });
            modelBuilder.Entity<Product>().Property(p => p.SellingPrice).IsRequired().HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(p => p.PurchasePrice).IsRequired().HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(p => p.ImageUrl).IsRequired();

            /// *-*-*-*-* Warehouse 
            modelBuilder.Entity<Warehouse>().Property(c => c.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Warehouse>().Property(c => c.City).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Warehouse>().Property(c => c.Address).HasMaxLength(150);

            // *-*-*-*-* Product Stock 
          
            modelBuilder.Entity<ProductStock>().HasKey(ps => new { ps.ProductId, ps.WarehouseId });
            modelBuilder.Entity<ProductStock>().Property(ps => ps.Quantity).IsRequired();
            modelBuilder.Entity<ProductStock>().HasOne(ps => ps.Product).WithMany().HasForeignKey(ps => ps.ProductId);
            modelBuilder.Entity<ProductStock>().HasOne(ps => ps.Warehouse).WithMany().HasForeignKey(ps => ps.WarehouseId);
            // *-*- *-*-* Stock Movement
            modelBuilder.Entity<StockMovement>().HasOne(sm => sm.Product).WithMany().HasForeignKey(sm => sm.ProductId);
            modelBuilder.Entity<StockMovement>().HasOne(sm => sm.FromWarehouse).WithMany().HasForeignKey(sm => sm.FromWarehouseId);
            modelBuilder.Entity<StockMovement>().HasOne(sm => sm.ToWarehouse).WithMany().HasForeignKey(sm => sm.ToWarehouseId);
        }
    }
}
