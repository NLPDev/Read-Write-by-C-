using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace conflictmatrix
{
    /// <summary>
    /// Class to store one CSV row
    /// </summary>
    public class CsvRow : List<string>
    {
        public string LineText { get; set; }
    }

    /// <summary>
    /// Class to write data to a CSV file
    /// </summary>
    public class CsvFileWriter : StreamWriter
    {
        public CsvFileWriter(Stream stream)
            : base(stream)
        {
        }

        public CsvFileWriter(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Writes a single row to a CSV file.
        /// </summary>
        /// <param name="row">The row to be written</param>
        public void WriteRow(CsvRow row)
        {
            StringBuilder builder = new StringBuilder();
            bool firstColumn = true;
            foreach (string value in row)
            {
                // Add separator if this isn't the first value
                if (!firstColumn)
                    builder.Append(',');
                // Implement special handling for values that contain comma or quote
                // Enclose in quotes and double up any double quotes
                if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                else
                    builder.Append(value);
                firstColumn = false;
            }
            row.LineText = builder.ToString();
            WriteLine(row.LineText);
        }
    }

    /// <summary>
    /// Class to read data from a CSV file
    /// </summary>
    public class CsvFileReader : StreamReader
    {
        public CsvFileReader(Stream stream)
            : base(stream)
        {
        }

        public CsvFileReader(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Reads a row of data from a CSV file
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool ReadRow(CsvRow row)
        {
            row.LineText = ReadLine();
            if (String.IsNullOrEmpty(row.LineText))
                return false;

            int pos = 0;
            int rows = 0;

            while (pos < row.LineText.Length)
            {
                string value;

                // Special handling for quoted field
                if (row.LineText[pos] == '"')
                {
                    // Skip initial quote
                    pos++;

                    // Parse quoted value
                    int start = pos;
                    while (pos < row.LineText.Length)
                    {
                        // Test for quote character
                        if (row.LineText[pos] == '"')
                        {
                            // Found one
                            pos++;

                            // If two quotes together, keep one
                            // Otherwise, indicates end of value
                            if (pos >= row.LineText.Length || row.LineText[pos] != '"')
                            {
                                pos--;
                                break;
                            }
                        }
                        pos++;
                    }
                    value = row.LineText.Substring(start, pos - start);
                    value = value.Replace("\"\"", "\"");
                }
                else
                {
                    // Parse unquoted value
                    int start = pos;
                    while (pos < row.LineText.Length && row.LineText[pos] != ',')
                        pos++;
                    value = row.LineText.Substring(start, pos - start);
                }

                // Add field to list
                if (rows < row.Count)
                    row[rows] = value;
                else
                    row.Add(value);
                rows++;

                // Eat up to and including next comma
                while (pos < row.LineText.Length && row.LineText[pos] != ',')
                    pos++;
                if (pos < row.LineText.Length)
                    pos++;
            }
            // Delete any unused items
            while (row.Count > rows)
                row.RemoveAt(rows);

            // Return true if any columns read
            return (row.Count > 0);
        }
    }


    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello World!");
            ReadTest();

        }


        public static void ReadTest()
        {
            // Read sample data from CSV file
            using (CsvFileReader reader = new CsvFileReader("ReadTest1.csv"))
            {
                using (CsvFileWriter writer = new CsvFileWriter("WriteTest1_teacher.csv"))
                {

                    int course_num = -1;
                    int student_num = -1;
                    int teacher_num = -1;
                    int grade_num = -1;
                    List<string> course = new List<string>();
                    List<int>[] stu = new List<int>[10000];
                    List<int> stu_id = new List<int>();
                    int N = 500;
                    int[,] res = new int[N, N];

                    for (int i = 0; i < N;i++)
                    {
                        for (int j = 0; j < N;j++)
                        {
                            res[i, j] = 0;
                        }
                    }

                    for (int i = 0; i < 10000;i++)
                    {
                        stu[i] = new List<int>();
                    }

                    CsvRow row = new CsvRow();
                    int cnt_rw = 0;
                    while (reader.ReadRow(row))
                    {
                        if(cnt_rw==0)
                        {

                            int cnt = 0;
                            foreach (string s in row)
                            {
                                if(s=="Course_Section")
                                {
                                    course_num = cnt;
                                }
                                if(s=="Student_Number")
                                {
                                    student_num = cnt;
                                }
                                if(s=="Teacher_Name")
                                {
                                    teacher_num = cnt;
                                }
                                if(s=="Course_Grade")
                                {
                                    grade_num = cnt;
                                }
                                cnt++;
                            }


                        }
                        else
                        {
                            int cc = 0;
                            int cur = 0;

                            string st_course = "";
                            string st_grade = "";
                            foreach (string s in row)
                            {

                                if(cc==student_num)
                                {
                                    int stu_num = -1;
                                    stu_num = Convert.ToInt32(s);
                                    bool flag = false;
                                    int nni = 0;
                                    foreach(int nn in stu_id)
                                    {
                                        if(nn==stu_num)
                                        {
                                            cur = nni;
                                            flag = true;
                                        }
                                        nni++;
                                    }
                                    if(flag==false)
                                    {
                                        stu_id.Add(stu_num);
                                        cur = nni;
                                    }
                                }
                                if(cc==course_num)
                                {
                                    bool flag = false;
                                    int cou = 0;
                                    foreach(string st in course)
                                    {
                                        if(st==s)
                                        {
                                            flag = true;
                                            stu[cur].Add(cou);
                                        }
                                        cou++;
                                    }
                                    if(flag==false)
                                    {

                                        stu[cur].Add(cou);
                                        course.Add(s);
                                    }
                                }
                                //if(cc==course_num)
                                //{
                                //    st_course = s;
                                //}
                                //if(cc==grade_num)
                                //{
                                //    st_grade = s;
                                //}
                                //if(cc==teacher_num)
                                //{
                                //    string[] names = s.Split(',');
                                //    st_course = st_course +"-"+ names[0] +"-"+ names[1]+"("+st_grade+" Grade)";
                                //    bool flag = false;
                                //    int cou = 0;
                                //    foreach(string st in course)
                                //    {
                                //        if(st==st_course)
                                //        {
                                //            flag = true;
                                //            stu[cur].Add(cou);
                                //        }
                                //        cou++;
                                //    }
                                //    if(flag==false)
                                //    {

                                //        stu[cur].Add(cou);
                                //        course.Add(st_course);
                                //    }
                                //}
                                cc++;
                            }
                            //writer.WriteRow(rr);
                        }

                        cnt_rw++;
                        //Console.WriteLine();
                    }

                    //conflict matrix

                    int cur_stu = 0;
                    foreach(int aa in stu_id)
                    {
                        for (int i = 0; i < stu[cur_stu].Count;i++)
                        {
                            for (int j = i+1; j < stu[cur_stu].Count; j++)
                            {
                                res[stu[cur_stu][i], stu[cur_stu][j]]++;
                                res[stu[cur_stu][j], stu[cur_stu][i]]++;
                            }
                        }

                        cur_stu++;
                    }

                    CsvRow ww = new CsvRow();
                    ww.Add("Course(Teacher)");

                    for (int i = 0; i < course.Count; i++)
                    {
                        ww.Add(course[i]);
                    }

                    writer.WriteRow(ww);
                    for (int i = 0; i < course.Count; i++)
                    {
                        CsvRow wwcourse = new CsvRow();
                        wwcourse.Add(course[i]);
                        for (int j = 0; j < i;j++)
                        {
                            wwcourse.Add(" ");
                        }
                        wwcourse.Add("X");
                        for (int j = i+1; j < course.Count;j++)
                        {
                            wwcourse.Add(Convert.ToString(res[i, j]));
                        }
                        writer.WriteRow(wwcourse);

                    }

                }

            }
        }
    }
}

