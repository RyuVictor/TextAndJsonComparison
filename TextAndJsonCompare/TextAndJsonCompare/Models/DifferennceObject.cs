namespace TextAndJsonCompare.Models
{
    public class DifferenceObject
    {
        public string PropertyName { get; set; }
        public object Json1Value { get; set; }
        public object Json2Value { get; set; }
    }
}
