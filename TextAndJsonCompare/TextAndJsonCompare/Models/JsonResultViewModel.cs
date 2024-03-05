using System.Collections.Generic;

namespace TextAndJsonCompare.Models
{
    public class JsonResultViewModel
    {
        public string Json1 { get; set; }
        public string Json2 { get; set; }
        public List<DifferenceObject> Differences { get; set; }
    }
}
