namespace InventoryManagement.Shared.DTO
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int Next { get; set; }
        public int Previous { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
