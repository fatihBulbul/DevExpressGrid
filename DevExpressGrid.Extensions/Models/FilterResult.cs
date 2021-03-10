using System.Collections.Generic;

namespace DevExpressGrid.Extensions.Models
{
    public class FilterResult<T> where T:class
    {
        public IEnumerable<T> data { get; set; }
        public int? totalCount { get; set; }
    }
}
