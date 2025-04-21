namespace SimpleBlog.Application.DTOs
{
    public class Pagination<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public int TotalPageNo
        {
            get
            {
                return TotalPages;
            }
        }
        public List<T> Items { get; private set; }
        public Pagination(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }
    }
}
