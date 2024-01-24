namespace CryptoCreditCardRewards.Models.Dtos
{
    public class PagedResultsDto<T>
    {
        public List<T> Items { get; set; }
        public PageDto Page { get; set; }
        public SortOrderDto SortOrder { get; set; }
        public int TotalCount { get; set; }
    }
}
