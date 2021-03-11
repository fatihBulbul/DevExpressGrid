﻿using DevExpressGrid.Extensions.DataExtensions;
using DevExpressGrid.Extensions.Models;
using System.Collections;
using System.Linq;

namespace DevExpressGrid.Extensions
{
    public static class QueryableGridExtensions
    {
        public static FilterResult Load<T>(this IQueryable<T> queryable, FilterRequest filterRequest) where T : class
        {
            var total = filterRequest.requireTotalCount ? queryable.Count() : (int?)null;

            queryable = queryable
                .Where(filterRequest.filter)
                .OrderBy(filterRequest._sort)
                .Skip(filterRequest.skip)
                .Take(filterRequest.take);

            var isGroupRequest = filterRequest._group?.Any() ?? false;
            IEnumerable result = isGroupRequest ? queryable.GroupByAndSelectKey(filterRequest._group).AsEnumerable() : queryable.AsEnumerable();
            return new FilterResult
            {
                data = result,
                totalCount = total
            };
        }
    }
}
