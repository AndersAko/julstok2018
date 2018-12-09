using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Advent2018_common
{


    class MainClass
    {
        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");

            foreach (var line in input)
            {
                var numPlayers = Convert.ToInt32(line.Split(" :;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0]);
                var lastMarble = Convert.ToInt32(line.Split(" :;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[6]);

                Int64[] scores = new Int64[numPlayers];
                var marbleCircle = new LinkedList<int>();
                marbleCircle.AddFirst(0);

                var currentMarble = marbleCircle.First;
                var player = 0;

                for (int marbleToPlace = 1; marbleToPlace <= lastMarble *100 ; marbleToPlace++)
                {
                    // If mod 23: Score process
                    if (marbleToPlace % 23 == 0)
                    {
                        scores[player] += marbleToPlace;
                        for (int i = 0; i < 7; i++)
                        {
                            currentMarble = currentMarble.Previous ?? marbleCircle.Last;
                        }

                        scores[player] += currentMarble.Value;
                        var newCurrent = currentMarble.Previous;
                        marbleCircle.Remove(currentMarble);
                        currentMarble = newCurrent.Next ?? marbleCircle.First;
                    }
                    else
                    // Else place marble after 1 marble after current
                    {
                        currentMarble = currentMarble.Next ?? marbleCircle.First;

                        var newMarble = new LinkedListNode<int>(marbleToPlace);
                        marbleCircle.AddAfter(currentMarble, newMarble);
                        currentMarble = newMarble;
                    }
                    //Console.Write($"[{marbleToPlace}, {player}]: ");
                    //foreach (var marble in marbleCircle)
                    //{
                    //    if (marble == currentMarble.Value) Console.Write($"({marble}) ");
                    //    else Console.Write($" {marble}  ");
                    //}
                    //Console.WriteLine($"Highscore: { scores.Max()}");

                    player = (player + 1) % numPlayers;
                }
                Console.WriteLine($"{numPlayers} players; last marble is worth {lastMarble} points: high score is {scores.Max()}   ---- {line}");
            }
            Console.ReadKey();
        }
    }
}
