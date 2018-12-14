using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace Advent2018_common
{

    class MainClass
    {
        public static string Puzzle(int numberOfRecipesBefore)
        {
            var recipes = new List<int> { 3, 7 };
            int elfOne = 0;
            int elfTwo = 1;

            for (int i = 0; i < numberOfRecipesBefore + 10; i++)
            {
                // Create new recipies
                var newMix = recipes[elfOne] + recipes[elfTwo];
                var newRecipe = newMix / 10;
                if (newRecipe != 0) recipes.Add(newRecipe);

                newRecipe = newMix % 10;
                recipes.Add(newRecipe);

                // Move
                elfOne = (elfOne + 1 + recipes[elfOne]) % recipes.Count;

                elfTwo = (elfTwo + 1 + recipes[elfTwo]) % recipes.Count;

                if (i<20)
                {
                    Console.Write($"{i} - {elfOne};{elfTwo}: ");
                    foreach (var r in recipes)
                    {
                        Console.Write($"{r} ");
                    }
                    Console.WriteLine();
                }
            }
            var result = new StringBuilder();
            for (int i=numberOfRecipesBefore; i<numberOfRecipesBefore+10; i++)
            {
                result.Append($"{recipes[i]}");
            }
            return result.ToString();
        }

        public static int PuzzlePattern(string pattern)
        {
            var recipes = new List<int> { 3, 7 };
            int elfOne = 0;
            int elfTwo = 1;

            var recipeString = new StringBuilder("37");

            for (int i=0; ; i++)
            {
                // Create new recipies
                var newMix = recipes[elfOne] + recipes[elfTwo];
                var newRecipe = newMix / 10;
                if (newRecipe != 0)
                {
                    recipes.Add(newRecipe);
                    recipeString.Append($"{newRecipe}");
                }

                newRecipe = newMix % 10;
                recipes.Add(newRecipe);
                recipeString.Append($"{newRecipe}");

                // Move
                elfOne = (elfOne + 1 + recipes[elfOne]) % recipes.Count;

                elfTwo = (elfTwo + 1 + recipes[elfTwo]) % recipes.Count;

                if (i < 20)
                {
                    Console.Write($"{i} - {elfOne};{elfTwo}: ");
                    foreach (var r in recipes)
                    {
                        Console.Write($"{r} ");
                    }
                    Console.WriteLine(recipeString);
                }
                
                if (recipeString.Length >=5)
                {
                    char[] end = new char[pattern.Length];

                    recipeString.CopyTo(recipeString.Length - pattern.Length, end, 0, pattern.Length);
                    var endString = new string (end);

                    if (pattern.Equals(new string(end) ))
                    {
                        Console.WriteLine($"Pattern {pattern} is matching at {i}: {recipeString}");
                        return recipeString.Length - pattern.Length;
                    }
                }
            }
        }

        public static void Main(string[] args)
        {
            //    string[] input = System.IO.File.ReadAllLines("../../testPart2.txt");

            Console.WriteLine($"Scores after 9: {Puzzle(9)}");
            Console.WriteLine($"Scores after 5: {Puzzle(5)}");
            Console.WriteLine($"Scores after 18: {Puzzle(18)}");
            Console.WriteLine($"Scores after 074501: {Puzzle(074501)}");

            Console.WriteLine(PuzzlePattern("51589"));
            Console.WriteLine(PuzzlePattern("01245"));
            Console.WriteLine(PuzzlePattern("92510"));
            Console.WriteLine(PuzzlePattern("59414"));
            Console.WriteLine(PuzzlePattern("074501"));


            Console.ReadKey();
        }
    }
}
