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
            var unitPairs = getUnitPairs();
            for (int i=0; i<result.Length-1; i++)
            {
                if (unitPairs.Contains(result.ToString(i,2)) )
                {
                    result.Remove(i, 2);
                }
            }
            return result;
        }
        private static HashSet<string> unitPairs = null;
        // Return a list of pairs such as "Aa", "aA", "Bb", "bB", ...
        private static HashSet<string> getUnitPairs()
        {
            if (unitPairs == null)
            {
                // Generate list of unitPairs
                var list = Enumerable.Range('A', 'Z' - 'A' + 1)
                        .Select(c => ((char)c).ToString() + Char.ToLower((char) c).ToString())
                    .Concat(Enumerable.Range('A', 'Z' - 'A' + 1)
                        .Select(c => (char.ToLower((char)c).ToString() + (char)c).ToString()));
                unitPairs = new HashSet<string>(list);
            }
            return unitPairs;
        }
        
        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");
            var polymer = new StringBuilder( input[0] );

            int minimumPolymerLength = Int32.MaxValue;
            Console.WriteLine($"Our pairs are:");
            foreach (var x in getUnitPairs())
            {
                Console.Write($"{x} ");
            }
            Console.WriteLine();

            string units = "abcdefghijklmnopqrstuvwxyz";
            foreach (var unit in units.ToCharArray())
            {
                polymer = new StringBuilder(input[0]);
                polymer = polymer.Replace(Char.ToLower(unit).ToString() , "");
                polymer = polymer.Replace(Char.ToUpper(unit).ToString(), "");

                var length = polymer.Length;
                while (true)
                {
                    var reduced = reducePolymer(polymer);
                    if (reduced.Length == length) break;
                    polymer = reduced;
                    length = polymer.Length;
                }
                if (polymer.Length < minimumPolymerLength) minimumPolymerLength = length;
                Console.WriteLine($"{polymer.Length}: {polymer}");

            }
            Console.WriteLine($"{minimumPolymerLength}");
        }
    }
}
