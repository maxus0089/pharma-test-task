namespace PharmStd.Data.Models
{
    public class Drug
    {
        
        public int DrugId { get; set; }
        public string Name { get; set; }
        public string[] PharmGroup { get; set; }
        public string ActiveComponent{ get; set; }
        public string AtxCode { get; set; }
        public bool RxOnly { get; set; }
        public string Category { get; set; }
    }
}