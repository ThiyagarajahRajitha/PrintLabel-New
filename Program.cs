using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintLabel_New
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }
        static async Task MainAsync()
        {

            try
            {
                DataAccess dataAccessService = new DataAccess();

                var billdata = await dataAccessService.GetAllData();

                foreach (var header in billdata.BillHeaderData)
                {
                    header.BillTransactions = billdata.BillTranData.Where(p => p.bill_no == header.bill_no && p.bill_date == header.bill_date)
                    .ToList();
                    header.PrinterData = new List<PrinterData>();

                    foreach (var transaction in billdata.BillTranData)
                    {
                        var pr = transaction.formatted_Printer_No;
                        foreach (var printerNo in pr)
                        {
                            var printDaata = billdata.PrinterData
                            .Where(p => p.prn_no == printerNo).FirstOrDefault();
                            if (!(header.PrinterData.Contains(printDaata)))
                            {
                                header.PrinterData.Add(printDaata);
                            }

                        }

                    }
                    if (header.BillTransactions.Count > 0)
                    {
                        header.KOTNo = header.BillTransactions.FirstOrDefault().KOTNo;
                    }
                }

                foreach (var bill in billdata.BillHeaderData)
                {

                    if (bill.BillTransactions.Count > 0)
                    {

                        ReceiptTemplate receiptTemplate = new ReceiptTemplate(bill);
                        foreach (var printer in bill.PrinterData)
                        {
                            receiptTemplate.print(printer.prn_name);

                        }

                        var connectionString = ConfigurationManager.ConnectionStrings["MSSql"].ConnectionString;
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            await dataAccessService.UpdateSourceDatabase("bill_header", bill.bill_no.ToString());
                            await dataAccessService.UpdateSourceDatabase("bill_tran", bill.bill_no.ToString());
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Message : " + ex.Message);
                Console.WriteLine("InnerException : " + ex.InnerException);
                
            }
        }
    }
}
