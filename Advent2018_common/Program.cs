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
            StringBuilder result = new StringBuilder();
            for (int i=0; i< input.Length-1; i++)
            {
                if (Char.IsLower(input[i]) && Char.IsUpper(input[i+1]) &&
                 input[i] == char.ToLower(input[i+1]) ){

                    i++; 
                    continue;
                }
                if (Char.IsUpper(input[i]) && Char.IsLower(input[i+1]) &&
                                 input[i] == char.ToUpper(input[i + 1]) ) {
                    i++; 
                    continue;
                }
                result.Append(input[i]);
            }
            result.Append(input[input.Length - 1]);
            return result;
        }

        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../test.txt");
            var polymer = new StringBuilder( input[0] );
            while (true)
            {
                var reduced = reducePolymer(polymer);
                if (reduced.Length == polymer.Length) break;
                Console.WriteLine($"{reduced.Length}: {reduced}");
                polymer = reduced;
            }
            Console.WriteLine($"{polymer.Length}: {polymer}");
        }
    }
}
