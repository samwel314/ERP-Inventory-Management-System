using InventoryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.UnitTests.Domain
{
    public class ProductTests
    {
        [Fact]
        public void Constructor_WhenNameIsTooLong_ShouldFail()
        {
            string longName = new string('A', 101);
            Assert.Throws<ArgumentException>(
                () => new Product
                (longName, "Sk-123-41", 15.5m, 12.5m, 10, "/images/p1.png", 1));
        }

        [Fact]
        public void ValidateSellingPrice_WhenSellingPriceIsPositive_ShouldSucceed()
        {
            var testSellingPrice = 10.5m;
            var product = new Product
                ("Ap-name", "Sk-123-41", 15.5m, 12.5m, 10, "/images/p1.png", 1);
            product.UpdateSellingPrice(testSellingPrice);
            Assert.Equal(testSellingPrice , product.SellingPrice);
        }
        [Fact]
        public void ValidateSellingPrice_WhenSellingPrice_IsNegative_ShouldFail()
        {
            var testSellingPrice = -1.5m;
            var product = new Product
                ("Ap-name", "Sk-123-41", 15.5m, 12.5m, 10, "/images/p1.png", 1);
            Assert.Throws<ArgumentException>
                (() => product.UpdateSellingPrice(testSellingPrice));
        }
        [Fact]
        public void ProfitPerUnit_ShouldReturnDifferenceBetweenSellingAndPurchasePrice()
        {
            var sellingPrice = 200m;
            var purchasePrice = 150m;
            var product = new Product
                ("Ap-name", "Sk-123-41", sellingPrice, purchasePrice, 10, "/images/p1.png", 1);
            var profit = product.ProfitPerUnit();
            var expectedProfit = sellingPrice - purchasePrice;
            Assert.Equal(expectedProfit, profit);
        }
    }
}
