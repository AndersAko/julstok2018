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
            Console.ReadKey();
        }
    }
}
