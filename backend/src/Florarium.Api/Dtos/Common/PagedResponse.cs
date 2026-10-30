namespace Florarium.Api.Dtos.Common;

public class PagedResponse<T>
{
    public IEnumerable<T> Items {get; set;} = [];
    public int Total {get; set;}
    public int Page {get; set;}
    public int PageSize {get; set;}
}