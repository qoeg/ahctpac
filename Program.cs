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
        static ManualResetEvent waitObj = new ManualResetEvent(false);

        static Dictionary<int, int> dict = new Dictionary<int, int>();
        static StringBuilder buffer = new StringBuilder();

        static void Main(string[] args)
        {
            Log("In client program", true);

            try
            {
                ThreadPool.QueueUserWorkItem(Answer, null);
            }
            catch (Exception ex)
            {
                Log(ex.Message, true);
            }

            string str = string.Empty;
            while (true)
            {
                try
                {
                    char glyph = (char)Console.Read();
                    buffer.Append(glyph);

                    waitObj.Set();
                }
                catch (Exception ex)
                {
                    Log(ex.Message, true);
                }
            }
        }

        private static void Answer(object state)
        {
            while (true)
            {
                try
                {
                    Log("Waiting for question", true);
                    if (waitObj.WaitOne())
                    {
                        Thread.Sleep(1000);
                        waitObj.Reset();

                        Log("-----------------------");

                        string question = buffer.ToString();
                        buffer.Clear();

                        string answer = string.Empty;

                        Log(string.Format("Processing question: {0}", question), true);
                        if (question.Length > 0)
                        {
                            if (question.Contains("How steps"))
                            {
                                answer = Collatz(question);
                            }
                            else if (question.ToLower().Contains("color"))
                            {
                                answer = FavoriteColor();
                            }
                            else if (question.ToLower().Contains("times"))
                            {
                                answer = Multiply(question);
                            }
                            else if (question.ToLower().Contains("tell me a joke"))
                            {
                                answer = Joke();
                            }
                            else if (question.ToLower().Contains("triangle"))
                            {
                                answer = Triangle(question);
                            }
                            else if (question.ToLower().Contains("human"))
                            {
                                answer = "NO";
                            }
                            else if (question.ToLower().Contains("unscramble"))
                            {
                                answer = Unscramble(question);
                            }
                            else if (question.ToLower().Contains("streak"))
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
                catch (Exception ex)
                {
                    Log(ex.Message, true);
                }
            }
        }

        public static string Collatz(string question)
        {
            int current = 55;
            string oddExpression = string.Empty;
            string evenExpression = string.Empty;

            string pattern = "How steps does it take for ([\\d]+) to hit a number it previously hit?";
            Match match = Regex.Match(question, pattern);

            if (match.Success)
            {
                current = (int.Parse(match.Groups[1].Captures[0].Value));
                Log(string.Format("Collatz number: {0}", current), true);
            }

            int steps = 0;

            while (true)
            {
                int newNum;

                //even
                if (current % 2 == 0)
                {
                    newNum = even(current);
                }
                else //odd
                {
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

        private static string Multiply(string question)
        {
            int result = 1;
            string[] numbers = question.Split(new string[] { "What is", "times", "?" }, StringSplitOptions.RemoveEmptyEntries);

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

        private static string Unscramble(string question)
        {
            string letters = question.Replace("Unscramble the letters in the word ", "").Trim();

            return String.Concat(letters.OrderBy(c => c));

        }

        private static string Triangle(string question)
        {
            List<int> ints = parseIntsFromString(question);
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
