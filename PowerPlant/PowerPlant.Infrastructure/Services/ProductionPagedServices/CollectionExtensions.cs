using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerPlant.Infrastructure.Services.ProductionPagedServices
{
    public class Pagination<T> where T : class
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public List<T> Items { get; set; }
    }

    public static class CollectionExtensions
    {
        public static Pagination<T> GetPaginationData<T>(this ICollection<T> data, int page, int limit) where T : class
        {
            var paged = new Pagination<T>();

            page = (page < 0) ? 1 : page;
            paged.CurrentPage = page;
            paged.PageSize = limit;

            var startRow = (page - 1) * limit;
            paged.Items = data.Skip(startRow).Take(limit).ToList();

            paged.TotalItems = data.Count();
            paged.TotalPages = (int)Math.Ceiling(paged.TotalItems / (double)limit);

            return paged;
        }
    }
}
