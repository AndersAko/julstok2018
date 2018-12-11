using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent2018_common
{

    class MainClass
    {
        public static Dictionary<(int X, int Y), int> Cache = new Dictionary<(int X, int Y), int>();

        public static int CalcPower(int x, int y, int grid)
        {
            //if (Cache.ContainsKey((x, y)))
            //{
            //    return Cache[(x, y)];
            //}
            int result = (((x + 10) * y) + grid) * (x + 10);
            result = ((result / 100) % 10) - 5;
            //Cache[(x, y)] = result;
            return result;
        }
        public static void Main(string[] args)
        {
            // string[] input = System.IO.File.ReadAllLines("../../input.txt");

            //Console.WriteLine($"Power at 122,79 for grid 57: {CalcPower(x: 122, y: 79, grid: 57)}");
            //Console.WriteLine($"Power at 217,196 for grid 39: {CalcPower(x: 217, y: 196, grid: 39)} ");
            //Console.WriteLine($"Power at 101,153 for grid 71: {CalcPower(x: 101, y: 153, grid: 71)}");

            var grid = 6878;
            var maxSum = 0;
            (int X, int Y, int Size) topLeft = (0, 0, 0);

            for (int y = 1; y <= 300; y++)
            {
                for (int x = 1; x <= 300; x++)
                {
                    int sum = 0; 
                    for (int squareSize = 1; squareSize <= 300  && +squareSize <= 300 && y+squareSize<=300; squareSize++)
                    {
                        for (int dx = 0; dx < squareSize-1; dx++)
                        {
                            sum += CalcPower(x + dx, y+squareSize-1, grid);
                        }
                        for (int dy = 0; dy < squareSize - 1; dy++)
                        {
                            sum += CalcPower(x+squareSize-1, y + dy, grid);
                        }
                        sum += CalcPower(x + squareSize-1, y + squareSize-1, grid);
                        if (sum > maxSum)
                        {
                            maxSum = sum;
                            topLeft = (x, y, squareSize);
                        }
                    }
                }
                Console.WriteLine($"After row {y}: {maxSum} at {topLeft.X},{topLeft.Y},{topLeft.Size}");
            }

            Console.WriteLine($"Maxmimum sum {maxSum} at {topLeft.X},{topLeft.Y},{topLeft.Size}");
            Console.ReadKey();
        }
    }
}
