using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace Advent2018_common
{

    class MainClass
    {
        public static IEnumerable<int> Recipe()
        {
            var recipes = new List<int> { 3, 7 };
            yield return 3;
            yield return 7;

            int elfOne = 0;
            int elfTwo = 1;

            while (true) 
            {
                // Create new recipies
                var newMix = recipes[elfOne] + recipes[elfTwo];
                var newRecipe = newMix / 10;
                if (newRecipe != 0)
                {
                    recipes.Add(newRecipe);
                    yield return newRecipe;
                }
                newRecipe = newMix % 10;
                recipes.Add(newRecipe);
                yield return newRecipe;

                // Move
                elfOne = (elfOne + 1 + recipes[elfOne]) % recipes.Count;
                elfTwo = (elfTwo + 1 + recipes[elfTwo]) % recipes.Count;
            }
        }


        public static string Puzzle(int numberOfRecipesBefore)
        {
            var recipes = Recipe().GetEnumerator();
            for (int i=0; i<numberOfRecipesBefore; i++)
            {
                if (!recipes.MoveNext()) break;
            }
            var result = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                if (recipes.MoveNext()) result.Append($"{recipes.Current}");
                else break;
            }
            return result.ToString();
        }

        public static int PuzzlePattern(string pattern)
        {
            var recipes = Recipe().GetEnumerator();
            var recipeString = new StringBuilder("");
            for (int i=0; recipes.MoveNext(); i++)
            {
                recipeString.Append($"{recipes.Current}");
                if (recipeString.Length >= pattern.Length)
                {
                    char[] end = new char[pattern.Length];

                    recipeString.CopyTo(recipeString.Length - pattern.Length, end, 0, pattern.Length);
                    var endString = new string(end);

                    if (pattern.Equals(new string(end)))
                    {
                        // Console.WriteLine($"Pattern {pattern} is matching at {i}: {recipeString}");
                        return recipeString.Length - pattern.Length;
                    }
                }
            }
            return 0;
        }
        public static int PuzzlePattern2(string pattern)
        {
            var recipes = Recipe().GetEnumerator();
            var matches = new List<int>();  // Index into pattern where matches are found

            for (int i = 0; recipes.MoveNext(); i++)
            {
                var newMatches = new List<int>();
                if (recipes.Current == pattern[0]-'0')
                {
                    newMatches.Add(1); // Pattern matched, next character to match for is index 1
                }
                foreach (var m in matches)
                {
                    if (recipes.Current == pattern[m] - '0')
                    {
                        if (m == pattern.Length - 1) return i-pattern.Length+1;
                        newMatches.Add(m + 1);      // Still matching, check next character
                    }
                }
                matches = newMatches;
            }
            return 0;
        }

        public static void Main(string[] args)
        {
            //    string[] input = System.IO.File.ReadAllLines("../../testPart2.txt");

            Console.WriteLine($"Scores after 9: {Puzzle(9)}");
            Console.WriteLine($"Scores after 5: {Puzzle(5)}");
            Console.WriteLine($"Scores after 18: {Puzzle(18)}");
            Console.WriteLine($"Scores after 074501: {Puzzle(074501)}");

            Console.WriteLine(PuzzlePattern("51589"));
            Console.WriteLine(PuzzlePattern2("51589"));
            Console.WriteLine(PuzzlePattern("01245"));
            Console.WriteLine(PuzzlePattern2("01245"));
            Console.WriteLine(PuzzlePattern("92510"));
            Console.WriteLine(PuzzlePattern2("92510"));
            Console.WriteLine(PuzzlePattern("59414"));
            Console.WriteLine(PuzzlePattern2("59414"));
            Console.WriteLine(PuzzlePattern("074501"));
            Console.WriteLine(PuzzlePattern2("074501"));

            Console.ReadKey();
        }
    }
}
