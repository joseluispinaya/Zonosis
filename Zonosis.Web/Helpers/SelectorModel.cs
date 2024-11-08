namespace Zonosis.Web.Helpers
{
    public class SelectorModel
    {
        public SelectorModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
