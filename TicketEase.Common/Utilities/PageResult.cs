namespace TicketEase.Common.Utilities
{
    public class PageResult<T>
    {
        public T Data { get; set; }
        public int PerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPageCount { get; set; }
        public int TotalCount { get; set; }
    }
}
