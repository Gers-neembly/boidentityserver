namespace Neembly.BOIDServer.SharedClasses
{
    public class UACMenuInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsMenuItem { get; set; }
        public bool IsMenuHeader { get; set; }
        public string ParentItemCode { get; set; }
    }
}
