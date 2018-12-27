using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace Advent2018_common
{
    class NanoBot
    {
        public (int X, int Y, int Z) pos;
        public int range;

        public List<NanoBot> OverlappingBots = new List<NanoBot>();

        public NanoBot(int x, int y, int z, int range)
        {
            pos.X = x;
            pos.Y = y;
            pos.Z = z;
            this.range = range;
        }
        // Is the other bot in range of this?
        public bool InRange((int X, int Y, int Z) otherPos)
        {
            var distance = Math.Abs(otherPos.X - pos.X) + Math.Abs(otherPos.Y - pos.Y) + Math.Abs(otherPos.Z - pos.Z);
            return distance <= range;
        }
        // At least one square overlaps in range
        public bool Overlap(NanoBot other)
        {
            var distance = Math.Abs(other.pos.X - pos.X) + Math.Abs(other.pos.Y - pos.Y) + Math.Abs(other.pos.Z - pos.Z);
            return distance <= range + other.range;
        }
        private int rangeDist(int min, int max, int x)
        {
            if (x > max) return x - max;
            if (x < min) return min - x;
            return 0;
        }
        public int DistanceToCube(SearchCube cube)
        {
            return rangeDist(cube.min.X, cube.max.X, pos.X) +
                   rangeDist(cube.min.Y, cube.max.Y, pos.Y) +
                   rangeDist(cube.min.Z, cube.max.Z, pos.Z);
        }
        public override string ToString()
        {
            return $"{pos}{range}";
        }
    }
    struct SearchCube
    {
        public (int X, int Y, int Z) min;
        public (int X, int Y, int Z) max;

        List<NanoBot> bots;
        int? _botsInRange;
        public int BotsInRange
        {
            get
            {
                var thisCube = this;
                if (_botsInRange == null) _botsInRange = bots.Count(bot => bot.DistanceToCube(thisCube) <= bot.range);
                return _botsInRange.Value;
            }
        }
        public int Size
        {
            get
            {
                return max.X - min.X + max.Y - min.Y + max.Z - min.Z;
            }
        }
        public IEnumerable<SearchCube> SplitCube()
        {
            for (int x = min.X; x <= max.X; x += (max.X - min.X + 2) / 2)
            {
                for (int y = min.Y; y <= max.Y; y += (max.Y - min.Y + 2) / 2)
                {
                    for (int z = min.Z; z <= max.Z; z += (max.Z - min.Z + 2) / 2)
                    {
                        (int X, int Y, int Z) newCubeMax = (Math.Min(x + (max.X - min.X) / 2, max.X),
                                                             Math.Min(y + (max.Y - min.Y) / 2, max.Y),
                                                             Math.Min(z + (max.Z - min.Z) / 2, max.Z));
                        if (newCubeMax.X < x || newCubeMax.Y < y || newCubeMax.Z < z) Console.WriteLine($"Split cube too small: {(x, y, z)}- {newCubeMax}"); 
                        yield return new SearchCube((x, y, z), newCubeMax, bots);
                    }
                }
            }
        }

        public SearchCube((int X, int Y, int Z) min, (int X, int Y, int Z) max, List<NanoBot> bots)
        {
            this.min = min;
            this.max = max;
            this.bots = bots;
            this._botsInRange = null;
        }
        public override string ToString()
        {
            return $"{min} - {max}: {BotsInRange}";
        }
    }
    class CubeSorter : IComparer<SearchCube>
    {
        List<NanoBot> bots;
        public CubeSorter(List<NanoBot> bots)
        {
            this.bots = bots;
        }
        public int Compare(SearchCube a, SearchCube b)
        {
            // Most bots
            if (a.BotsInRange != b.BotsInRange) return a.BotsInRange.CompareTo(b.BotsInRange);
            // then closest to 0,0,0
            if (a.min.X + a.min.Y + a.min.Z != b.min.X + b.min.Y + b.min.Z) return (a.min.X + a.min.Y + a.min.Z).CompareTo(b.min.X + b.min.Y + b.min.Z);
            // then size
            if (a.Size != b.Size) return a.Size.CompareTo(b.Size);
            // then identity (essentially)
            return a.min.CompareTo(b.min);
        }
    }
    class MainClass
    {
        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");

            var bots = new List<NanoBot>();
            foreach (var line in input)
            {
                var parts = line.Split(" <>,=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                .Where(p => int.TryParse(p, out int result)).Select(p => Convert.ToInt32(p)).ToList();

                bots.Add(new NanoBot(parts[0], parts[1], parts[2], parts[3]));
            }
            var botWithGreatestRange = bots.OrderByDescending(b => b.range).First();

            Console.WriteLine($"Bot with greatest range: {botWithGreatestRange}");
            var inRangeOfBot1 = bots.Where(b => botWithGreatestRange.InRange(b.pos)).ToList();

            Console.WriteLine($"A total of {inRangeOfBot1.Count} bots are in range.");
            var minExtent = (bots.Min(b => b.pos.X), bots.Min(b => b.pos.Y), bots.Min(b => b.pos.Z));
            var maxExtent = (bots.Max(b => b.pos.X), bots.Max(b => b.pos.Y), bots.Max(b => b.pos.Z));

            var toSearch = new SortedSet<SearchCube>(new CubeSorter(bots));
            toSearch.Add(new SearchCube(minExtent, maxExtent, bots));

            while (toSearch.Any())
            {
                var cube = toSearch.Last();
                toSearch.Remove(cube);
                   if (cube.Size == 0)
                {
                    Console.WriteLine($"Found a 1 sized box with {cube.BotsInRange} bots at {cube.min}: Distance: {cube.min.X + cube.min.Y + cube.min.Z}");
                    break;
                }
                else
                {
                    // Split cube
                    foreach (var newCube in cube.SplitCube())
                    {
                        if (!toSearch.Add(newCube)) Console.WriteLine($"Did not add cube: {newCube.min}-{newCube.max}");
                    }
                }

            }


            //            Console.WriteLine($"{maxBotsInRange} at {bestPositions.OrderByDescending(p => p.R).ThenBy(p => p.X + p.Y + p.Z).First()}");
            Console.ReadKey();
        }
    }
}
