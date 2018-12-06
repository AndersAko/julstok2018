using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Advent2018_common
{
    /// <summary>
    /// Keeps track of each possible target coordinate
    /// x, y, closest squares
    /// </summary>
    internal class Coord
    {
        public int id;
        public Point pos;
        public HashSet<Point> closest;
        public bool infinite = false;
        public bool complete = false;

        internal static void Add(HashSet<Point> set, Point p, Point min, Point max, HashSet<Point> visited)
        {
            if (p.x < min.x || p.x > max.x) return;
            if (p.y < min.y || p.y > max.y) return;
            if (visited.Contains(p)) return;
            set.Add(p);
        }
        internal HashSet<Point> GetSquaresAtDistance(int distance, Point min, Point max, HashSet<Point> visited)
        {
            var squares = new HashSet<Point>();

            for (int x = -distance; x <= distance; x++)
            {
                var y = distance - Math.Abs(x);
                Coord.Add(squares, new Point(pos.x + x, pos.y + y), min, max, visited);
                Coord.Add(squares, new Point(pos.x + x, pos.y - y), min, max, visited);

            }
            return squares;
        }
    }
    public struct Point
    {
        internal int x, y;
        public Point(int x, int y)
        {
            this.x = x; this.y = y;
        }
        public override string ToString()
        {
            return $"{x},{y}";
        }
        public int DistanceTo(Point p) => Math.Abs(p.x - x) + Math.Abs(p.y - y);
        internal int DistanceTo(List<Coord> coords)
        {
            int sum = 0;
            foreach (var c in coords)
            {
                sum += DistanceTo(c.pos);
            }
            return sum;
        }
    }

    class MainClass
    {

        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");

            var coords = new List<Coord>();

            Point max = new Point();
            Point min = new Point();

            for (int i = 0; i < input.Length; i++)
            {
                var x = new Coord();
                var inputCoords = input[i].Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(c => Convert.ToInt32(c)).ToArray();
                x.pos = new Point(inputCoords[0], inputCoords[1]);
                x.closest = new HashSet<Point>();

                if (i == 0)
                {
                    max = x.pos;
                    min = x.pos;
                }
                else
                {
                    if (x.pos.x > max.x) { max.x = x.pos.x; }
                    if (x.pos.y > max.y) { max.y = x.pos.y; }
                    if (x.pos.x < min.x) { min.x = x.pos.x; }
                    if (x.pos.y < min.y) { min.y = x.pos.y; }
                }

                x.id = i;
                coords.Add(x);
            }
            Console.WriteLine($"Extents: {min} to {max}");
            foreach (var c in coords)
            {
                Console.WriteLine($"{c.id}: {c.pos.x}, {c.pos.y}");
            }
            // Try increasing manhattan distances upto a reasonable maximum
            var visitedSquares = new HashSet<Point>();

            for (int distance = 0; distance < max.x - min.x && distance < max.y - min.y; distance++)
            {
                Console.WriteLine($"DISTANCE: {distance}");
                var visitedSquaresThisDistance = new HashSet<Point>();
                var searchComplete = true;
                foreach (var c in coords)
                {
                    if (c.complete) continue;
                    searchComplete = false;
                    var squaresAtDistance = c.GetSquaresAtDistance(distance, min, max, visitedSquares);

                    c.closest.UnionWith(squaresAtDistance);

                    HashSet<Point> squaresAtEqualDistance = new HashSet<Point>(squaresAtDistance.Intersect(visitedSquaresThisDistance));
                    if (squaresAtEqualDistance.Count != 0)
                    {
                        foreach (var c2 in coords)
                        {
                            c2.closest.ExceptWith(squaresAtEqualDistance);
                        }
                    }
                    c.infinite = c.closest.Any(p => p.x == min.x || p.y == min.y || p.x == max.x || p.y == max.y);

                    if (squaresAtDistance.Count - squaresAtEqualDistance.Count == 0) c.complete = true;
                    visitedSquaresThisDistance.UnionWith(squaresAtDistance);
                }
                visitedSquares.UnionWith(visitedSquaresThisDistance);
                foreach (var c in coords)
                {
                    Console.WriteLine($"{c.id}: {c.pos.x}, {c.pos.y} Area: {c.closest.Count} {c.infinite} {c.complete}");
                }
                var largestArea = coords.Where(c => !c.infinite).OrderByDescending(c => c.closest.Count).First();
                Console.WriteLine($"Greatest of them all: {largestArea.id} at {largestArea.pos.x}, {largestArea.pos.y} with size {largestArea.closest.Count}");
                if (searchComplete) break;
            }
            Console.WriteLine($"x: {min.x} to {max.x}");
            for (int y = min.y - 5; y <= max.y + 5; y++)
            {
                Console.Write($"{y,3}: ");
                for (int x = min.x - 5; x <= max.x + 5; x++)
                {
                    var coordClosestToThisPoint = coords.Where(c => c.closest.Contains(new Point(x, y)));
                    var coordAtThisPoint = coords.Where(c => c.pos.x == x && c.pos.y == y);
                    if (coordAtThisPoint.Count() == 1)
                    {
                        Console.Write((char)(coordAtThisPoint.First().id % 26 + 'A'));
                    }
                    else if (coordClosestToThisPoint.Count() == 1)
                    {
                        Console.Write((char)(coordClosestToThisPoint.First().id % 26 + 'a'));
                    }
                    else if (coordClosestToThisPoint.Count() > 1)
                    {
                        Console.Write(coordClosestToThisPoint.Count());
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }
                Console.WriteLine();
            }

            // Part Two
            int lessThan10000 = 0;
            for (int y = min.y - 5; y <= max.y + 5; y++)
            {
                for (int x = min.x - 5; x <= max.x + 5; x++)
                {
                    if (new Point(x, y).DistanceTo(coords) < 10000) lessThan10000++;
                }
            }
            Console.WriteLine($"Size of region closest to all: {lessThan10000}");
            Console.ReadKey();
        }
    }
}
