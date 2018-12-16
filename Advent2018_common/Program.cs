using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace Advent2018_common
{
    abstract class Unit
    {
        struct Route
        {
            internal (int X, int Y) endPos;
            internal List<(int X, int Y)> path;
            internal Route((int X, int Y) endPos, List<(int X, int Y)> path)
            {
                this.endPos = endPos;
                this.path = new List<(int X, int Y)>(path);
                this.path.Add(endPos);
            }
            internal Route((int X, int Y) endPos) : this(endPos, new List<(int X, int Y)>()) { }
            public override string ToString()
            {
                return $"{path.Count}:{endPos.Y}.{endPos.X}";
            }
        }
        public (int X, int Y) Pos;
        public int hp;
        public Unit(int X, int Y)
        {
            hp = 200;
            this.Pos = (X, Y);
        }
        public Unit (Unit unitToCopy)
        {
            Pos = unitToCopy.Pos;
            hp = 200;
        }
        public abstract char Symbol();
        // Move this unit to the closest reachable unit and return list of potential melee targets
        public List<Unit> MoveAndFindTargets(List<string> maze, List<Unit> units)
        {
            // Already in range, no need to move
            var meleeUnits = FindMeleeUnits(units);
            if (meleeUnits.Any()) return meleeUnits;

            // Otherwise, look for closest path to closest target
            // Console.WriteLine($"Finding path for {this}");
            var movesToSearch = new SortedList<string, Route>();
            foreach (var newMove in PossibleMoves(Pos, maze, units).Select(m => new Route(m))) {
                movesToSearch[newMove.ToString()] = newMove;
            }
            var examinedRoutes = new HashSet<(int X, int Y)>();

            while (movesToSearch.Any())
            {
                // Pick next position in list
                var checkPosition = movesToSearch.First();
                movesToSearch.RemoveAt(0);
                examinedRoutes.Add(checkPosition.Value.endPos);

                //Console.WriteLine($"Checking {checkPosition.Value}");

                // If Adjacent to another unit, return a list of all melee units
                var closestUnits = units.Where(u => u.GetType() != this.GetType())
                                        .Where(u => new List<(int X, int Y)> { (0, -1), (-1, 0), (1, 0), (0, 1) }
                                                        .Any(delta => u.Pos.X == checkPosition.Value.endPos.X + delta.X && u.Pos.Y == checkPosition.Value.endPos.Y + delta.Y));
                if (closestUnits.Any())
                {
                    Pos = checkPosition.Value.path[0];    // First step
                    return closestUnits.ToList();
                }
                // Find possible moves from here and add to list
                foreach (var newMove in PossibleMoves(checkPosition.Value.endPos, maze, units)
                                       .Where(newMove => !examinedRoutes.Contains(newMove))
                                       .Select(m => new Route(m, checkPosition.Value.path)))
                {
                    movesToSearch[newMove.ToString()] = newMove;
                }
            }
            //Console.WriteLine($"No units are reachable from {this}");
            //MainClass.PrintMaze(maze, units);
            return new List<Unit>();

        }
        static List<(int X, int Y)> PossibleMoves((int X, int Y) pos, List<string> maze, List<Unit> units)
        {
            var result = new List<(int X, int Y)>();
            foreach (var delta in new List<(int X, int Y)> { (0, -1), (-1, 0), (1, 0), (0, 1) })
            {
                if (maze[pos.Y + delta.Y][pos.X + delta.X] == '.' && !units.Any(u => u.Pos.X == pos.X + delta.X && u.Pos.Y == pos.Y + delta.Y))
                {
                    result.Add((pos.X + delta.X, pos.Y + delta.Y));
                }
            }
            return result;
        }
        public List<Unit> FindMeleeUnits(List<Unit> units)
        {
            var closestUnits = units.Where(u => u.GetType() != this.GetType())
                                    .Where(u => new List<(int X, int Y)> { (0, -1), (-1, 0), (1, 0), (0, 1) }
                                                    .Any(delta => u.Pos.X == Pos.X + delta.X && u.Pos.Y == Pos.Y + delta.Y));
            //if (closestUnits.Any()) Console.WriteLine($"The {this.GetType()} at {Pos} can directly attack {String.Join(",", closestUnits)}");
            return closestUnits.ToList();
        }
        public override string ToString()
        {
            return $"{Symbol()}{Pos}";
        }
    }
    class Elf : Unit
    {
        public Elf(int X, int Y) : base(X, Y)
        {
        }
        public Elf (Elf e) : base(e) { }
        public override char Symbol() { return 'E'; }
    }
    class Goblin : Unit
    {
        public Goblin(int X, int Y) : base(X, Y)
        {

        }
        public Goblin(Goblin g) : base(g) { }
        public override char Symbol() { return 'G'; }
    }

    class MainClass
    {
        public static void PrintMaze(List<string> maze, List<Unit> units)
        {
            for (int i = 0; i < maze.Count; i++)
            {
                var line = new StringBuilder(maze[i]);
                foreach (var unitInLine in units.Where(u => u.Pos.Y == i))
                {
                    line[unitInLine.Pos.X] = unitInLine.Symbol();
                    line.Append($"  {unitInLine}:{unitInLine.hp}");
                }
                Console.WriteLine(line.ToString());
            }
            Console.WriteLine();
        }
        public static int RunCombat(List<string> maze, List <Unit> units, int elfAttackPower, bool acceptElfDeath)
        {
            var round = 1;
            for (; ; round++)
            {
                foreach (var unit in units.OrderBy((u) => u.Pos.Y).ThenBy(u => u.Pos.X).ToList())
                {
                    if (unit.hp <= 0)
                        continue;
                    if (!units.Any(u => u.GetType() != unit.GetType())) goto combat_complete;
                    var oldPosition = unit.Pos;
                    var targets = unit.MoveAndFindTargets(maze, units);

                    var meleeUnits = unit.FindMeleeUnits(units);
                    if (meleeUnits.Any())
                    {

                        var unitToAttack = meleeUnits.OrderBy(u => u.hp).ThenBy(u => u.Pos.Y).ThenBy(u => u.Pos.X).First();
                        if (unit is Goblin)
                        {
                            unitToAttack.hp -= 3;
                            if (unitToAttack.hp <= 0 && !acceptElfDeath)
                            {
                                Console.WriteLine("An elf dies, we cannot have that!");
                                return -1;    // No elf may be harmed!
                            }
                        }
                        else
                        {
                            unitToAttack.hp -= elfAttackPower;
                        }
                        if (unitToAttack.hp <= 0)
                        {
                            units.Remove(unitToAttack);
                        }
                    }
                }
                Console.WriteLine($"After {round} round(s):");
                PrintMaze(maze, units);
            }
        combat_complete:
            round--;    // Last round was not complete
            Console.WriteLine($"Combat ended after {round} rounds with attack power of {elfAttackPower}: ");
            Console.WriteLine($"{units.Count} units remaining with a total of {units.Sum(u => u.hp)}");
            Console.WriteLine($"Outcome: {units.Sum(u => u.hp) * round}");
            return units.Sum(u => u.hp) * round;
        }
        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");
            var maze = new List<string>(); // # = wall .=open
            var initialUnits = new List<Unit>();

            for (int i = 0; i < input.Length; i++)
            {
                var line = input[i];
                int index = -1;
                while ((index = line.IndexOf('G', index + 1)) != -1)
                {
                    var goblin = new Goblin(index, i);
                    initialUnits.Add(goblin);
                }
                while ((index = line.IndexOf('E', index + 1)) != -1)
                {
                    var elf = new Elf(index, i);
                    initialUnits.Add(elf);
                }
                line = line.Replace('G', '.').Replace('E', '.');
                maze.Add(line);
            }

            Console.WriteLine("Initial position:");
            PrintMaze(maze, initialUnits);
            var outcome = RunCombat(maze, initialUnits, 3, true);
            Console.WriteLine($"With default power of 3 and accepting deaths, the outcome was {outcome}:");
            PrintMaze(maze, initialUnits);


            outcome = -1;
            List<Unit> units = null; 
            for (int power=4; outcome == -1; power++)
            {
                units = new List<Unit>();
                for (int i = 0; i < input.Length; i++)
                {
                    var line = input[i];
                    int index = -1;
                    while ((index = line.IndexOf('G', index + 1)) != -1)
                    {
                        var goblin = new Goblin(index, i);
                        units.Add(goblin);
                    }
                    while ((index = line.IndexOf('E', index + 1)) != -1)
                    {
                        var elf = new Elf(index, i);
                        units.Add(elf);
                    }
                }
                outcome = RunCombat(maze, units, power, false);
            }
            Console.WriteLine($"At lowest increased power, the outcome was {outcome}.");
            PrintMaze(maze, units);
            Console.ReadKey();
        }
    }
}
