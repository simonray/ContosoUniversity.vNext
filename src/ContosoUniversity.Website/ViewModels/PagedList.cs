using System.Collections.Generic;
using System.Linq;

namespace ContosoUniversity.ViewModels
{
    public interface IPagedList<T> : IEnumerable<T>
    {
        IReadOnlyCollection<T> Items { get; }
        int Page { get; }
        int PageSize { get; }
        int TotalCount { get; }
    }

    public class PagedList<T> : IPagedList<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IReadOnlyCollection<T> Items { get; set; }

        public PagedList(int page, int pageSize, int totalCount, IEnumerable<T> source)
        {
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
            Items = source.ToList(); 
        }

        public IEnumerator<T> GetEnumerator()
            => Items.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => Items.GetEnumerator();
    }
}
