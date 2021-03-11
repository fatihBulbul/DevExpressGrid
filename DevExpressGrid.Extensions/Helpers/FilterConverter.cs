using DevExpressGrid.Extensions.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DevExpressGrid.Extensions.Helpers
{
    internal static class FilterConverter
    {
        public static Expression<Func<T, bool>> ToExpression<T>(string filterQuery) where T : class
        {
            if (!string.IsNullOrWhiteSpace(filterQuery))
            {
                var conditions = JsonConvert.DeserializeObject<JArray>(filterQuery);
                var filter = ParseFilter<T>(conditions, typeof(T).GetProperties());
                var predicate = filter.Build();
                return predicate;
            }

            return x => true;
        }

        private static Dictionary<string, Func<Expression, Expression, Expression>> filters = new Dictionary<string, Func<Expression, Expression, Expression>>
        {
            { ">", (member, constant) => Expression.GreaterThan(member,constant) },
            { ">=", (member, constant) => Expression.GreaterThanOrEqual(member,constant) },
            { "<", (member, constant) => Expression.LessThan(member,constant) },
            { "<=", (member, constant) => Expression.LessThanOrEqual(member,constant) },
            { "=", (member, constant) => Expression.Equal(member,constant) },
            { "<>", (member, constant) => Expression.NotEqual(member,constant) },
            { "contains", (member, constant) => ExpressionHelper.StringContains(member,constant) },
            { "notcontains", (member, constant) => ExpressionHelper.StringNotContains(member,constant) },
            { "startswith", (member, constant) => ExpressionHelper.StringStartsWith(member,constant) },
            { "endswith", (member, constant) => ExpressionHelper.StringEndsWith(member,constant) },
        };

        private static Expression<Func<T, bool>> GetExpression<T>(string propertyName, string filterType, object value)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);

            if (value == null && !TypeHelper.IsNullableType(propertyInfo.PropertyType))
            {
                if (filterType == "=")
                {
                    return x => false;
                }
                else if (filterType == "<>")
                {
                    return x => true;
                }
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var member = Expression.MakeMemberAccess(parameter, propertyInfo);
            var changeTypeValue = value == null ? null : Convert.ChangeType(value, propertyInfo.PropertyType);
            var constant = Expression.Constant(changeTypeValue, propertyInfo.PropertyType);

            var body = filters[filterType](member, constant);
            return Expression.Lambda<Func<T, bool>>(body, new[] { parameter });
        }

        private static Filter<T> ParseFilter<T>(JArray conditions, PropertyInfo[] properties) where T : class
        {
            var filter = new Filter<T>();

            foreach (JToken item in conditions)
            {
                if (item.Type == JTokenType.String)
                {
                    var propName = item.ToObject<string>();

                    if (propName == "!")
                    {
                        filter.Not = true;
                    }
                    else if (propName == "and")
                    {
                        filter.Logic = Logic.AND;
                    }
                    else if (propName == "or")
                    {
                        filter.Logic = Logic.OR;
                    }
                    else
                    {
                        var propType = properties.First(x => x.Name == propName).PropertyType.ToNullableType();
                        var conditionValue = item.Next.Next.ToObject(propType);
                        filter.Filters.Add(GetExpression<T>(propName, item.Next.ToObject<string>(), conditionValue));
                        break;
                    }

                }
                else if (item.Type == JTokenType.Array)
                {
                    if (item.Children().Any(c => c.Type == JTokenType.Array))
                    {
                        filter.SubFilter = ParseFilter<T>(item as JArray, properties);
                    }
                    else
                    {
                        var propName = item[0].ToObject<string>();
                        var propType = properties.First(x => x.Name == propName).PropertyType.ToNullableType();
                        var conditionValue = item[2].ToObject(propType);

                        var operation = item[1].ToObject<string>();
                        if (item.Next != null)
                        {
                            if (item.Next.Type == JTokenType.String)
                            {
                                var logic = item.Next.ToObject<string>();
                                filter.Filters.Add(GetExpression<T>(propName, operation, conditionValue));
                                filter.Logic = logic == "and" ? Logic.AND : Logic.OR;
                            }
                        }
                        else
                        {
                            filter.Filters.Add(GetExpression<T>(propName, operation, conditionValue));
                        }
                    }
                }
            }

            return filter;
        }
    }
}
