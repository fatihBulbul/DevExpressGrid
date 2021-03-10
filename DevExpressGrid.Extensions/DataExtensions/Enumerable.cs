﻿using DevExpressGrid.Extensions.Common;
using DevExpressGrid.Extensions.Models;
using System.Collections.Generic;
using System.Linq;

namespace DevExpressGrid.Extensions.DataExtensions
{
    public static class Enumerable
    {
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, string conditions) where T : class
        {
            var predicate = FilterConverter.ToExpression<T>(conditions).Compile();
            return source.Where(predicate);
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, params SortModel[] sorts)
        {
            for (int i = 0; i < sorts.Length; i++)
            {
                var lambda = ExpressionHelper.ToLambda<T>(sorts[i].selector).Compile();
                var readyToThenBy = i > 0;

                if (sorts[i].desc)
                    source = readyToThenBy ? (source as IOrderedEnumerable<T>).ThenByDescending(lambda) : source.OrderByDescending(lambda);
                else
                    source = readyToThenBy ? (source as IOrderedEnumerable<T>).ThenBy(lambda) : source.OrderBy(lambda);
            }

            return (IOrderedEnumerable<T>)source;
        }
    }
}
