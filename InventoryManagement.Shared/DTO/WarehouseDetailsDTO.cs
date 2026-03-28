namespace InventoryManagement.Shared.DTO
{
    public class WarehouseDetailsDTO
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string City { get; private set; } = null!;
        public string? Address { get; private set; } = null!;
    }
}
