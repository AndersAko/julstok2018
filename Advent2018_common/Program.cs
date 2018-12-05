using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent2018_common
{
    class MainClass
    {
        private static string DiffMedEtt(string a, string[] lines)
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
                var letterCount = (line.OrderBy(c => c).GroupBy(c => c)).Select(c => c.Count());
                if (letterCount.Contains(2)) tvåor++;
                if (letterCount.Contains(3)) treor++;

                Console.WriteLine($"Counts: {line}: {tvåor}, {treor}");
            }

            Console.WriteLine($"Resultat: {tvåor} * {treor} = {tvåor * treor}");

            foreach (var line in input)
            {
                var commonString = DiffMedEtt(line, input);
                if (commonString != "") Console.WriteLine(commonString);
            }
            Console.ReadKey();
        }
    }
}
