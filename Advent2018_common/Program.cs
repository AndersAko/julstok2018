using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace Advent2018_common
{
    class Pots
    {
        BitArray state=new BitArray(3, false);
        public int StartIndex = -3;

        public Pots (string initialState)
        {
            foreach (var c in initialState)
            {
                state.Length++;
                state[state.Length - 1] = (c == '#');
            }
            state.Length += 3;
        }
        public Pots (BitArray b, int i)
        {
            state = b;
            StartIndex = i;
        }
        public Pots NextGeneration(List<BitArray> patterns)
        {
            var nextState = new BitArray(state.Length, false);
            var nextStartIndex = StartIndex;
           

            for (int i=0; i<state.Length-5; i++)
            {
                foreach (var p in patterns)
                {
                    var match = true;
                    for (int b=0; b<5; b++)
                    {
                        if (state[i+b] != p[b]) 
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)  // Found a pattern that indicates a new plant at index i+2 (middle of pattern) = pot number i+2+StartIndex
                    {
                        // New plant at first position (ie at index 2), add a new blank cell before to keep three blanks 
                        if (i==0)
                        {
                            nextStartIndex--;
                        }
                        nextState[i + 2 + StartIndex - nextStartIndex] = true;
                    } 
                }
            }
            return new Pots(nextState, nextStartIndex);
        }
        public override string ToString()
        {
            var result = new StringBuilder($"{StartIndex}");
            for (int i=0; i<state.Length; i++)
            {
                result.Append(state[i] ? "#" : ".");
            }
            return result.ToString();
        }
    }

    class MainClass
    {

        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../test.txt");
            Pots pots = new Pots(input[0].Split(" :".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[2]);

            var patterns = new List<BitArray>();
            for (int i=2; i<input.Length; i++)
            {
                if (input[i].Split(" ".ToCharArray())[2]=="#")
                {
                    var inputPattern = input[i].Split(" ".ToCharArray())[0].ToCharArray().Select(x => x == '#');
                    var pattern = new BitArray(inputPattern.ToArray());
                    patterns.Add(pattern);
                }
            }
            
            for (var generation = 1; generation<=20; generation++)
            {
                pots = pots.NextGeneration(patterns);
                Console.WriteLine($"{generation,3}:{new String(' ',7+pots.StartIndex)}{pots}");
            }
            Console.WriteLine($"Maxmimum sum");
            Console.ReadKey();
        }
    }
}
