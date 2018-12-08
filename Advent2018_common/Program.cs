using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Advent2018_common
{

    public class Node
    {
        public List<Node> childNodes { get; set; } = new List<Node>() ;
        public List<int> metaData { get; set; } = new List<int>() ;
        int id;

        static int nextId = 0;

        public Node(IEnumerator<int> inputSequence)
        {
            id = nextId++;

            var numChildren = inputSequence.Current;
            if (!inputSequence.MoveNext()) Console.WriteLine("Failed to parse!");

            var numMetadata = inputSequence.Current;
            if (!inputSequence.MoveNext()) Console.WriteLine("Failed to parse!!");

            for (int i=0; i<numChildren; i++)
            {
                childNodes.Add(new Node(inputSequence));
            }
            for (int i = 0; i < numMetadata; i++)
            {
                var meta = inputSequence.Current;
                if (!inputSequence.MoveNext()) Console.WriteLine("Failed to parse metadata");

                metaData.Add(meta);
            }
        }
        public int SumMetadata()
        {
            return metaData.Sum() + childNodes.Select(c => c.SumMetadata()).Sum();
        }
        public int Value()
        {
            if (childNodes.Count() == 0) return metaData.Sum();

            int value = 0; 
            foreach (var index in metaData)
            {
                 value += (childNodes.ElementAtOrDefault(index-1)?.Value() ?? 0);
            }
            return value;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder($"{id}: C(");
            foreach (var c in childNodes)
            {
                result.Append($"{c.id} ");
            }
            result.Append (") M( ");
            foreach (var m in metaData)
            {
                result.Append($"{m} ");
            }
            result.Append(")");
            return result.ToString();
        }

    }
    class MainClass
    {

        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");

            var inputSequence = input[0].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x)).GetEnumerator();

            inputSequence.MoveNext();
            var root = new Node(inputSequence);

            Console.WriteLine($"Root node: {root}.");
            foreach (var c in root.childNodes) Console.WriteLine($"Children: {c}.");

            Console.WriteLine($"Sum of all metadata: {root.SumMetadata()}");
            Console.WriteLine($"Value of root: {root.Value()}");

            Console.ReadKey();
        }
    }
}
