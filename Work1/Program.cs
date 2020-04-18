using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Work1
{
    class CompareD
    {
        static int Main(string[] args)
        {
            if(args.Length != 3)
            {
                Console.WriteLine("Usage: Work1.exe Path1 Path2 PathToResultFile");
                Console.ReadLine();
                return 1;
            }

            string d1, d2, res;
            
            d1 = args[0];
            d2 = args[1];
            res = args[2];
            
            try
            {
                Directory.GetFiles(d1);
                Directory.GetFiles(d2);
                using(var fs = new FileInfo(res).CreateText())
                { }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error: " + e.Message);
                return 1;
            }
            catch (ArgumentException e1)
            {
                Console.WriteLine("Error: " + e1.Message);
                return 1;
            }
            catch (UnauthorizedAccessException e2)
            {
                Console.WriteLine("Error: " + e2.Message);
                return 1;
            }
            
            System.IO.DirectoryInfo dir1 = new DirectoryInfo(d1); // Creating method for acting with directories on the specified path
            System.IO.DirectoryInfo dir2 = new DirectoryInfo(d2); // The same thing but for the 2nd path

            IEnumerable<System.IO.FileInfo> lst1 = dir1.GetFiles("*.*", SearchOption.AllDirectories); // "Capturing" file paths and directories in enumerator, which allows navigation between dirs and files
            IEnumerable<System.IO.FileInfo> lst2 = dir2.GetFiles("*.*", SearchOption.AllDirectories); // The same thing for the 2nd path
           
            FileCompare fc = new FileCompare(); // Creating a new FileCompare object

            bool Identity = lst1.SequenceEqual(lst2, fc); // Checking if item in lst1 exists in lst2 by fc criteria
            if (Identity == true) // Making the result
            {
                Console.WriteLine("The folders are seems to be the same");
            }
            if (Identity == false) // 2nd part
            {
                Console.WriteLine("The folders aren't the same");

                var queryDF = (from file in lst1 select file).Except(lst2, fc); // Creating a query to select files from lst1(1st dir and its subdirs) which are not existing in lst2(2nd dir and its subdirs)
                var fw = new FileInfo(res); // Creating a FileInfo variable which allows us to interact with files

                using (StreamWriter sw = fw.CreateText()) // Making output in file using StreamWriter class
                {
                    sw.WriteLine("Following files are existing in dir1 and not existing in dir2: ");
                    foreach (var v in queryDF) // Enumerating all items which has been given us in query
                    {
                        sw.WriteLine(v.FullName); // Writing their pathes in the result file
                    }
                    sw.Close();
                }
            }
           
            Console.ReadLine();
            return 0;
        }
    }

    class FileCompare : IEqualityComparer<FileInfo> // Making a comparator class
    {
        public FileCompare()
        {

        }

        public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2) // This method will return "True" if a name of f1 and its size equals to f2
        {
            return (f1.Name == f2.Name && f1.Length == f2.Length);
        }

        public int GetHashCode(System.IO.FileInfo fi) // Returning a hashcode that reflects the comparison
        {
            string s = $"{fi.Name}{fi.Length}";
            return s.GetHashCode();
        }
    }
}