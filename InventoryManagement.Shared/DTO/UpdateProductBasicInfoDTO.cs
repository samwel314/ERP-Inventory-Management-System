namespace InventoryManagement.Shared.DTO
{
    public class UpdateProductBasicInfoDTO
    {
        public Guid ? ProductId { get; set; }  
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int ? CategoryId   { get; set; }
        public int ? MinimumStock { get; set; }

    }

}
