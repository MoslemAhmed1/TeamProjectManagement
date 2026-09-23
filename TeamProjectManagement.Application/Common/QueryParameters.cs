using TeamProjectManagement.Application.Settings;

namespace TeamProjectManagement.Application.Common
{
    public class QueryParameters
    {
        private static PaginationSettings _pagination = new();
        private int _pageSize = _pagination.DefaultPageSize;

        public static void Configure(PaginationSettings settings)
        {
            _pagination = settings;
        }

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > _pagination.MaxPageSize ? _pagination.MaxPageSize : value;
        }

        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }

        // Free-text search (name, title, description). Exact filters like Status belong on typed query classes.
        public string? SearchTerm { get; set; }
    }
}
