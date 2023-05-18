using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintLabel_New
{
    public class BillHeaderData
    {
        public int bill_no { get; set; }
        public DateTime bill_date { get; set; }
        public string bill_start_time { get; set; }
        public decimal No_Of_Pax { get; set; }
        public decimal bill_amt { get; set; }
        public string Table_No { get; set; }
        public int KOTNo { get; set; }
        public List<BillTranData> BillTransactions
        {
            get; set;
        }
        public List<PrinterData> PrinterData
        {
            get; set;
        }
    }
}
