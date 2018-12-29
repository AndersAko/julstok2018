using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

namespace Advent2018_common
{
    enum SoilType { sand, moist, drop, water, clay };
    // TODO: Replace with extension to ValueTuple to support addition...
    struct Drop
    {
        public int X;
        public int Y;
        Drop((int X, int Y) from)
        {
            this.X = from.X;
            this.Y = from.Y;
        }
        public static Drop operator +(Drop a, (int X, int Y) b)
        {
            return new Drop((a.X + b.X, a.Y + b.Y));
        }
        public static implicit operator ValueTuple<int, int>(Drop drop)
        {
            return (drop.X, drop.Y);
        }
        public static implicit operator Drop(ValueTuple<int, int> tuple)
        {
            return new Drop(tuple);
        }
        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }


    class BelowGround
    {
        Dictionary<(int X, int Y), SoilType> ground = new Dictionary<(int X, int Y), SoilType>();
        readonly int MinY;
        readonly int MaxY;
        int MinX { get { return ground.Min(s => s.Key.X); } }
        int MaxX { get { return ground.Max(s => s.Key.X); } }

        public SoilType this[(int X, int Y) ix]
        {
            get
            {
                if (ground.ContainsKey(ix)) return ground[ix];
                return SoilType.sand;
            }
            set
            {
                ground[ix] = value;
            }
        }
        public BelowGround(string[] input)
        {
            foreach (var line in input)
            {
                var split = line.Split(",= .".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (split[0] == "x")
                {
                    var x = Convert.ToInt32(split[1]);
                    for (int y = Convert.ToInt32(split[3]); y <= Convert.ToInt32(split[4]); y++)
                    {
                        ground[(x, y)] = SoilType.clay;
                    }
                }
                else if (split[0] == "y")
                {
                    var y = Convert.ToInt32(split[1]);
                    for (int x = Convert.ToInt32(split[3]); x <= Convert.ToInt32(split[4]); x++)
                    {
                        ground[(x, y)] = SoilType.clay;
                    }
                }
            }
            MaxY = ground.Max(s => s.Key.Y);
            MinY = ground.Min(s => s.Key.Y);
        }
        public (int, int) WetSquares()
        {
            return (ground.Count(s => s.Key.Y >= MinY && s.Key.Y <= MaxY && (s.Value == SoilType.moist || s.Value == SoilType.water)),
                    ground.Count(s => s.Key.Y >= MinY && s.Key.Y <= MaxY && s.Value == SoilType.water));
        }
        public void PrintGround()
        {
            var minX = MinX;
            var maxX = MaxX;

            string symbol = ".|d~#";
            for (var xrow = 100; xrow >= 1; xrow /= 10)
            {
                Console.Write($"    ");
                for (int x = minX - 5; x < maxX + 5; x++)
                {
                    Console.Write((x / xrow) % 10);
                }
                Console.WriteLine();
            }
            for (int y = MinY; y <= MaxY; y++)
            {
                Console.Write($"{y,3} ");
                for (int x = minX - 5; x < maxX + 5; x++)
                {
                    SoilType square = this[(x, y)];
                    Console.Write(symbol[(int)square]);
                }
                Console.WriteLine();
            }
        }
        static HashSet<Drop> CompletedSprings = new HashSet<Drop>();
        // Add a splash of water, falling down until it settles, repeating until full
        public bool SplashWater(List<(int X, int Y)> springHistory)
        {
            Console.WriteLine($"Splash at {springHistory.Last()}");

            Drop drop = springHistory.Last();
            if (CompletedSprings.Contains(drop)) return false;      // been there, done that...

            if (this[drop] == SoilType.water)
            {
                springHistory.RemoveAt(springHistory.Count - 1);
                Console.WriteLine("Spring is waterlogged, back up to previous spring and try again...");

                return SplashWater(springHistory);
            }
            this[drop] = SoilType.moist;

            // Drop down until it hits clay or water
            while (this[drop + (0, 1)] != SoilType.clay && this[drop + (0, 1)] != SoilType.water)
            {
                if (drop.Y > MaxY)
                {
                    CompletedSprings.Add(springHistory.Last());
                    //PrintGround();
                    return false;
                }

                drop += (0, 1);
                this[drop] = SoilType.moist;
            }

            // Expand to left and right until it hits a wall or loses support from clay or water 
            Drop leftLimit = drop;
            for (int dx = -1; this[drop + (dx, 0)] != SoilType.clay &&
                            (this[drop + (dx, 1)] == SoilType.clay || this[drop + (dx, 1)] == SoilType.water); dx--)
            {
                if (this[drop + (dx, 0)] != SoilType.sand && this[drop + (dx, 0)] != SoilType.moist)
                {
                    Console.WriteLine($"Surprise: square at {drop + (dx, 0)} was {this[drop + (dx, 0)]}");
                }
                this[drop + (dx, 0)] = SoilType.moist;
                leftLimit = drop + (dx, 0);
            }
            Drop rightLimit = drop;
            for (int dx = 1; this[drop + (dx, 0)] != SoilType.clay &&
                            (this[drop + (dx, 1)] == SoilType.clay || this[drop + (dx, 1)] == SoilType.water); dx++)
            {
                if (this[drop + (dx, 0)] != SoilType.sand && this[drop + (dx, 0)] != SoilType.moist)
                {
                    Console.WriteLine($"Surprise: square at {drop + (dx, 0)} was {this[drop + (dx, 0)]}");
                }
                this[drop + (dx, 0)] = SoilType.moist;
                rightLimit = drop + (dx, 0);
            }
            if (this[leftLimit + (-1, 0)] == SoilType.clay && this[rightLimit + (1, 0)] == SoilType.clay)
            {
                // Walls on either side, fill with water and return
                for (int x = leftLimit.X; x <= rightLimit.X; x++)
                {
                    this[(x, drop.Y)] = SoilType.water;
                }
                return SplashWater(springHistory);
            }
            // Overflows on either or both sides
            bool SettlesAfterOverflow = false;
            if (this[leftLimit + (-1, 0)] != SoilType.clay)
            {
                var newSpring = new List<(int X, int Y)>(springHistory);
                newSpring.Add(leftLimit + (-1, 0));

                if (SplashWater(newSpring))
                {
                    SettlesAfterOverflow = true;
                }
                else
                {
                    CompletedSprings.Add(springHistory.Last());
                }
            }
            if (this[rightLimit + (1, 0)] != SoilType.clay)
            {
                var newSpring = new List<(int X, int Y)>(springHistory);
                newSpring.Add(rightLimit + (1, 0));
                if (SplashWater(newSpring))
                {
                    SettlesAfterOverflow = true;
                }
                else
                {
                    CompletedSprings.Add(springHistory.Last());
                }
            }
            return SettlesAfterOverflow;
        }
    }
    class MainClass
    {
        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");

            var ground = new BelowGround(input);
            ground.PrintGround();

            List<(int X, int Y)> Springs = new List<(int X, int Y)> { (500, 0) };
            ground.SplashWater(Springs);

            ground.PrintGround();
            Console.WriteLine($"Total wet squares: {ground.WetSquares()}");

            Console.ReadKey();
        }


    }
}
