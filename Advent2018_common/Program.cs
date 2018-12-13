using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace Advent2018_common
{

    class MainClass
    {
        class Cart
        {
            public (int X, int Y) Pos;
            public int Dir { get; set; }    // Up = 0, left = 1, ..
            internal int nextTurn;                   // 0 = left, 1 = straight, 2 = right

            public Cart(int row, int col, int dir)
            {
                Pos.X = col;
                Pos.Y = row;
                Dir = dir;
                nextTurn = 0;
            }
            public char Symbol()
            {
                return "^<v>".ToCharArray()[Dir];
            }
            public void NextPos(string[] tracks)
            {
                switch (Dir)
                {
                    case 0:
                        Pos.Y--; break;
                    case 1:
                        Pos.X--; break;
                    case 2:
                        Pos.Y++; break;
                    case 3:
                        Pos.X++; break;
                }
                if (tracks[Pos.Y][Pos.X] == '+')
                {
                    Dir = (Dir + 5 - nextTurn) % 4;
                    nextTurn = (nextTurn + 1) % 3;
                }
                else if (tracks[Pos.Y][Pos.X] == '/')
                {
                    Dir = (3 - Dir) % 4;
                }
                else if (tracks[Pos.Y][Pos.X] == '\\')
                {
                    Dir = (5 - Dir) % 4;
                }
            }
        }

        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../testPart2.txt");

            string[] tracks = new string[input.Length];    // Same format as input, but without carts
            var carts = new List<Cart>();

            for (int row = 0; row < input.Length; row++)
            {
                try
                {
                    int index = 0;
                    while (true)
                    {
                        var cartIndex = input[row].IndexOfAny("^<v>".ToCharArray(), index);
                        if (cartIndex == -1) break;
                        index = cartIndex + 1;
                        var dir = 0;
                        switch (input[row][cartIndex])
                        {
                            case '^':
                                dir = 0;
                                break;
                            case '<':
                                dir = 1;
                                break;
                            case 'v':
                                dir = 2;
                                break;
                            case '>':
                                dir = 3;
                                break;
                        }
                        carts.Add(new Cart(row, cartIndex, dir));
                    }
                }
                catch (ArgumentNullException) { };
                var line = input[row].Replace('^', '|').Replace('v', '|').Replace('<', '-').Replace('>', '-');
                tracks[row] = line;
            }

            Console.WriteLine("Initial position");
            for (int row = 0; row < tracks.Length; row++)
            {
                var line = tracks[row].ToCharArray();
                foreach (var cart in carts.Where(c => c.Pos.Y == row))
                {
                    line[cart.Pos.X] = cart.Symbol();
                }
                Console.WriteLine(new String(line));
            }

            while (carts.Count > 1)
            {
                foreach (var cart in carts.OrderBy(c => c.Pos.Y).OrderBy(c => c.Pos.X))
                {
                    cart.NextPos(tracks);

                    var collisionAt = carts.GroupBy(c => c.Pos).Where(g => g.Count() > 1);

                    if (collisionAt.Any())
                    {
                        for (int row = 0; row < tracks.Length; row++)
                        {
                            var line = tracks[row].ToCharArray();
                            var cartText = new StringBuilder();
                            foreach (var c in carts.Where(c => c.Pos.Y == row).OrderBy(c => c.Pos.X))
                            {
                                line[c.Pos.X] = c.Symbol();
                                cartText.Append($"({c.Pos.X},{c.Pos.Y}): {c.Symbol()} {c.nextTurn}   ");
                            }
                            Console.WriteLine(new String(line) + cartText.ToString());
                        }

                        var crashSite = collisionAt.First().Key;

                        Console.WriteLine($"Bang! at {crashSite.X}, {crashSite.Y}");
                        foreach (var c in collisionAt.First())
                        {
                            carts.Remove(c);
                        }
                        Console.WriteLine($"{carts.Count} carts remaining");
                    }
                }  
            }
            Console.WriteLine($"One cart remaining at {carts.First().Pos.X}, {carts.First().Pos.Y}");

            Console.ReadKey();
        }
    }
}
