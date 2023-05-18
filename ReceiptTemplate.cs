using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintLabel_New
{
    public class ReceiptTemplate
    {
        public string PrinterName { get; set; }
        BillHeaderData printData;
        bool isWithPrinterName = Convert.ToBoolean(ConfigurationManager.AppSettings["isWithPrinterName"]);


        public ReceiptTemplate(BillHeaderData billHeaderData)
        {
            printData = billHeaderData;
        }
        public void print(string printerName)
        {
            PrinterName = printerName;
            if (isWithPrinterName)
            {

                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = printerName;
                pd.PrintPage += new PrintPageEventHandler(GenerateTemplate);
                pd.Print();
            }
            else
            {

                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler(GenerateTemplate);
                pd.Print();
            }
        }
        public void GenerateTemplate(Object Sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics graphic = e.Graphics;


            int fonsizeSmall = Convert.ToInt32(ConfigurationManager.AppSettings["fonsizeSmall"]);
            int fontsizeLarge = Convert.ToInt32(ConfigurationManager.AppSettings["fontsizeLarge"]);
            string takeAway = Convert.ToString(ConfigurationManager.AppSettings["takeAway"]);
            bool isWithCustomerName = Convert.ToBoolean(ConfigurationManager.AppSettings["isWithCustomerName"]);

            Font font = new Font("Courier New", fonsizeSmall, FontStyle.Bold);
            float fontHeight = font.GetHeight();

            int startX = 5;
            int startY = 10;
            int offset = 10;

            offset = offset + (int)fontHeight + 10;

            graphic.DrawString("" + PrinterName, new Font("Courier New", fonsizeSmall), new SolidBrush(Color.Black), startX, startY + offset);


            graphic.DrawString("                   KOT No:" + printData.KOTNo, new Font("Courier New", fonsizeSmall), new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + (int)fontHeight + 7;

            graphic.DrawString("------------------------------------", new Font("Courier New", fonsizeSmall, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + (int)fontHeight;

            graphic.DrawString("" + takeAway + printData.Table_No, new Font("Courier New", fontsizeLarge, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + (int)fontHeight + 7;

            graphic.DrawString("Bill No. : " + printData.bill_no + "      Pax : " + printData.No_Of_Pax, new Font("Courier New", fontsizeLarge, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + (int)fontHeight + 7;

            graphic.DrawString("Served By : SMART", new Font("Courier New", fonsizeSmall), new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + (int)fontHeight + 7;

            graphic.DrawString(" " + printData.bill_date.ToString("dd-MMM-yyyy") + "         " + printData.bill_start_time, new Font("Courier New", fonsizeSmall), new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + (int)fontHeight + 7;


            graphic.DrawString("------------------------------------", new Font("Courier New", fonsizeSmall), new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + 15;
            graphic.DrawString("QTY    DESCRIPTION", new Font("Courier New", fonsizeSmall), new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + 12;
            graphic.DrawString("------------------------------------", new Font("Courier New", fonsizeSmall), new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + 15;
            var index = 1;
            foreach (var item in printData.BillTransactions)
            {
                if (item.tran_type != "A")
                {
                    graphic.DrawString("" + item.tran_qty.ToString("0", CultureInfo.InvariantCulture) + "      " + item.tran_desc, new Font("Courier New", fonsizeSmall), new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + 20;
                }


            }
            foreach (var tran in printData.BillTransactions)
            {
                if (tran.tran_type == "A")
                {
                    graphic.DrawString("      +++" + tran.tran_desc, new Font("Courier New", fonsizeSmall), new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + 20;
                }
            }


            graphic.DrawString("------------------------------------", new Font("Courier New", fonsizeSmall, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + 12;
            graphic.DrawString("Total Qty. " + printData.BillTransactions.Count(x => x.tran_type != "A"), new Font("Courier New", fonsizeSmall), new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + (int)fontHeight + 7;

            if (isWithCustomerName)
            {
                offset = offset + 8;
                graphic.DrawString("Name: " + printData.DeliveryMoreAddress.FirstOrDefault().Cfname, new Font("Courier New", fonsizeSmall), new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 7;

                graphic.DrawString("Tel: " + printData.DeliveryMoreAddress.FirstOrDefault().cPhone, new Font("Courier New", fonsizeSmall), new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 7;
            }
            graphic.DrawString("Pick up time : " + printData.bill_end_time, new Font("Courier New", fonsizeSmall), new SolidBrush(Color.Black), startX, startY + offset);

            offset = offset + 15;

            graphic.DrawString("***NEW***", new Font("Courier New", fonsizeSmall), new SolidBrush(Color.Black), startX, startY + offset);
            offset = offset + 10;

        }
    }
}
