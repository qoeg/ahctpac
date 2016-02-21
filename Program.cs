using System;
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
        static int streak = 0;
        static AutoResetEvent waitObj = new AutoResetEvent(false);

        static Dictionary<int, int> dict = new Dictionary<int, int>();
        static StringBuilder buffer = new StringBuilder();

        static void Main(string[] args)
        {
            
            bool readyToAnswer = false;

            if (File.Exists(LogPath))
            {
                File.Delete(LogPath);
            }

            ThreadPool.QueueUserWorkItem(Answer, null);

            while (true)
            {
                string str = Console.ReadLine();
                Log(str);

                if (readyToAnswer && !string.IsNullOrWhiteSpace(str))
                {
                    Log(string.Format("Appending line to buffer: {0}", str), true);
                    buffer.AppendLine(str);
                    waitObj.Set();
                    continue;
                }

                if (str.ToLower().Contains("ahctpac"))
                {
                    Log("READY TO ANSWER", true);
                    readyToAnswer = true;
                }
            }
        }

        private static void Answer(object state)
        {
            while (true)
            {
                Log("Waiting for question", true);
                if (waitObj.WaitOne())
                {
                    Thread.Sleep(1000);
                    Log("-----------------------");

                    string question = buffer.ToString();
                    buffer.Clear();

                    string answer = string.Empty;
                    string[] lines = question.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                    Log(string.Format("Processing lines: {0}", lines.Length), true);
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
                        else if(lines[0].ToLower().Contains("times"))
                        {
                            answer = Multiply(lines);
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
                        else if (lines[0].ToLower().Contains("unscramble"))
                        {
                            answer = Unscramble(lines);
                        }
                        else if(lines[0].ToLower().Contains("streak"))
                        {
                            answer = streak.ToString();
                        }
                        else
                        {
                            answer = "IDK";
                        }

                        Log(string.Format("A: {0}", answer));
                        Log("-----------------------");

                        if (!string.IsNullOrWhiteSpace(answer))
                        {
                            Console.Out.Write(answer + "\n");
                            streak++;

                        }
                    }
                }
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

                if (dict.ContainsKey(newNum))
                {
                    break;
                }
                else
                {
                    steps++;
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

        private static string Multiply(string[] lines)
        {
            int result = 1;
            string[] numbers = lines[0].Split(new string[] {"What is", "times", "?" }, StringSplitOptions.RemoveEmptyEntries);

            foreach(string number in numbers)
            {
                string num = number.Trim();
                if(!string.IsNullOrWhiteSpace(num))
                {
                    int multiplier = 1;
                    if(int.TryParse(num, out multiplier))
                    {
                        result *= multiplier;
                    }
                }
            }

            return result.ToString();
        }

        private static string Unscramble(string[] lines)
        {
            string letters = lines[0].Replace("Unscramble the letters in the word ", "").Trim();

            return String.Concat(letters.OrderBy(c => c));

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

        private static void Log(string line, bool debug = false)
        {
            string prefix = debug ? "{DEBUG} " : string.Empty;
            File.AppendAllText(LogPath, prefix + line + Environment.NewLine);
        }
    }
}
