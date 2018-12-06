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
        public List<Point> closest;
        public bool infinite=true;

        internal List<Point> GetSquaresAtDistance(int distance, Point min, Point max)
        {
            var squares = new List<Point>();

            for (int x = -distance; x <= distance; x++)
            {
                var y = distance - Math.Abs(x);
                if (!(pos.x + x > min.x && pos.x + x < max.x)) break;
                if (pos.y + y > min.y && pos.y + y < max.y)
                {
                    squares.Add(new Point(pos.x + x, pos.y + y));
                }
                if (pos.y - y > min.y && pos.y - y < max.y && y!= 0)
                {
                    squares.Add(new Point(pos.x + x, pos.y - y));
                }
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
        
    }
    public static class Extensions
    {
        public static string ToString(this List<Point> points) => "[ " + String.Join(",", points.Select(x => x.ToString()).ToArray()) + "]";
    }

    class MainClass
    {

        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");

            var coords = new List<Coord>();

            Point max = new Point();
            Point min = new Point();
            
            for (int i=0; i< input.Length; i++)
            {
                var x = new Coord();
                var inputCoords = input[i].Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(c => Convert.ToInt32(c)).ToArray();
                x.pos = new Point(inputCoords[0], inputCoords[1]);
                x.closest = new List<Point>();

                if (i == 0)
                {
                    max = x.pos;
                    min = x.pos;
                } else
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
            foreach(var c in coords)
            {
                Console.WriteLine($"{c.id}: {c.pos.x}, {c.pos.y}");
            }
            // Try increasing manhattan distances upto a reasonable maximum
            var visitedSquares = new List<Point>();
            
            for (int distance=0; distance < max.x-min.x && distance<max.y-min.y; distance++)
            {
                Console.WriteLine($"DISTANCE: {distance}");
                var visitedSquaresThisDistance = new List<Point>();
                foreach (var c in coords)
                {
                    var squaresAtDistance = c.GetSquaresAtDistance(distance, min, max);
                    
                    // Remove squares that are already visited ie closer to some other point
                    squaresAtDistance = squaresAtDistance.Except(visitedSquares).ToList();
                   
                    c.closest = c.closest.Concat(squaresAtDistance).ToList();

                    List<Point> squaresAtEqualDistance = squaresAtDistance.Intersect(visitedSquaresThisDistance).ToList(); 
                    if (squaresAtEqualDistance.Count != 0)
                    {
                        // Console.WriteLine($"Some squares are equal distance to two points: {String.Join("; ", squaresAtEqualDistance.Select(x => x.ToString()).ToArray())}");
                        // Remove duplicate squares from all coords
                        foreach (var c2 in coords)
                        {
                            c2.closest = c2.closest.Except(squaresAtEqualDistance).ToList();
                        }
                    }
                    if (squaresAtDistance.Count - squaresAtEqualDistance.Count == 0)
                    {
                        c.infinite = false;
                    }
                    visitedSquaresThisDistance = visitedSquaresThisDistance.Union(squaresAtDistance).ToList();
                }
                visitedSquares = visitedSquares.Concat(visitedSquaresThisDistance).ToList();
                foreach (var c in coords)
                {
                    Console.WriteLine($"{c.id}: {c.pos.x}, {c.pos.y} Area: {c.closest.Count} {c.infinite}");
                }
            }
            Console.WriteLine($"x: {min.x} to {max.x}");
            for (int y=min.y-5; y<=max.y+5; y++)
            {
                Console.Write($"{y,3}: ");
                for (int x = min.x-5; x <= max.x+5; x++)
                {
                    var coordClosestToThisPoint = coords.Where(c => c.closest.Contains(new Point(x,y)));
                    var coordAtThisPoint = coords.Where(c => c.pos.x == x && c.pos.y == y);
                    if (coordAtThisPoint.Count() == 1)
                    {
                        Console.Write((char) (coordAtThisPoint.First().id + 'a') + " ");
                    } else if (coordClosestToThisPoint.Count() == 1)
                    {
                        Console.Write((char) (coordClosestToThisPoint.First().id + 'a')+" ");
                    } else if (coordClosestToThisPoint.Count() > 1)
                    {
                        Console.Write(coordClosestToThisPoint.Count()+" ");
                    } else
                    {
                        Console.Write(". ");
                    }
                }
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}
