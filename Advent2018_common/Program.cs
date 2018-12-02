using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent2018_common
{
    class MainClass
    {
        // HashSet
        private static Dictionary<char, int> letterCount(string input)
        {
            var result = new Dictionary<char, int>();
            for (int i=0; i< input.Length; i++)
            {
                if (result.ContainsKey(input[i])) {
                    result[input[i]] += 1;
                }
                else
                {
                    result[input[i]] = 1;
                }
            }
            return result;
        }

        private static string diffMedEtt(string a, string[] lines)
        {
            // Jämför med alla andra rader, tecken för tecken
            foreach (var b in lines)
            {
                int diffs = 0;
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i] != b[i])
                    {
                        diffs++;
                    }
                }
                if (diffs == 1)
                {
                    Console.WriteLine($"Hittade diff med ett mellan {a} och {b}");
                    string result = "";
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (a[i] == b[i])
                        {
                            result += a[i];
                        }
                    }
                    return result;
                }
            }
            return "";
        }

        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");
            int tvåor = 0;
            int treor = 0;

            foreach (var line in input)
            {
                var bokstäver = letterCount(line);
                bool settTvå = false;
                bool settTre = false;
                foreach (var b in bokstäver)
                {
                    if (b.Value == 2 && !settTvå)
                    {
                        tvåor++;
                        settTvå = true;
                    }
                    if (b.Value == 3 && !settTre)
                    {
                        treor++;
                        settTre = true;
                    }


                }
                Console.WriteLine($"Counts: {line}: {tvåor}, {treor}");
            }

            Console.WriteLine($"Resultat: {tvåor} * {treor} = {tvåor * treor}");

            foreach (var line in input)
            {
                var commonString = diffMedEtt(line, input);
                if (commonString != "") Console.WriteLine(commonString);
            }

        }
    }
}
