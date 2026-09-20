namespace TeamProjectManagement.Application.Common
{
    public class QueryParameters
    {
        private const int MaxPageSize = 50; // TODO: put into configuration file
        private int _pageSize = 10; // TODO: put into configuration file

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
        public string? SearchTerm { get; set; }
    }
}
