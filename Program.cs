﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OddEven
{
    /*
    "x -> x/2 if x is even" + Environment.NewLine +

    "x -> 3x-1 if x is odd" + Environment.NewLine +

    "Test case: 4->2 because it's even, 9->26 because it is odd." + Environment.NewLine +

    "How steps does it take for 68 to hit a number it previously hit?";
     */

    class Program
    {
        const string LogPath = @"C:\Test\ahctpac.log";

        static AutoResetEvent waitObj = new AutoResetEvent(false);

        static Dictionary<int, int> dict = new Dictionary<int, int>();
        static StringBuilder buffer = new StringBuilder();

        static void Main(string[] args)
        {
            if (File.Exists(LogPath))
            {
                File.Delete(LogPath);
            }

            ThreadPool.QueueUserWorkItem(Answer, null);

            while (true)
            {
                string str = Console.ReadLine();
                buffer.AppendLine(str);

                waitObj.Set();
            }
        }

        private static void Answer(object state)
        {
            while (true)
            {
                if (waitObj.WaitOne())
                {
                    Thread.Sleep(1000);

                    string question = buffer.ToString();

                    File.AppendAllText(LogPath, question + Environment.NewLine);
                    File.AppendAllText(LogPath, "-----------------------" + Environment.NewLine);

                    string answer = string.Empty;
                    string[] lines = question.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                    if (lines.Length > 0)
                    {
                        if (lines[0].Contains("Collatz"))
                        {
                            answer = Collatz(lines);
                        }
                        else if (lines[0].ToLower().Contains("color"))
                        {
                            answer = FavoriteColor();
                        }
                        else if (lines[0].ToLower().Contains("tell me a joke"))
                        {
                            answer = Joke();
                        }
                        else if (lines[0].ToLower().Contains("triangle"))
                        {
                            answer = Triangle(lines);
                        }
                        else if (lines[0].ToLower().Contains("human"))
                        {
                            answer = "NO";
                        }
                        else
                        {
                            answer = "IDK";
                        }
                    }

                    File.AppendAllText(LogPath, string.Format("A: {0}", answer + Environment.NewLine));
                    File.AppendAllText(LogPath, "-----------------------" + Environment.NewLine);

                    Console.Out.WriteLine(answer);
                }

                buffer.Clear();
            }
        }

        public static string Collatz(string[] lines)
        {
            int current = 55;
            string oddExpression = string.Empty;
            string evenExpression = string.Empty;
            foreach (string line in lines)
            {
                if(line.ToLower().Contains("hit a number it previously hit"))
                {
                    current = (int.Parse(line.Substring(27, line.IndexOf(" to hit a number it previously hit") - 27)));
                }
            }

            int steps = 0;

            while (true)
            {
                int newNum;

                //even
                if (current % 2 == 0)
                {
                    //var result = new DataTable().Compute(evenExpression, null);
                    newNum = even(current);
                }
                else //odd
                {
                    //var result = new DataTable().Compute(oddExpression, null);
                    newNum = odd(current);
                }

                steps++;

                if (dict.ContainsKey(newNum))
                {
                    break;
                }
                else
                {
                    dict.Add(newNum, newNum);
                    current = newNum;
                }
            }

            return steps.ToString();
        }

        private static string FavoriteColor()
        {
            return "AMBER";
        }

        private static string Joke()
        {
            return "WHAT IS A JOKE?";
        }

        private static string Triangle(string[] lines)
        {
            List<int> ints = parseIntsFromString(lines[0]);
            return  Math.Round(AreaOfTriangle(ints[0], ints[1], ints[2]), ints[3]).ToString();
        }

        public static List<int> parseIntsFromString(String s)
        {
            List<int> ints = new List<int>();
            Regex re = new Regex(@"\d+");
            foreach (Match m in re.Matches(s))
            {
                int i;
                if (int.TryParse(m.Value, out i))
                    ints.Add(i);
            }
            return ints;
        }

        public static double AreaOfTriangle(int a, int b, int c)
        {
            double p = (a + b + c) / 2.0;
            return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }

        private static int even(int current)
        {
            return current / 2;
        }

        private static int odd(int current)
        {
            return current * 3 - 1;
        }
    }
}