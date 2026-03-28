using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.Domain.Entities
{
    public class Warehouse
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string City { get; private set; } = null!;
        public string? Address { get; private set; } = null!; 
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        private Warehouse() { }    
        public Warehouse(string name , string city)
        {
            ValidateName(name);
            ValidateCity(city); 
            Name = name;
            City = city;        
        }
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required");
            if (name.Length > 100)
                throw new ArgumentException("Name must be less than 100 characters");
        }
        private void ValidateCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("city is required");
            if (city.Length > 50)
                throw new ArgumentException("city must be less than 50 characters");
        }
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateName(string name)
        {
            ValidateName(name);
            Name = name;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateCity(string city)
        {
            ValidateCity(city);
            City = city;
            UpdatedAt = DateTime.UtcNow;
        } 
        public void UpdateAddress(string address)
        {
            Address = address;
            UpdatedAt = DateTime.UtcNow;
        } 
    }
}
