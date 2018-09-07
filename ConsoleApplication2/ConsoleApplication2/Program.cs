using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Runtime.InteropServices;

using Excel = Microsoft.Office.Interop.Excel;


namespace ConsoleApplication2
{
    class Program
    {
        public static double[,] in_data = new double[100, 20];
        public static int[] in_year = new int[100];
        public static int tot_year;

        public static double[,] pre_data = new double[100, 20];

        static void Main(string[] args)
        {
            getExcelFile();
            
            tot_year = 5;

            double[] x = new double[20];

            double[,] xs = new double[100,20];
            double[,] ys = new double[100,20];
            double[,] x2s = new double[100,20];
            double[,] xys = new double[100,20];

            double[,] b1 = new double[100,20];
            double[,] b0 = new double[100,20];

            double[,] yt = new double[100,20];
            
            double[,] CyRltve = new double[100,20];

            double[] yt2 = new double[20];
            
            for(int i = 0; i < 20; i++)
            {
                x[i] = i;
            }

            int start_year = 5;
            int months = 12;

            double av_xs, av_ys;

            for (int i = 0; i < months; i++)
            {
                xs[start_year, i] = 0;
                ys[start_year, i] = 0;
                x2s[start_year, i] = 0;
                xys[start_year, i] = 0;
                for(int j = 0; j < tot_year; j++)
                {
                    xs[start_year,i] = xs[start_year, i] + x[j];
                    ys[start_year, i] = ys[start_year, i] + in_data[j, i];
                    x2s[start_year, i] = x2s[start_year, i] + x[j] * x[j];
                    xys[start_year, i] = xys[start_year, i] + x[j] * in_data[j, i];
                }

                av_xs = xs[start_year, i] / tot_year;
                av_ys = ys[start_year, i] / tot_year;

                
                b1[start_year, i] = (xys[start_year, i] - av_xs * av_ys*tot_year) / (x2s[start_year,i] -av_xs*av_xs*tot_year);
                b0[start_year, i] = av_ys - (b1[start_year, i] * av_xs);
                yt[start_year, i] = b0[start_year, i] + b1[start_year, i] * tot_year;
                CyRltve[start_year, i] = 100.0 * in_data[start_year, i] / yt[start_year, i];

                pre_data[start_year, i] = yt[start_year, i];
                
            }

            //predict
            
            
            int res = 10;

            for (int j = start_year+1; j <=res; j++)
            {
                                
                for(int i = 0; i < months; i++)
                {
                    xs[j, i] = xs[j - 1, i] + x[j-1];
                    ys[j, i] = ys[j - 1, i] + in_data[j-1, i];
                    x2s[j, i] = x2s[j - 1, i] + x[j-1] * x[j-1];
                    xys[j, i] = xys[j - 1, i] + x[j-1] * in_data[j-1, i];

                    av_xs = xs[j, i] / j;
                    av_ys = ys[j, i] / j;

                    

                    b1[j, i] = (xys[j,i] - av_xs * av_ys*j) / (x2s[j,i] - av_xs * av_xs * j);
                    b0[j,i] = av_ys - (b1[j,i] * av_xs);
                    yt[j,i] = b0[j,i] + b1[j,i] * j;
                    if (j < res)
                    {
                        CyRltve[j, i] = 100.0 * in_data[j, i] / yt[j, i];
                    }
                                        
                }

            }

            //res predict

            double[] sum_cy = new double[20];
            for(int i = 0; i < months; i++)
            {
                sum_cy[i] = 0;
                for(int j = start_year; j < res; j++)
                {
                    sum_cy[i] = sum_cy[i] + CyRltve[j, i];
                }
                yt2[i] = yt[res,i] + sum_cy[i] / tot_year / 100.0;
            }


            //create
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            
            //Excel            
            xlWorkSheet.Cells[1,1] = "year/Mon";
            for (int i = 0; i < 10; i++)
            {
                xlWorkSheet.Cells[i + 2, 1] = 2008 + i; 
                for (int j = 0; j < 12; j++)
                {
                    xlWorkSheet.Cells[i+2,j+2] = in_data[i,j];
                }
            }
            for(int i = 0; i < 12; i++)
            {
                xlWorkSheet.Cells[1, i + 2] = i + 1;
            }

            xlWorkSheet.Cells[15, 1] = "Predict";
            int start_row = 16;
            for(int i = 0; i < 6; i++)
            {
                xlWorkSheet.Cells[start_row + i, 1] = 2013 + i;
                for (int j = 0; j < 12; j++)
                {
                    xlWorkSheet.Cells[start_row + i, j + 2] = yt[start_year + i, j];
                }
            }

            xlWorkSheet.Cells[24, 1] = "YT2";
            for(int i = 0; i < 12; i++)
            {
                xlWorkSheet.Cells[24, 2 + i] = yt2[i];
            }

            xlWorkBook.SaveAs("d:\\ex2.csv");
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);


            Console.ReadKey();

        }
        public static void getExcelFile()
        {

            //Create COM Objects. Create a COM object for everything that is referenced
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"D:\Grocery.csv");
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

       
            
            for(int i = 0; i < rowCount-1; i++)
            {
                in_year[i] = Convert.ToInt32(xlRange.Cells[i + 2, 1].Value2);
                for (int j = 0; j < colCount-1; j++)
                {
                     
                    in_data[i, j] = Convert.ToDouble(xlRange.Cells[i + 2, j + 2].Value2);
                }
            }

            tot_year = xlRange.Rows.Count - 1;

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

           
        }
    }
}
