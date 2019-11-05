using System.Collections.Generic;

namespace ProductCatalogApi.ViewModels
{
    public class PaginatedItemsViewModel<TEntity> where TEntity : class
    {
        public int PageSize { get; private set; }

        public int PageIndex { get; private set; }

        public long Count { get; set; }

        public IEnumerable<TEntity> Data { get; set; }

        public PaginatedItemsViewModel(int pageSize, int pageIndex, long count, IEnumerable<TEntity> data)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
            Count = count;
            Data = data;
        }
    }
}