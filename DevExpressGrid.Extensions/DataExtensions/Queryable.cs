using DevExpressGrid.Extensions.Helpers;
using DevExpressGrid.Extensions.Models;
using System.Linq;

namespace DevExpressGrid.Extensions.DataExtensions
{
    internal static class Queryable
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> source, string conditions) where T : class
        {
            var predicate = FilterConverter.ToExpression<T>(conditions);
            return source.Where(predicate);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, params SortModel[] sorts)
        {
            if (sorts == null)
                return (IOrderedQueryable<T>)source;

            for (int i = 0; i < sorts.Length; i++)
            {
                var lambda = ExpressionHelper.ToLambda<T>(sorts[i].selector);
                var readyToThenBy = i > 0;

                if (sorts[i].desc)
                    source = readyToThenBy ? (source as IOrderedQueryable<T>).ThenByDescending(lambda) : source.OrderByDescending(lambda);
                else
                    source = readyToThenBy ? (source as IOrderedQueryable<T>).ThenBy(lambda) : source.OrderBy(lambda);
            }

            return (IOrderedQueryable<T>)source;
        }

        public static IQueryable<object> GroupByAndSelectKey<T>(this IQueryable<T> source, params GroupModel[] groups)
        {
            var lambda = ExpressionHelper.ToLambda<T>(groups[0].selector);
            return source.GroupBy(lambda).Select(x => new
            {
                count = x.Count(),
                key = x.Key
            });
        }
    }
}
