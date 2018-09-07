ausing System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            
            
            using (StreamWriter outputFile = new StreamWriter(Path.Combine("text1.txt")))
            {
                string path = "weight.txt";

                using (StreamReader sr = new StreamReader(path))
                {
                    //This is an arbitrary size for this example.
                    String st, st_temp;

                    Double[,,] res2d = new double[50, 50, 50];
                    Double[,] res1d = new double[50, 50];
                    Double dou_temp;

                    int tot2d = -1, tot1d = -1, a = -1, b = -1;

                    int e_index = 0;
                    int s_index = -2;

                    int flg = 0;
                    while (sr.Peek() >= 0)
                    {

                        st = sr.ReadLine();
                        String[] split_char = { " " };
                        String[] substs = st.Split(split_char, StringSplitOptions.RemoveEmptyEntries);



                        foreach (var s_str in substs)
                        {

                            //Console.WriteLine(s_str);

                            if (s_str.Contains("[["))
                            {
                                tot2d = tot2d + 1;
                                e_index = s_str.LastIndexOf('[');


                                a = a + 1;
                                flg = 2;
                                if (e_index != s_str.Length - 1)
                                {
                                    st_temp = s_str.Substring(e_index + 1);

                                    dou_temp = Convert.ToDouble(st_temp);
                                    //Console.WriteLine(dou_temp);

                                    b = b + 1;

                                    res2d[tot2d, a, b] = dou_temp;



                                }

                            }
                            else if (s_str.Contains("["))
                            {
                                if (flg == 2)
                                {
                                    a = a + 1;
                                    e_index = s_str.LastIndexOf('[');
                                    if (e_index != s_str.Length - 1)
                                    {
                                        st_temp = s_str.Substring(e_index + 1);

                                        dou_temp = Convert.ToDouble(st_temp);
                                        //Console.WriteLine(dou_temp);

                                        b = 0;

                                        res2d[tot2d, a, b] = dou_temp;

                                    }
                                    else
                                    {
                                        b = -1;
                                    }


                                }
                                else
                                {
                                    flg = 1;

                                    tot1d = tot1d + 1;
                                    e_index = s_str.LastIndexOf('[');



                                    if (e_index != s_str.Length - 1)
                                    {
                                        st_temp = s_str.Substring(e_index + 1);

                                        dou_temp = Convert.ToDouble(st_temp);



                                        a = a + 1;
                                        res1d[tot1d, a] = dou_temp;

                                    }

                                }
                            }
                            else if (s_str.Contains("]]"))
                            {

                                s_index = s_str.IndexOf("]");

                                if (s_index != 0)
                                {
                                    st_temp = s_str.Substring(0, s_index);
                                    b = b + 1;
                                    res2d[tot2d, a, b] = Convert.ToDouble(st_temp);

                                }
                                for (int i = 0; i <= a; i++)
                                {
                                    for (int j = 0; j <= b; j++)
                                    {
                                        st_temp = Convert.ToString(res2d[tot2d, i, j]);
                                        outputFile.Write(st_temp);
                                        outputFile.Write("  ");
                                    }
                                    outputFile.WriteLine(" ");
                                }
                                //outputFile.WriteLine(tot2d);
                                outputFile.WriteLine("-------------------------------");
                                a = -1;
                                b = -1;
                                flg = 0;
                            }
                            else if (s_str.Contains("]"))
                            {
                                s_index = s_str.IndexOf("]");

                                if (s_index != 0)
                                {
                                    st_temp = s_str.Substring(0, s_index);
                                    if (flg == 1)
                                    {
                                        a = a + 1;
                                        res1d[tot1d, a] = Convert.ToDouble(st_temp);
                                    }
                                    else
                                    {
                                        b = b + 1;
                                        res2d[tot2d, a, b] = Convert.ToDouble(st_temp);
                                    }
                                }
                                if (flg == 1)
                                {
                                    flg = 0;

                                    for (int i = 0; i <= a; i++)
                                    {
                                        outputFile.WriteLine(res1d[tot1d, i]);
                                    }
                                    outputFile.WriteLine("-------------------------------");
                                    a = -1;
                                }
                                b = 0;
                            }
                            else
                            {

                                if (flg == 2)
                                {
                                    b = b + 1;
                                    dou_temp = Convert.ToDouble(s_str);
                                    res2d[tot2d, a, b] = dou_temp;

                                }
                                else if (flg == 1)
                                {
                                    a = a + 1;
                                    dou_temp = Convert.ToDouble(s_str);
                                    res1d[tot1d, a] = dou_temp;
                                }
                            }
                        }


                    }
                }

            }
            Console.ReadKey();
        }
    }
}
