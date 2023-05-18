using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintLabel_New
{
    public class Bill
    {
        public List<BillHeaderData> BillHeaderData { get; set; } = new List<BillHeaderData>();
        public List<BillTranData> BillTranData { get; set; } = new List<BillTranData>();
        public List<PrinterData> PrinterData { get; set; } = new List<PrinterData>();
    }
}
