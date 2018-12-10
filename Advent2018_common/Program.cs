using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent2018_common
{

    class Point
    {
        public int x, y;
        public int velX, velY;
        public Point(int x, int y, int velX, int velY)
        {
            this.x = x;
            this.y = y;
            this.velX = velX;
            this.velY = velY;
        }
        public override string ToString()
        {
            return $"({x},{y})"; 
        }

        public  Point (string input)
        {
            var split = input.Split(" <>=,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
            this.x = Convert.ToInt32(split[1]);
            this.y = Convert.ToInt32(split[2]);
            this.velX = Convert.ToInt32(split[4]);
            this.velY = Convert.ToInt32(split[5]);
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
                    if (points.Exists(p => p.x == x && p.y == y)) Console.Write("#");
                    else Console.Write(".");
                }
                Console.WriteLine();
            }
        }
        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../test.txt");

            var points = new List<Point>();
            int minX=0, maxX=0, minY=0, maxY = 0;

            foreach (var line in input)
            {
                var point = new Point(line);
                minX = Math.Min(minX, point.x);
                maxX = Math.Max(maxX, point.x);
                minY = Math.Min(minY, point.y);
                maxY = Math.Max(maxY, point.y);

                points.Add(point);                    
            }
            Console.WriteLine("Second 0"); 
//            PrintPoints(points, minX, maxX, minY, maxY);

            for (int i=0; i<1000000; i++)
            {
                int newMinX = Int32.MaxValue, newMaxX = Int32.MinValue, newMinY = Int32.MaxValue, newMaxY = Int32.MinValue;
                foreach (var point in points)
                {
                    string oldPoint = point.ToString();

                    point.x += point.velX;
                    point.y += point.velY;
                    newMinX = Math.Min(newMinX, point.x);
                    newMaxX = Math.Max(newMaxX, point.x);
                    newMinY = Math.Min(newMinY, point.y);
                    newMaxY = Math.Max(newMaxY, point.y);
                }
                if ((newMaxX - newMinX < maxX - minX && newMaxX - newMinX < 100) || (newMaxY-newMinY < maxY - minY && newMaxY-newMinY < 50))
                {
                    Console.WriteLine($"Second {i}");
                    PrintPoints(points, newMinX, newMaxX, newMinY, newMaxY);
                }
                if (newMaxX - newMinX > maxX - minX && newMaxY - newMinY > maxY - minY) break;
                minX = newMinX; maxX = newMaxX; minY = newMinY; maxY = newMaxY;
            }

            Console.WriteLine($"{0} ");
            Console.ReadKey();
        }
    }
}
