using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Advent2018_common
{

    class MainClass
    {
        private static StringBuilder reducePolymer(StringBuilder input)
        {
            StringBuilder result = input;
            var original = input.Length;

            for (int i = 0; i < result.Length - 1; i++)
            {
                if ((result[i] ^ result[i + 1]) == 32)
                {
                    result.Remove(i, 2);
                    // Backup so the "new" next character is not skipped
                    i--;
                    // If not the begining of string, backup one addtional character to detect if a new match is found
                    if (i >= 0) i--;
                }
            }
            //Console.Write($"( reduced {original}-> {result.Length} ) ");
            return result;
        }

        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");

            var polymer = new StringBuilder(input[0]);
            polymer = reducePolymer(polymer);
            Console.WriteLine($"Reduced length {polymer.Length}");

            //  Generate "abcdefghijklmnopqrstuvwxyz" to avoid typos
            string units = new string(Enumerable.Range('a', 'z' - 'a' + 1).Select(c => (char)c).ToArray());

            int minimumPolymerLength = Int32.MaxValue;

            foreach (var unit in units.ToCharArray())
            {
                var cleanedUpPolymer = new StringBuilder(polymer.ToString());
                cleanedUpPolymer = cleanedUpPolymer.Replace(Char.ToLower(unit).ToString(), "");
                cleanedUpPolymer = cleanedUpPolymer.Replace(Char.ToUpper(unit).ToString(), "");

                cleanedUpPolymer = reducePolymer(cleanedUpPolymer);

                Console.WriteLine($"{unit} => {cleanedUpPolymer.Length}");
                if (cleanedUpPolymer.Length < minimumPolymerLength)
                {
                    minimumPolymerLength = cleanedUpPolymer.Length;
                    Console.WriteLine($"{cleanedUpPolymer.Length}: {cleanedUpPolymer}");
                }

            }
            Console.WriteLine($"{minimumPolymerLength}");
            Console.ReadKey();
        }
    }
}
