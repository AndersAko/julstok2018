using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace Advent2018_common
{
    class Cave
    {
        Dictionary<(int, int), int> erosionLevel = new Dictionary<(int, int), int>();
        int depth;
        public (int X, int Y) target;

        public Cave(int depth, (int X, int Y) target)
        {
            this.depth = depth;
            this.target = target;
        }

        public int ErosionLevel((int X, int Y) coord)
        {
            if (!erosionLevel.ContainsKey(coord))
            {
                if (coord == target)
                {
                    erosionLevel[coord] = 0 + depth;
                }
                else if (coord.Y == 0)
                {
                    erosionLevel[coord] = (coord.X * 16807 + depth) % 20183;
                }
                else if (coord.X == 0)
                {
                    erosionLevel[coord] = (coord.Y * 48271 + depth) % 20183;
                }
                else
                {
                    erosionLevel[coord] = (ErosionLevel((coord.X - 1, coord.Y)) * ErosionLevel((coord.X, coord.Y - 1)) + depth) % 20183;
                }
            }
            return erosionLevel[coord];
        }
        public List<CavePosition> PossibleMoves(CavePosition cpos)
        {
            var result = new List<CavePosition>();

            if (cpos.pos.X > 0) result.Add(new CavePosition(cpos.pos.X - 1, cpos.pos.Y, cpos.equiped, cpos));
            if (cpos.pos.Y > 0) result.Add(new CavePosition(cpos.pos.X, cpos.pos.Y - 1, cpos.equiped, cpos));
            result.Add(new CavePosition(cpos.pos.X + 1, cpos.pos.Y, cpos.equiped, cpos));
            result.Add(new CavePosition(cpos.pos.X, cpos.pos.Y + 1, cpos.equiped, cpos));
            if (cpos.equiped != Tool.climbing) result.Add(new CavePosition(cpos.pos.X, cpos.pos.Y, Tool.climbing, cpos));
            if (cpos.equiped != Tool.torch) result.Add(new CavePosition(cpos.pos.X, cpos.pos.Y, Tool.torch, cpos));
            if (cpos.equiped != Tool.neither) result.Add(new CavePosition(cpos.pos.X, cpos.pos.Y, Tool.neither, cpos));

            var toReturn = result.Where(cp => cp.CompatibleTool(this)).ToList();
            //Console.WriteLine($"From position {cpos}, the possible moves are: {string.Join(";", toReturn)}");
            return toReturn;
        }
    }
    public enum Tool { climbing, torch, neither };
    struct CavePosition
    {
        public (int X, int Y) pos;
        public Tool equiped;
        public int cost;
        public List<string> path;   // CavePosition.ToString() list

        public CavePosition(int X, int Y, Tool tool, CavePosition? previous = null)
        {
            pos.X = X;
            pos.Y = Y;
            equiped = tool;
            if (previous == null) path = new List<string>();
            else
            {
                path = new List<string>(previous.Value.path)
                {
                    $"{pos.X},{pos.Y}:{equiped}"
                };
            }
            if (previous == null) cost = 0;
            else cost = previous.Value.cost + (previous.Value.equiped == tool ? 1 : 7);
        }

        public bool CompatibleTool(Cave cave)
        {
            var terrainType = cave.ErosionLevel(pos) % 3;
            switch (terrainType)
            {
                case 0:
                    return equiped == Tool.climbing || equiped == Tool.torch;
                case 1:
                    return equiped == Tool.climbing || equiped == Tool.neither;
                case 2:
                    return equiped == Tool.torch || equiped == Tool.neither;
            }
            return false;
        }
        public override string ToString()
        {
            return $"{pos.X},{pos.Y}:{equiped}";
        }
    }
    class AStarSorting : IComparer<CavePosition>
    {
        public Cave cave;

        public AStarSorting(Cave cave)
        {
            this.cave = cave;
        }
        public int Compare(CavePosition a, CavePosition b)
        {
            var estimateA = a.cost + Math.Abs(cave.target.X - a.pos.X) + Math.Abs(cave.target.Y - a.pos.Y);
            var estimateB = b.cost + Math.Abs(cave.target.X - b.pos.X) + Math.Abs(cave.target.Y - b.pos.Y);
            if (estimateA.CompareTo(estimateB) != 0) return estimateA.CompareTo(estimateB);
            if (a.pos.CompareTo(b.pos) != 0) return a.pos.CompareTo(b.pos);
            return a.equiped.CompareTo(b.equiped);
//            return string.Compare(a.ToString(), b.ToString(), StringComparison.Ordinal);
        }
    }
    class MainClass
    {
        public static int ShortestPath(Cave cave)
        {
            var positionsToCheck = new SortedSet<CavePosition>(new AStarSorting(cave));

            var startPosition = new CavePosition(0, 0, Tool.torch);
            var checkedPositions = new HashSet<string>();       // CavePosition.ToString()
            checkedPositions.Add(startPosition.ToString());

            positionsToCheck.UnionWith(cave.PossibleMoves(startPosition));

            while (positionsToCheck.Any())
            {
                var checkPosition = positionsToCheck.First();
                positionsToCheck.Remove(checkPosition);
                checkedPositions.Add(checkPosition.ToString());

                if (checkPosition.pos == cave.target && checkPosition.equiped == Tool.torch)
                {
                    Console.WriteLine($"Found him at {checkPosition.pos} after {checkPosition.cost} minutes");
                    Console.WriteLine($"Path: {String.Join(";", checkPosition.path)}");
                    return checkPosition.cost;
                }
                var newPositions = cave.PossibleMoves(checkPosition)
                                       .Where(cp => !checkedPositions.Contains(cp.ToString())).ToList();
                //foreach (var cp in newPositions)
                //{
                //    var adding = positionsToCheck.Add(cp);
                //    if (adding) Console.WriteLine($"Added: {cp}");
                //    else Console.WriteLine($"Didn't add {cp}");
                //}
                positionsToCheck.UnionWith(newPositions);
             }
            return 0; // Didn't find a path...
        }

        public static void Main(string[] args)
        {
            // string[] input = System.IO.File.ReadAllLines("../../input.txt");
            int depth;
            (int X, int Y) target;

            // Puzzle
            depth = 3066;
            target = (13, 726);
            //depth = 510;
            //target = (10, 10);
            var typeMarker = ".=|";

            var cave = new Cave(depth, target);

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    if ((x, y) == target)
                    {
                        Console.Write("T");
                    }
                    else if ((x, y) == (0, 0))
                    {
                        Console.Write("M");
                    }
                    else
                    {
                        var erosion = (cave.ErosionLevel((x, y)));
                        Console.Write(typeMarker[erosion % 3]);
                    }
                }
                Console.WriteLine();
            }

            int risklevel = 0;
            for (int y = 0; y <= target.Y; y++)
            {
                for (int x = 0; x <= target.X; x++)
                {
                    risklevel += cave.ErosionLevel((x, y)) % 3;
                }
            }
            Console.WriteLine($"Total risk level: {risklevel}");

            // Find the shortest path
            Console.WriteLine($"Finding shortest path: {ShortestPath(cave)}");


            Console.ReadKey();
        }
    }
}
