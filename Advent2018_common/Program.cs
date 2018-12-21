using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace Advent2018_common
{
    struct Instruction
    {
        string opCode;
        int inputA;
        int inputB;
        int output;

        public Instruction(string Opcode, int A, int B, int Out)
        {
            opCode = Opcode;
            inputA = A;
            inputB = B;
            output = Out;
        }
        public Instruction (string instruction) :this(instruction.Split(' ')[0], Convert.ToInt32(instruction.Split(' ')[1]), 
            Convert.ToInt32(instruction.Split(' ')[2]), Convert.ToInt32(instruction.Split(' ')[3]))
        {        
        }

        public int[] Execute(int[] inputRegisters)
        {
            //var result = new Dictionary<int, int>(inputRegisters);
            var result = inputRegisters;

            switch (opCode)
            {
                case "addr":
                    result[output] = inputRegisters[inputA] + inputRegisters[inputB];
                    break;
                case "addi":
                    result[output] = inputRegisters[inputA] + inputB;
                    break;
                case "mulr":
                    result[output] = inputRegisters[inputA] * inputRegisters[inputB];
                    break;
                case "muli":
                    result[output] = inputRegisters[inputA] * inputB;
                    break;
                case "banr":
                    result[output] = inputRegisters[inputA] & inputRegisters[inputB];
                    break;
                case "bani":
                    result[output] = inputRegisters[inputA] & inputB;
                    break;
                case "borr":
                    result[output] = inputRegisters[inputA] | inputRegisters[inputB];
                    break;
                case "bori":
                    result[output] = inputRegisters[inputA] | inputB;
                    break;
                case "setr":
                    result[output] = inputRegisters[inputA]; 
                    break;
                case "seti":
                    result[output] = inputA;
                    break;
                case "gtir":
                    result[output] = inputA > inputRegisters[inputB] ? 1 : 0;
                    break;
                case "gtri":
                    result[output] = inputRegisters[inputA] > inputB ? 1 : 0;
                    break;
                case "gtrr":
                    result[output] = inputRegisters[inputA] > inputRegisters[inputB] ? 1 : 0;
                    break;
                case "eqir":
                    result[output] = inputA == inputRegisters[inputB] ? 1 : 0;
                    break;
                case "eqri":
                    result[output] = inputRegisters[inputA] == inputB ? 1 : 0;
                    break;
                case "eqrr":
                    result[output] = inputRegisters[inputA] == inputRegisters[inputB] ? 1 : 0;
                    break;
            }
            return result;
        }
    }


    class MainClass
    {
        public static int TestOpcodes(int[] before, int[] after, int A, int B, int C)
        {
            string[] opCodes = { "addr", "addi", "mulr", "muli", "banr", "bani", "borr", "bori", "setr", "seti", "gtir", "gtri", "gtrr", "eqir", "eqri", "eqrr" };

            var matchingOpcodes = 0;
            foreach (var opCode in opCodes)
            {
                int[] beforeTest = new int[4];

                before.CopyTo(beforeTest, 0);
                var instr = new Instruction(opCode, A, B, C);
                var afterExecuteTest = instr.Execute(beforeTest);
                if (afterExecuteTest.SequenceEqual(after))
                {
                   // Console.WriteLine($"Found a match for {opCode}");
                    matchingOpcodes++;
                }
            }
            return matchingOpcodes;
        }
        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");
            
           // var registers = new int[4];

            // Test code:
            var beforeTest = new int[] { 3, 2, 1, 1 };
            var afterTest = new int[] { 3, 2, 2, 1 };

            Console.WriteLine($"{TestOpcodes(beforeTest, afterTest, 2, 1, 2)} match the given behaviour in the test case.");

            var inputEnum = input.GetEnumerator();
            int matchThreeOrMore = 0;
            while (inputEnum.MoveNext())
            {
                string line = inputEnum.Current as string;
                if (!line.StartsWith("Before")) break;

                var before = line.Split(" [,]".ToCharArray()).Where(x => Int32.TryParse(x, out int n)).Select(x => { if (Int32.TryParse(x, out int result)) return result; else return 0; }).ToArray();

                inputEnum.MoveNext();
                line = inputEnum.Current as string;
                var instr = line.Split(" [,]".ToCharArray()).Select(x => Convert.ToInt32(x)).ToArray();

                inputEnum.MoveNext();
                line = inputEnum.Current as string;
                var after = line.Split(" [,]".ToCharArray()).Where(x => Int32.TryParse(x, out int n)).Select(x => { if (Int32.TryParse(x, out int result)) return result; else return 0; }).ToArray();

                if (TestOpcodes(before, after, instr[1], instr[2], instr[3]) >= 3 )
                {
                    Console.WriteLine($"Found an input that matches more than 3: {String.Join(" ", instr)}");
                    matchThreeOrMore++;
                }
                inputEnum.MoveNext();
            }
            Console.WriteLine($"A total of {matchThreeOrMore} patterns match three or more opcodes");
            Console.ReadKey();
        }
    }
}
