using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DevExpressGrid.Extensions.Models
{
    internal class Filter<T>
    {
        public Filter()
        {
            Filters = new List<Expression<Func<T, bool>>>();
        }

        public bool Not { get; set; }
        public Logic Logic { get; set; }
        public List<Expression<Func<T, bool>>> Filters { get; set; }

        public Filter<T> SubFilter { get; set; }
    }
}
