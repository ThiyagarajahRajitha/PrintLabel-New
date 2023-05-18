using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintLabel_New
{
    public class DataAccess
    {
        public async Task<Bill> GetAllData()
        {
            var bill = new Bill();
            int clrk_code = Convert.ToInt32(ConfigurationManager.AppSettings["clrk_code"]);
            var connectionString = ConfigurationManager.ConnectionStrings["MSSql"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(
                    @"select bill_No, bill_date,bill_start_time, No_Of_Pax ,bill_amt,Table_No,clrk_code,cCode,bill_end_time from bill_header WHERE IsPrinted = 0 AND clrk_code =" + clrk_code, conn);

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        var newObject = new BillHeaderData();
                        reader.MapDataToObject(newObject);
                        bill.BillHeaderData.Add(newObject);
                    }
                }

                if(bill.BillHeaderData.Count() == 0)
                {
                    return new Bill();
                }

                var sqlQueryDelivery = @"select * from [dbo].[delivery_moreAddress] where ccode in (@0)";


                var ccode = bill.BillHeaderData.Select(p => p.cCode).ToList();

                var codeString = string.Join(",", ccode.ToList());

                sqlQueryDelivery = sqlQueryDelivery.Replace("@0", codeString);

                SqlCommand commandDel = new SqlCommand(sqlQueryDelivery, conn);

                using (SqlDataReader reader = commandDel.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var newObject = new DeliveryMoreAddress();
                        reader.MapDataToObject(newObject);
                        bill.DeliveryMoreAddress.Add(newObject);
                    }
                }

                var sqlQuery = @"select * from [dbo].[bill_tran] where Printer_No <> '0' AND IsPrinted = 0 AND bill_no in (@0) and bill_date in (@1)";

                var billNos = bill.BillHeaderData.Select(p => p.bill_no).ToList();
                var dates = bill.BillHeaderData.Select(p => p.bill_date).Distinct().ToList();

                var numbersString = string.Join(",", billNos.ToList());
                var dateString = string.Join(",", dates.Select(p => String.Format("'{0}'", p)));

                sqlQuery = sqlQuery.Replace("@0", numbersString)
                                   .Replace("@1", dateString);

                SqlCommand command1 = new SqlCommand(sqlQuery, conn);

                using (SqlDataReader reader = command1.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var newObject = new BillTranData();
                        reader.MapDataToObject(newObject);
                        newObject.formatted_Printer_No = newObject.Printer_No.Split(';').ToList();
                        bill.BillTranData.Add(newObject);
                    }
                }


                var sqlQueryToPrinter = @"select * from [dbo].[prn_mast] where prn_no in (@0)";

                List<String> allPrinters = new List<string>();

                foreach (var trans in bill.BillTranData)
                {
                    allPrinters = allPrinters.Concat(trans.formatted_Printer_No).ToList();

                }

                allPrinters = allPrinters.Distinct().ToList();
                var printNumbersString = "";

                printNumbersString = string.Join(",", allPrinters.ToList());


                sqlQueryToPrinter = sqlQueryToPrinter.Replace("@0", printNumbersString);

                SqlCommand commandprint = new SqlCommand(sqlQueryToPrinter, conn);

                using (SqlDataReader reader = commandprint.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var newObject = new PrinterData();
                        reader.MapDataToObject(newObject);
                        bill.PrinterData.Add(newObject);
                    }
                }
            }
            return bill;

        }

        public async Task UpdateSourceDatabase(string tableName, string columnName)
        {
            //Connection string for MS SQL Server
            string sourceConnectionString = ConfigurationManager.ConnectionStrings["MSSql"].ConnectionString;
            using (SqlConnection sourceConnection = new SqlConnection(sourceConnectionString))
            {
                sourceConnection.Open();
                SqlCommand command = new SqlCommand("UPDATE " + tableName + " SET IsPrinted = 1 WHERE bill_no = " + columnName, sourceConnection);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
