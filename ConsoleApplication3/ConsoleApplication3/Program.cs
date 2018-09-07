using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConsoleApplication3
{
    class Program
    {
        //private static object txtAll;

        static void Main(string[] args)
        {
            string folderPath = @"D:\folder";

            string rest1 = "D:\\res\\res_ex";
            string rest2 = ".svg";

            int num = 0;

            foreach (string file in Directory.EnumerateFiles(folderPath, "*.svg"))
            {
                string contents = File.ReadAllText(file);

                
                num = num + 1;

                string rr;
                rr = rest1 + num.ToString() + rest2;

                Console.WriteLine(file);
                string pathres = rr;

                pathres = file;

                
                if (!File.Exists(pathres))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(pathres))
                    {
                        sw.WriteLine("");
                    }
                }

                // Open the file to read from.
                string path = @"D:\ex2.svg";

                using (StreamWriter sw = new StreamWriter(pathres))
                {

                    using (StreamReader sr = File.OpenText(path))
                    {
                        string s = "";
                        s = sr.ReadLine();

                        string[] split_char = { " " };
                        string[] subsets = s.Split(split_char, StringSplitOptions.RemoveEmptyEntries);

                        bool flag = false;

                        foreach (var s_str in subsets)
                        {
                            sw.Write(s_str);
                            sw.Write(" ");

                            if (flag == false)
                            {
                                sw.Write("fill=\"#F9F9F9\" ");
                                flag = true;
                            }
                        }
                        sw.WriteLine("");
                        while ((s = sr.ReadLine()) != null)
                        {
                            sw.WriteLine(s);
                        }
                    }
                }
            }

            
            Console.ReadKey();
        }
    }
}
