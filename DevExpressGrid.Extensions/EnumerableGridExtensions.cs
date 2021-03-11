using DevExpressGrid.Extensions.DataExtensions;
using DevExpressGrid.Extensions.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DevExpressGrid.Extensions
{
    public static class EnumerableGridExtensions
    {
        public static FilterResult Load<T>(this IEnumerable<T> enumerable, FilterRequest filterRequest) where T : class
        {
            var total = filterRequest.requireTotalCount ? enumerable.Count() : (int?)null;

            enumerable = enumerable
                .Where(filterRequest.filter)
                .OrderBy(filterRequest._sort)
                .Skip(filterRequest.skip)
                .Take(filterRequest.take);

            var isGroupRequest = filterRequest._group?.Any() ?? false;
            IEnumerable result = isGroupRequest ? enumerable.GroupByAndSelectKey(filterRequest._group) : enumerable;

            return new FilterResult
            {
                data = result,
                totalCount = total
            };
        }
    }
}
