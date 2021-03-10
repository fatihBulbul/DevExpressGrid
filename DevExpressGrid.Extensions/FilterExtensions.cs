using DevExpressGrid.Extensions.DataExtensions;
using DevExpressGrid.Extensions.Models;
using System.Collections.Generic;
using System.Linq;

namespace DevExpressGrid.Extensions
{
    public static class FilterExtensions
    {
        public static FilterResult<T> GetData<T>(this IQueryable<T> queryable, FilterRequest filterRequest) where T : class
        {
            var total = filterRequest.requireTotalCount ? queryable.Count() : (int?)null;

            queryable = queryable
                .Where(filterRequest.filter)
                .OrderBy(filterRequest._sort)
                .Skip(filterRequest.skip)
                .Take(filterRequest.take);

            return new FilterResult<T>
            {
                data = queryable.AsEnumerable(),
                totalCount = total
            };
        }

        public static FilterResult<T> GetData<T>(this IEnumerable<T> enumerable, FilterRequest filterRequest) where T : class
        {
            var total = filterRequest.requireTotalCount ? enumerable.Count() : (int?)null;

            enumerable = enumerable
                .Where(filterRequest.filter)
                .OrderBy(filterRequest._sort)
                .Skip(filterRequest.skip)
                .Take(filterRequest.take);

            return new FilterResult<T>
            {
                data = enumerable,
                totalCount = total
            };
        }
    }
}
