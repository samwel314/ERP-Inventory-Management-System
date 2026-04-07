using InventoryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.UnitTests.Domain
{
    public class CategoryTests
    {
        [Fact]
        public void CanBeDeleted_IfProductsCountIsZero_ShouldReturnTrue()
        {
            var category = new Category("TestOne"); 
            var result = category.CanBeDeleted(0);
            Assert.True(result); 
        }
        [Fact]
        public void CanBeDeleted_IfProductsCountIsMoreThanZero_ShouldReturnFalse()
        {
            var category = new Category("TestOne");
            var result = category.CanBeDeleted(10);
            Assert.False(result);
        }
    }
}
