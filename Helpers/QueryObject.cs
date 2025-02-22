using System.ComponentModel.DataAnnotations;

namespace Exadel.BookManagement.API.Helpers
{
    public class QueryObject
    {
        public string Title { get; set; } = string.Empty;
        public EnumHelper SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
        [Range(1, 100)]
        public int PageNumber { get; set; } = 1;
        [Range(1, 100)]
        public int PageSize { get; set; } = 20;
    }
}
