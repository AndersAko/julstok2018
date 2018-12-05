using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Advent2018_common
{

    class MainClass
    {
        private static StringBuilder reducePolymer (StringBuilder input)
        {
            StringBuilder result = input;
            for (int i=0; i< result.Length-1; i++)
            {
                if (Char.IsLower(result[i]) && Char.IsUpper(result[i+1]) &&
                 result[i] == char.ToLower(result[i+1]) ){
                    result = result.Remove(i, 2);
                    continue;
                }
                if (Char.IsUpper(result[i]) && Char.IsLower(result[i+1]) &&
                                 result[i] == char.ToUpper(result[i + 1]) ) {
                    result = result.Remove(i, 2);
                }
            }
            return result;
        }

        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");
            var polymer = new StringBuilder( input[0] );

            int minimumPolymerLength = Int32.MaxValue;

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
