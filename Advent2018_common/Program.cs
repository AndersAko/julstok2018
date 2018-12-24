using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace Advent2018_common
{
    struct Instruction
    {
        public enum InstructionSet { eqri, bori, addi, bani, seti, eqrr, addr, gtri, borr, gtir, setr, eqir, mulr, muli, gtrr, banr, nop };
        InstructionSet opCode;
        int inputA;
        int inputB;
        int output;
        string comment;

        public Instruction(InstructionSet Opcode, int A, int B, int Out)
        {
            opCode = Opcode;
            inputA = A;
            inputB = B;
            output = Out;
            comment = "";
        }
        public Instruction(string instruction)
        {
            string [] args = instruction.Split(" ".ToCharArray(),  StringSplitOptions.RemoveEmptyEntries);
            inputA = Convert.ToInt32(args[1]);
            inputB = Convert.ToInt32(args[2]);
            output = Convert.ToInt32(args[3]);
            if (args.Length >4)
            {
                comment = String.Join(" ", args.Skip(4));
            }
            else
            {
                comment = "";
            }

            switch (args[0])
            {
                case "addr":
                    opCode = InstructionSet.addr;
                    break;
                case "addi":
                    opCode = InstructionSet.addi;
                    break;
                case "mulr":
                    opCode = InstructionSet.mulr;
                    break;
                case "muli":
                    opCode = InstructionSet.muli;
                    break;
                case "banr":
                    opCode = InstructionSet.banr;
                    break;
                case "bani":
                    opCode = InstructionSet.bani;
                    break;
                case "borr":
                    opCode = InstructionSet.borr;
                    break;
                case "bori":
                    opCode = InstructionSet.bori;
                    break;
                case "setr":
                    opCode = InstructionSet.setr;
                    break;
                case "seti":
                    opCode = InstructionSet.seti;
                    break;
                case "gtir":
                    opCode = InstructionSet.gtir;
                    break;
                case "gtri":
                    opCode = InstructionSet.gtri;
                    break;
                case "gtrr":
                    opCode = InstructionSet.gtrr;
                    break;
                case "eqir":
                    opCode = InstructionSet.eqir;
                    break;
                case "eqri":
                    opCode = InstructionSet.eqri;
                    break;
                case "eqrr":
                    opCode = InstructionSet.eqrr;
                    break;
                default:
                    opCode = InstructionSet.nop;
                    break;
            }
        }
        static HashSet<int> r3Values = new HashSet<int>();
        static int previousValue = 0;

        public int[] Execute(int[] inputRegisters)
        {
            //var result = new Dictionary<int, int>(inputRegisters);
            var result = inputRegisters;
           // Console.Write($"{opCode} {inputA} {inputB} {output} {comment} [{String.Join(" ", inputRegisters)}]");
            switch (opCode)
            {
                case InstructionSet.addr:
                    result[output] = inputRegisters[inputA] + inputRegisters[inputB];
                    break;
                case InstructionSet.addi:
                    result[output] = inputRegisters[inputA] + inputB;
                    break;
                case InstructionSet.mulr:
                    result[output] = inputRegisters[inputA] * inputRegisters[inputB];
                    break;
                case InstructionSet.muli:
                    result[output] = inputRegisters[inputA] * inputB;
                    break;
                case InstructionSet.banr:
                    result[output] = inputRegisters[inputA] & inputRegisters[inputB];
                    break;
                case InstructionSet.bani:
                    result[output] = inputRegisters[inputA] & inputB;
                    break;
                case InstructionSet.borr:
                    result[output] = inputRegisters[inputA] | inputRegisters[inputB];
                    break;
                case InstructionSet.bori:
                    result[output] = inputRegisters[inputA] | inputB;
                    break;
                case InstructionSet.setr:
                    result[output] = inputRegisters[inputA];
                    break;
                case InstructionSet.seti:
                    result[output] = inputA;
                    break;
                case InstructionSet.gtir:
                    result[output] = inputA > inputRegisters[inputB] ? 1 : 0;
                    break;
                case InstructionSet.gtri:
                    result[output] = inputRegisters[inputA] > inputB ? 1 : 0;
                    break;
                case InstructionSet.gtrr:
                    result[output] = inputRegisters[inputA] > inputRegisters[inputB] ? 1 : 0;
                    break;
                case InstructionSet.eqir:
                    result[output] = inputA == inputRegisters[inputB] ? 1 : 0;
                    break;
                case InstructionSet.eqri:
                    result[output] = inputRegisters[inputA] == inputB ? 1 : 0;
                    break;
                case InstructionSet.eqrr:
                    result[output] = inputRegisters[inputA] == inputRegisters[inputB] ? 1 : 0;
                    break;
                case InstructionSet.nop:
                    if (r3Values.Count %1000 == 0) Console.WriteLine($"{opCode} {inputA} {inputB} {output} {comment} [{String.Join(" ", result)}] {r3Values.Count()}");
                    if (r3Values.Contains(result[3]))
                    {
                        Console.WriteLine($"Duplicate value {result[3]} [{String.Join(" ", result)}] {previousValue}");
                    }
                    else
                    {
                        r3Values.Add(result[3]);
                    }
                    previousValue = result[3];
                    break;
            }
            //Console.WriteLine($"[{String.Join(" ", result)}]");
            return result;
        }
    }


    class MainClass
    {
        public static Int64 StepsUntilHalt(List<Instruction> program, int ipReg, int r0)
        {
            int[] registers = new int[6];
            registers[0] = r0;

            Int64 i = 0;
            while (registers[ipReg] >= 0 && registers[ipReg] < program.Count)
            {
                var instr = program[registers[ipReg]];
                registers = instr.Execute(registers);
                registers[ipReg]++;
                i++;
//                if (i % 10000000 == 0 || i < 10) Console.WriteLine($"Registers: {String.Join(" ", registers)}");
            }
            Console.WriteLine($"Registers: {String.Join(" ", registers)}");
            return i;
        }
        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");

            int ipReg = -1;
            var program = new List<Instruction>();

            foreach (var line in input)
            {
                if (line.StartsWith("#ip"))
                {
                    ipReg = Convert.ToInt32(line.Split(' ')[1]);
                }
                else
                {
                    var instr = new Instruction(line);
                    program.Add(instr);
                }
            }

            // Shortest
            var steps = StepsUntilHalt(program, ipReg, 7967233);
            Console.WriteLine($"{7967233}: {steps}");

            // Longest
            steps = StepsUntilHalt(program, ipReg, 16477902);
            Console.WriteLine($"{16477902}: {steps}");


            Console.ReadKey();
        }
    }
}
