namespace InventoryManagement.Shared.DTO
{
    public class WarehouseDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? Address { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get;  set; }
    }
}
