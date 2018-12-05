using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent2018_common
{

    class MainClass
    {

        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");
            var sorted = (from line in input orderby line select line).ToArray();

            // Array of 60 ints for each guard, one slot per minute. Number indicates number of occurances asleep
            var guards = new Dictionary<int, int[]>();
            int activeGuard = 0;
            int asleepAtMinute = 0;
            
            for (int i=0; i<sorted.Count(); i++)
            {
                // Parse 
                //      [1518-02-08 23:57] Guard #1439 begins shift
                //      [1518-02-09 00:16] falls asleep
                //      [1518-02-09 00:53] wakes up
                //      [1518-02-09 00:56] falls asleep
                //      [1518-02-09 00:57] wakes up
                var splitLine = sorted[i].Split(" :][#".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (splitLine[3] == "Guard")
                {
                    activeGuard = Convert.ToInt32(splitLine[4]);
                    if (!guards.ContainsKey(activeGuard))
                    {
                        guards[activeGuard] = new int[60];
                    }
                }
                else if (splitLine[3] == "falls")
                {
                    asleepAtMinute = Convert.ToInt32(splitLine[2]);
                } else if (splitLine[3] == "wakes")
                {
                    var toMinute = Convert.ToInt32(splitLine[2]);

                    for (int j = asleepAtMinute; j < toMinute; j++)
                    {
                        guards[activeGuard][j]++;
                    }
                }
                string text = String.Join("", guards[activeGuard] );
                Console.WriteLine($"{activeGuard}: {sorted[i]}: {text}");
            }
            foreach (var guard in guards)
            {
                string text = String.Join("", guard.Value);
                Console.WriteLine($"{guard.Key}: {text}");
            }
            var asleepMinutes = (from guard in guards select (guard.Key, guard.Value.Sum())); 
            var maxAsleepGuard = (from guard in asleepMinutes where guard.Item2 == asleepMinutes.Max(x => x.Item2) select guard).First();
            Console.WriteLine($"Most asleep guard: { maxAsleepGuard.Item1}, sleeping { maxAsleepGuard.Item2} ");
            Console.WriteLine($"Most asleep minute: {Array.IndexOf(guards[maxAsleepGuard.Item1], guards[maxAsleepGuard.Item1].Max())}");
            Console.WriteLine($"Result: {maxAsleepGuard.Item1 * Array.IndexOf(guards[maxAsleepGuard.Item1], guards[maxAsleepGuard.Item1].Max())}");

            var mostFrequentAsleep = (from guard in guards select (guard.Key, guard.Value.Max()));
            var mostFrequentAsleepGuard = (from guard in mostFrequentAsleep where guard.Item2 == mostFrequentAsleep.Max(x => x.Item2) select guard).First();
            Console.WriteLine($"Most frequently asleep guard: { mostFrequentAsleepGuard.Item1}, sleeping { mostFrequentAsleepGuard.Item2} times");
            Console.WriteLine($"Most asleep minute: {Array.IndexOf(guards[mostFrequentAsleepGuard.Item1], guards[mostFrequentAsleepGuard.Item1].Max())}");
            Console.WriteLine($"Result: {mostFrequentAsleepGuard.Item1 * Array.IndexOf(guards[mostFrequentAsleepGuard.Item1], guards[mostFrequentAsleepGuard.Item1].Max())}");

        }
    }
}
