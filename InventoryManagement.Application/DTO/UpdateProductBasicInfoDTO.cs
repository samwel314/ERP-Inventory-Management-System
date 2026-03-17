namespace InventoryManagement.Application.DTO
{
    public class UpdateProductBasicInfoDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int CategoryId   { get; set; }
        public int MinimumStock { get; set; }

    }

}
