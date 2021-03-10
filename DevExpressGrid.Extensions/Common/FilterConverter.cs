using DevExpressGrid.ExpressionBuilder.Common;
using DevExpressGrid.ExpressionBuilder.Generics;
using DevExpressGrid.ExpressionBuilder.Interfaces;
using DevExpressGrid.ExpressionBuilder.Operations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DevExpressGrid.Extensions.Common
{
    public static class FilterConverter
    {
        public static Dictionary<string, IOperation> filterMaps = new Dictionary<string, IOperation>
            {
                {"contains",Operation.Contains },
                {"notcontains",Operation.DoesNotContain },
                {"endswith",Operation.EndsWith },
                {"=",Operation.EqualTo },
                {">",Operation.GreaterThan },
                {">=",Operation.GreaterThanOrEqualTo },
                {"<",Operation.LessThan },
                {"<=",Operation.LessThanOrEqualTo },
                {"<>",Operation.NotEqualTo },
                {"startswith",Operation.StartsWith }
                //{"",Operation.NotIn },
                //{"",Operation.In },
                //{"",Operation.IsEmpty },
                //{"",Operation.IsNotEmpty },
                //{"",Operation.IsNotNull },
                //{"",Operation.IsNotNullNorWhiteSpace },
                //{"",Operation.IsNull },
                //{"",Operation.IsNullOrWhiteSpace },
                //{"",Operation.Between },
            };

        public static Expression<Func<T, bool>> ToExpression<T>(string filterQuery) where T : class
        {
            IFilter filter = new Filter<T>();

            if (!string.IsNullOrWhiteSpace(filterQuery))
            {
                var conditions = JsonConvert.DeserializeObject<JArray>(filterQuery);
                GetFilter<T>(conditions, typeof(T).GetProperties(), ref filter);
            }

            return filter as Filter<T>;
        }

        private static void GetFilter<T>(JArray conditions, PropertyInfo[] properties, ref IFilter filter)
        {
            filter.StartGroup();
            foreach (JToken item in conditions)
            {
                if (item.Type == JTokenType.Array)
                {
                    if (item.Children().Any(c => c.Type == JTokenType.Array))
                    {
                        GetFilter<T>(item as JArray, properties, ref filter);
                    }
                    else
                    {
                        var propName = item[0].ToObject<string>();
                        var propType = properties.First(x => x.Name == propName).PropertyType;
                        var conditionValue =  item[2].ToObject(propType);
                        var operation = filterMaps[item[1].ToObject<string>()];
                        if (item.Next != null)
                        {
                            if (item.Next.Type == JTokenType.String)
                            {
                                var logic = item.Next.ToObject<string>();
                                filter.By(propName, operation, conditionValue, logic == "and" ? Connector.And : Connector.Or);
                            }
                        }
                        else
                        {
                            filter.By(propName, operation, conditionValue);
                        }
                    }
                }
                else if (item.Type == JTokenType.String)
                {
                    var propName = item.ToObject<string>();
                    if (propName != "and" && propName != "or")
                    {
                        var propType = properties.First(x => x.Name == propName).PropertyType;
                        var conditionValue = item.Next.Next.ToObject(propType);
                        var operation = filterMaps[item.Next.ToObject<string>()];
                        filter.By(propName, operation, conditionValue);
                        break;
                    }
                }
            }
        }
    }
}
