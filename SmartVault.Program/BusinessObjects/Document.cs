using System.Xml.Linq;

namespace SmartVault.Program.BusinessObjects
{
    public partial class Document
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string FilePath { get; set; }
    }
}
