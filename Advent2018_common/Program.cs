using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace Advent2018_common
{
    class Pots
    {
        HashSet<int> state = new HashSet<int>();

        public Pots (string initialState)
        {
            int i = 0;
            foreach (var c in initialSstate)
            {
                if (c == "#") state.Add(i);
                i++;
            }
        }
    }
    class Rule
    {
        BitArray pattern;
        bool nextGenPlant;

    }

    class MainClass
    {

        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../test.txt");
            Pots pots = new Pots(input[0].Split(" :".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[2]);


            Console.WriteLine($"Maxmimum sum");
            Console.ReadKey();
        }
    }
}
