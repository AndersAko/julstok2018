using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent2018_common
{
    class Claim
    {
        public int id;
        public int startx;
        public int starty;
        public int widthx;
        public int heighty;

        // Construct from string like: "#9 @ 593,72: 14x22"
        public Claim(string input)
        {
            var coords_str = input.Split(new char[] { '#', ' ', '@', ':', 'x', ',' }, StringSplitOptions.RemoveEmptyEntries);
            var coords = coords_str.Select(x => { int result;  var success = Int32.TryParse(x, out result); return success ? result : 0; }).ToArray();
            id = coords[0];
            startx = coords[1];
            starty = coords[2];
            widthx = coords[3];
            heighty = coords[4];
        }
        public override string ToString()
        {
            return $"{id}: {startx},{starty}:{widthx}x{heighty}";
        }
        public Tuple<int, int>[] Overlaps(Claim two)
        {
            if (this.startx>= two.startx+two.widthx || this.starty >= two.starty+two.heighty ||
                two.startx >= this.startx + this.widthx || two.starty >= this.starty + this.heighty)
            {
                return new Tuple<int, int>[0];
            }
            var result = new List<Tuple<int, int>>();
            for (int x = this.startx; x<this.startx+this.widthx; x++)
            {
                for (int y=this.starty; y<this.starty+this.heighty; y++)
                {
                    if (x>=two.startx && x < two.startx + two.widthx && y>=two.starty & y<two.starty+two.heighty)
                    {
                        result.Add(new Tuple<int, int>(x, y));
                    }
                }
            }
            return result.ToArray();
        }
    }

    class MainClass
    {

        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");
            var claims = new List<Claim>();
            foreach (var line in input)
            {
                var c = new Claim(line);
                claims.Add(c);
            }

            var overlaps = new HashSet<Tuple<int, int>>();
            foreach (var claim in claims)
            {
                bool doesOverlap = false;
                foreach (var claimTwo in claims)
                {
                    if (claim != claimTwo)
                    {
                        //Console.Write($"{claim} and {claimTwo} overlaps at:");
                        foreach (var overlap in claim.Overlaps(claimTwo))
                        {
                            //Console.Write($"{overlap}");
                            overlaps.Add(overlap);
                            doesOverlap = true;
                        }
                        //Console.WriteLine();
                    }
                }
                if (!doesOverlap)
                {
                    Console.WriteLine($"Claim: {claim} has no overlap with any other.");
                }
            }
            Console.WriteLine($"Overlaps: {overlaps.Count()}");
        }
    }
}
