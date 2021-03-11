using Newtonsoft.Json;

namespace DevExpressGrid.Extensions.Models
{
    public class FilterRequest
    {
        public int skip { get; set; }
        public int take { get; set; }
        public bool requireTotalCount { get; set; }
        public string sort { get; set; }
        public string group { get; set; }
        public string filter { get; set; }
        public SortModel[] _sort => !string.IsNullOrWhiteSpace(sort) ? JsonConvert.DeserializeObject<SortModel[]>(sort) : null;
        public GroupModel[] _group => !string.IsNullOrWhiteSpace(group) ? JsonConvert.DeserializeObject<GroupModel[]>(group) : null;
    }

    public class SortModel
    {
        public string selector { get; set; }
        public bool desc { get; set; }
    }
    public class GroupModel
    {
        public string selector { get; set; }
        public bool isExpanded { get; set; }
    }
}
