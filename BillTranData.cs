using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintLabel_New
{
    public class BillTranData
    {
        public decimal tran_qty { get; set; }
        public string tran_desc { get; set; }
        public string tran_type { get; set; }
        public int bill_no { get; set; }
        public DateTime bill_date { get; set; }
        public string Printer_No { get; set; }
        public int KOTNo { get; set; }
        public List<string> formatted_Printer_No { get; set; }
        public List<PrinterData> PrinterData
        {
            get; set;
        }
    }
}
