using System.Collections;

namespace DevExpressGrid.Extensions.Models
{
    public class FilterResult
    {
        public IEnumerable data { get; set; }
        public int? totalCount { get; set; }
    }
}
