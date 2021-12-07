using System;
using GoodMatchLibrary;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace GoodMatchUI
{
    class Program
    {
        static void Main(string[] args)
        {

            string? name1 = "";
            string? name2 = "";
            bool valid = false;
            string? quit = "";

            //Creating a list that stores all errors.
            List<string> errors = new List<string>();

            //list for excecution times
            List<string> execution = new List<string>();

            //vairiables to get average time
            double total1 = 0, total2 = 0, average = 0;

            do
            {


                do
                {
                    Console.WriteLine("Please enter first person's name");
                    name1 = Console.ReadLine();
                    if (!string.IsNullOrEmpty(name1))
                    {

                        valid = validator(name1);
                        if (valid)
                            Console.WriteLine("OK");
                        else
                            System.Console.WriteLine("Only use alphabetical characters.");
                    }
                    else
                    {
                        Console.WriteLine("Empty input, please try again");
                    }
                } while (string.IsNullOrEmpty(name1) || !valid);

                valid = false;
                do
                {
                    Console.WriteLine("Please enter second person's name");
                    name2 = Console.ReadLine();
                    if (!string.IsNullOrEmpty(name2))
                    {

                        valid = validator(name2);
                        if (valid)
                            Console.WriteLine("OK");
                        else
                            System.Console.WriteLine("Only use alphabetical characters.");
                    }
                    else
                    {
                        Console.WriteLine("Empty input, please try again");
                    }
                } while (string.IsNullOrEmpty(name2) || !valid);

                var m = new GoodMatchLibrary.Match();
                var result = m.getMatch(name1, name2);

                int percentage;
                if (int.TryParse(result, out percentage))
                {
                    if (percentage >= 80)
                        System.Console.WriteLine($"{name1} matches {name2} {result}%, good match");
                    else
                        System.Console.WriteLine($"{name1} matches {name2} {result}%.");
                }

                System.Console.WriteLine("Do you want to enter another name pair?\nType N for No, anthing else for yes.");
                quit = Console.ReadLine();

            } while (quit != "N");


            //lists in which names of males and females will be stored
            List<string> listMale = new List<string>();
            List<string> listFemale = new List<string>();

            //try and find and read CSV file to read in data
            try
            {
                //Creating path to current directory and csv file in there.
                var Inputpath = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csv", SearchOption.AllDirectories);

                using (var reader = new StreamReader(Inputpath[0]))
                {
                    System.Console.WriteLine("reading CSV file");
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        //To break each line of CSV file at ; or ,
                        char[] delimiters = new[] { ',', ';' };
                        var values = line.Split(delimiters);

                        //See if name is male
                        if (!string.IsNullOrEmpty(values[0]))
                        {

                            if (values[1].ToLower() == "m")
                            {
                                string name = values[0].ToLower();
                                if (string.IsNullOrEmpty(name))
                                {
                                    errors.Add("Contained Null or Empty line");
                                    continue;
                                }
                                else if (!validator(name))
                                {
                                    errors.Add($"{name} had non alphabetic charcters");
                                    continue;
                                }

                                else if (listMale.Contains(name))
                                {
                                    errors.Add($"{name} was a duplicate");
                                    continue;
                                }
                                listMale.Add(name);
                            }
                            //See if person in female
                            else if (values[1].ToLower() == "f")
                            {
                                string name = values[0].ToLower();
                                if (!validator(name))
                                {
                                    errors.Add($"{name} had non alphabetic charcters");
                                    continue;
                                }
                                else if (listFemale.Contains(name))
                                {
                                    errors.Add($"{name} was a duplicate");
                                    continue;
                                }
                                listFemale.Add(name);
                            }
                            else
                                errors.Add($"{values[0]} was neither male nor female");
                        }

                    }
                    reader.Close();
                    System.Console.WriteLine("Read file successfully");
                }
            }
            //Error handeling if file not found, directory not found, file in use or other
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("Directory not found");
                errors.Add(e.Message);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File not found");
                errors.Add(e.Message);

            }
            catch (Exception e)
            {

                Console.WriteLine(e);
                errors.Add(e.Message);
            }



            //List that will store all results from call Match function
            List<GoodMatchLibrary.Match> results = new List<GoodMatchLibrary.Match>();

            //looping over male and female lists to run "Good Match" function on each pair
            foreach (var person1 in listFemale)
            {
                foreach (var person2 in listMale)
                {
                    //Stopwatch to see timelaps
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    GoodMatchLibrary.Match m = new GoodMatchLibrary.Match();
                    string result = m.getMatch(person1, person2);
                    results.Add(m);

                    watch.Stop();

                    var elapsedMs = watch.Elapsed.TotalMilliseconds;
                    execution.Add($"{person1} and {person2} execution time: {elapsedMs}");
                    double temp = 0;
                    double.TryParse(result, out temp);
                    total1 += temp;

                }
            }

            //Looping over list in reverse
            foreach (var person1 in listFemale)
            {
                foreach (var person2 in listMale)
                {
                    //Stopwatch to see timelaps
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    GoodMatchLibrary.Match m = new GoodMatchLibrary.Match();
                    string result = m.getMatch(person1, person2);
                    results.Add(m);

                    watch.Stop();

                    var elapsedMs = watch.Elapsed.TotalMilliseconds;
                    execution.Add($"{person1} and {person2} execution time: {elapsedMs}");
                    double temp = 0;
                    double.TryParse(result, out temp);
                    total2 += temp;

                }
            }

            //Sorting list in descending order
            List<GoodMatchLibrary.Match> SortedList = results.OrderByDescending(o => o.ans).ToList();


            //Path for output.txt file in current directory
            var path = Path.Combine(Directory.GetCurrentDirectory(), "output.txt");

            //Deletes old output file if it exists
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                errors.Add(e.Message);
            }


            //looping over sorted list and writing to file 
            foreach (var v in SortedList)
            {
                try
                {

                    //Convert results to int
                    int percentage;
                    if (int.TryParse(v.ans, out percentage))
                    {

                        //final output text to display and write to file
                        string createText;

                        //getting text file for output
                        using (StreamWriter sw = new StreamWriter(path, true))
                        {
                            //See if % above/equal 80
                            if (percentage >= 80)
                            {
                                char c = char.ToUpper(v.person2[0]);
                                string person2 = c + v.person2.Substring(1);

                                c = char.ToUpper(v.person1[0]);
                                string person1 = c + v.person1.Substring(1);

                                createText = $"{person2} matches {person1} {percentage}%, good match" + Environment.NewLine;
                                System.Console.WriteLine(createText);

                            }

                            else
                            {
                                char c = char.ToUpper(v.person2[0]);
                                string person2 = c + v.person2.Substring(1);

                                c = char.ToUpper(v.person1[0]);
                                string person1 = c + v.person1.Substring(1);

                                createText = $"{person2} matches {person1} {percentage}%" + Environment.NewLine;
                                System.Console.WriteLine(createText);

                            }
                            sw.WriteLine(createText);
                        }
                    }
                    else
                        continue;
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);
                }

            }



            //path for errors and runtime file
            var logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs.txt");
            System.Console.WriteLine(logPath);

            //see if file already exists and deletes it
            try
            {

                if (File.Exists(logPath))
                {
                    File.Delete(logPath);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                errors.Add(e.Message);
            }

            //Calculating average runtime
            int count = (listFemale.Count() + listMale.Count()) * 2;
            average = (total1 + total2) / count;
            execution.Add($"average score for forward and reverse was: {average}");


            //write errors and execution times to log file
            try
            {

                File.WriteAllLines(logPath, errors);
                File.AppendAllLines(logPath, execution);

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

        }




        //Validator finction to see if input only contains alphabetical characters
        static bool validator(string s)
        {
            if (s != null)
            {
                return Regex.IsMatch(s, @"^[a-zA-Z]+$");

            }
            else return false;
        }
    }


}