using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent2018_common
{

    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        private readonly int velX;
        private readonly int velY;

        public  Point (string input)
        {
            var split = input.Split(" <>=,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
            this.X = Convert.ToInt32(split[1]);
            this.Y = Convert.ToInt32(split[2]);
            this.velX = Convert.ToInt32(split[4]);
            this.velY = Convert.ToInt32(split[5]);
        }
        public void Move()
        {
            X += velX;
            Y += velY;
        }
        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }

    class MainClass
    {
        public static void PrintPoints(List<Point> points, int minX, int maxX, int minY, int maxY)
        {
            for (var y = minY; y<=maxY; y++)
            {
                for (int x = minX; x<= maxX; x++)
                {
                    if (points.Exists(p => p.X == x && p.Y == y)) Console.Write("#");
                    else Console.Write(".");
                }
                Console.WriteLine();
            }
        }
        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");

            var points = new List<Point>();
            
            foreach (var line in input)
            {
                var point = new Point(line);
                points.Add(point);                    
            }
            Console.WriteLine("Second 0");
            //            PrintPoints(points, minX, maxX, minY, maxY);

            Nullable<(int X, int Y)> max = null, min = null;
            for (int i=1; i<1000000; i++)
            {
                foreach (var point in points)
                {
                    point.Move();
                }
                (int X, int Y) newMax = points.Select(p => (p.X, p.Y)).Aggregate(((int x, int y) p1, (int x, int y) p2) => (Math.Max(p1.x, p2.x), Math.Max(p1.y, p2.y)));
                (int X, int Y) newMin = points.Select(p => (p.X, p.Y)).Aggregate(((int x, int y) p1, (int x, int y) p2) => (Math.Min(p1.x, p2.x), Math.Min(p1.y, p2.y)));
                if (max == null) max = newMax;
                if (min == null) min = newMin;

                if ((newMax.X - newMin.X < max.Value.X - min.Value.X && newMax.X - newMin.X < 100) ||
                    (newMax.Y - newMin.Y < max.Value.Y - min.Value.Y && newMax.Y - newMin.Y < 50))
                {
                    Console.WriteLine($"Second {i}");
                    PrintPoints(points, newMin.X, newMax.X, newMin.Y, newMax.Y);
                }
                if (newMax.X - newMin.X > max.Value.X - min.Value.X && newMax.Y - newMin.Y > max.Value.Y - min.Value.Y) break;
                min = newMin; max = newMax;
            }

            Console.WriteLine($"Done");
            Console.ReadKey();
        }
    }
}
