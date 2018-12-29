using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

namespace Advent2018_common
{
    class LandScape
    {
        char[][] squares;
        int sizeX, sizeY;

        public LandScape(string[] input)
        {
            sizeX = input[0].Length;
            sizeY = input.Length;

            squares = new char[sizeY][];
            for (int i = 0; i < input.Length; i++)
            {
                squares[i] = input[i].ToCharArray();
            }
        }
        public LandScape(char[][] squares)
        {
            this.squares = squares;
            sizeX = squares[0].Length;
            sizeY = squares.Length;
        }
        public char this[(int X, int Y) ix]
        {
            get
            {
                if (ix.X >= 0 && ix.X < sizeX && ix.Y >= 0 && ix.Y < sizeY) return squares[ix.Y][ix.X];
                return ' ';
            }
            set
            {
                squares[ix.Y][ix.X] = value;
            }
        }
        public void PrintLandscape()
        {
            foreach (var line in squares)
            {
                Console.WriteLine(new string(line));
            }
        }
        public LandScape OneMinuteTick()
        {
            var nextMinute = new char[sizeY][];

            for (int y = 0; y < sizeY; y++)
            {
                var newRow = new char[sizeX];
                for (int x = 0; x < sizeX; x++)
                {
                    var neighbourghs = new List<char>();
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx != 0 || dy !=0) neighbourghs.Add(this[(x + dx, y + dy)]);
                        }
                    }
                    if (this[(x, y)] == '.' && neighbourghs.Count(c => c == '|') >= 3)
                    {
                        newRow[x] = '|';
                    }
                    else if (this[(x, y)] == '|' && neighbourghs.Count(c => c == '#') >= 3)
                    {
                        newRow[x] = '#';
                    }
                    else if (this[(x, y)] == '#' && (neighbourghs.Count(c => c == '#') < 1 || neighbourghs.Count(c => c == '|') < 1))
                    {
                        newRow[x] = '.';
                    }
                    else newRow[x] = this[(x, y)];
                }
                nextMinute[y] = newRow;
            }
            return new LandScape(nextMinute);
        }
        public int LumberYardTrees ()
        {
            var numLumberyards = squares.SelectMany(r => r).Count(c => c == '#');
            var numTrees = squares.SelectMany(r => r).Count(c => c == '|');
            return numLumberyards * numTrees;
        }
    }


    class MainClass
    {
        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");

            var landscape = new LandScape(input);
            landscape.PrintLandscape();

            var sustainedResources = new int[28];

            for (Int64 i=1; i <= 4000; i++)
            {
                landscape = landscape.OneMinuteTick();
                var resourceValue = landscape.LumberYardTrees();
                Console.WriteLine($"After {i} minute(s): {resourceValue} {sustainedResources[i % 28]} {resourceValue == sustainedResources[i % 28]}");
                sustainedResources[i % 28] = resourceValue;
            }
            landscape.PrintLandscape();

            Console.WriteLine($"1000000000 mod 28 = {1000000000 % 28 } => Resource value = {sustainedResources[1000000000 % 28]}");

            Console.ReadKey();
        }


    }
}
