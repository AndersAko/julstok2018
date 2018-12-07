using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Advent2018_common
{
    internal class Instruction
    {
        public string id;
        public List<Instruction> nextSteps;
        public List<Instruction> previousSteps;
        public bool completed = false;
        public Nullable<int> endAtSecond;

        public Instruction(string id)
        {
            this.id = id;
            this.nextSteps = new List<Instruction>();
            this.previousSteps = new List<Instruction>();
        }
    }

    class MainClass
    {

        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");

            Dictionary<string, Instruction> instructions = new Dictionary<string, Instruction>();

            // Parse
            foreach (var line in input)
            {
                var splitLine = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();
                var stepBeforeId = splitLine[1];
                var stepAfterId = splitLine[7];

                // Find after instruction if it already exists, otherwise create it
                Instruction stepAfter;
                if (!instructions.ContainsKey(stepAfterId))
                {
                    stepAfter = new Instruction(stepAfterId);
                    instructions[stepAfterId] = stepAfter;
                }
                stepAfter = instructions[stepAfterId];

                // Find before instruction if it already exists, otherwise create it
                Instruction stepBefore;
                if (!instructions.ContainsKey(stepBeforeId))
                {
                    stepBefore = new Instruction(stepBeforeId);
                    instructions[stepBeforeId] = stepBefore;
                }
                stepBefore = instructions[stepBeforeId];

                stepBefore.nextSteps.Add(stepAfter);
                stepAfter.previousSteps.Add(stepBefore);
            }
            var first = instructions.Where(s => s.Value.previousSteps.Count == 0).First();
            Console.Write($"Found {instructions.Where(s => s.Value.previousSteps.Count == 0).Count()} first steps: ");
            foreach (var i in instructions.Where(s => s.Value.previousSteps.Count == 0))
            {
                Console.Write($"{i.Value.id} ");
            }
            Console.WriteLine();

            List <Instruction> available = new List<Instruction>( instructions.Where(s => s.Value.previousSteps.Count == 0).Select(i => i.Value) );

            StringBuilder sequence = new StringBuilder("");
            while (available.Count > 0)
            {
                var nextAvailable = available.Where(i => i.previousSteps.Count == 0 || i.previousSteps.All(pi => pi.completed)).OrderBy(i => i.id).First();
            
                sequence.Append(nextAvailable.id);
                nextAvailable.completed = true;
                available.Remove(nextAvailable);
                available = available.Union(nextAvailable.nextSteps).ToList();

            }
            Console.WriteLine($" Correct sequence: {sequence}");

            // Part 2
            // Reset complete flag
            foreach (var i in instructions) i.Value.completed = false;
            sequence = new StringBuilder("");

            int secondNow = 0;
            var workInProgress = new List<Instruction>();

            while (true)
            {
                available = new List<Instruction>(instructions
                .Where(s => (s.Value.previousSteps.Count == 0 || s.Value.previousSteps.All(pi => pi.completed)) && !s.Value.endAtSecond.HasValue)
                .Select(i => i.Value)
                .OrderBy(i => i.id));
                if (available.Count == 0 && workInProgress.Count == 0) break;

                // Add more workers if needed
                if (workInProgress.Count < 6 && available.Count > 0)
                {
                    var nextAvailable = available.First();
                    nextAvailable.endAtSecond = secondNow + 60 + nextAvailable.id.ToCharArray()[0] - 'A' + 1 ;
                    workInProgress.Add(nextAvailable);
                    continue;
                }

                // Complete new completed work...
                var nextToComplete = workInProgress.OrderBy(i => i.endAtSecond).First();
                sequence.Append(nextToComplete.id);
                secondNow = nextToComplete.endAtSecond.Value;
                nextToComplete.completed = true;
                workInProgress.Remove(nextToComplete);

                // Console.WriteLine($" {secondNow}: sequence: {sequence} workers: {string.Join(" ", workInProgress.Select(i => $"{i.id} {i.endAtSecond}"))}");
            }
            Console.WriteLine($"Work completed in {secondNow} seconds.");
            Console.ReadKey();
        }
    }
}
