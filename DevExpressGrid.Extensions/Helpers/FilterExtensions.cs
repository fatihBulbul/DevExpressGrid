using DevExpressGrid.Extensions.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DevExpressGrid.Extensions.Helpers
{
    internal static class FilterExtensions
    {
        public static Expression<Func<T, bool>> Build<T>(this Filter<T> filter)
        {
            var filters = filter.Filters.ToArray();

            if (filter.SubFilter != null)
                filters = filters.Concat(new[] { filter.SubFilter.Build() }).ToArray();

            var expression = filter.Logic == Logic.AND ? ExpressionHelper.And(filters) : ExpressionHelper.Or(filters);

            if (filter.Not)
                expression = expression.Not();

            return expression;
        }
    }
}
