using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

namespace Advent2018_common
{
    class Room
    {
        public List<Room> Adjacent = new List<Room>();
        public Position label;
        public Room(Position label)
        {
            this.label = label;
        }
        public override string ToString()
        {
            return $"{label}: {String.Join(";", Adjacent.Select(r => r.label))}";
        }

    }
    class Position
    {
        int X, Y;
        Position((int X, int Y) pos)
        {
            X = pos.X;
            Y = pos.Y;
        }
        public static implicit operator Position((int X, int Y) pos) => new Position(pos);
        public static implicit operator (int X, int Y) (Position pos) => (pos.X, pos.Y);
        public static Position operator +(Position a, Position b) => new Position((a.X + b.X, a.Y + b.Y));
        //public static bool operator ==(Position a, Position b) => a.X == b.X && a.Y == b.Y;
        //public static bool operator !=(Position a, Position b) => a.X != b.X || a.Y != b.Y;

        public override string ToString() => $"({X},{Y})";
    }

    class MainClass
    {
        static void PrintRooms(Dictionary<(int X, int Y), Room> rooms)
        {
            for (int y = rooms.Min(r => r.Key.Y); y <= rooms.Max(r => r.Key.Y); y++)
            {

                for (int row = 0; row <= 1; row++)
                {
                    for (int x = rooms.Min(r => r.Key.X); x <= rooms.Max(r => r.Key.X); x++)
                    {
                        if (rooms.ContainsKey((x, y)))
                        {
                            if (row == 0 && rooms[(x, y)].Adjacent.Any(r => ((ValueTuple<int, int>)r.label) == (x, y - 1)))
                            {
                                Console.Write("| ");
                            }
                            else if (row == 1)
                            {
                                if ((x, y) == (0, 0)) Console.Write('X'); else Console.Write(".");

                                if (rooms[(x, y)].Adjacent.Any(r => ((ValueTuple<int, int>)r.label) == (x + 1, y))) Console.Write('-');
                                else Console.Write(' ');
                            }
                            else Console.Write("  ");
                        }
                        else
                        {
                            Console.Write("  ");
                        }

                    }
                    Console.WriteLine();
                }
            }

        }
        static Dictionary<(int X, int Y), Room> GenerateMap(string input)
        {
            if (input.Length < 100) Console.WriteLine("Generating map from: " + input);
            var directionMap = new Dictionary<char, Position> { { 'E', (1, 0) }, { 'S', (0, 1) }, { 'W', (-1, 0) }, { 'N', (0, -1) } };

            // Current position or positions
            var positions = new List<(int X, int Y)> { (0, 0) };

            var savedPositions = new Stack<List<(int X, int Y)>>();

            var rooms = new Dictionary<(int X, int Y), Room>();
            Room starting = new Room((0, 0));
            rooms[(0, 0)] = starting;

            foreach (var direction in input)
            {
                switch (direction)
                {
                    case '^':
                        continue;
                    case '$':
                        continue;
                    case '(':
                        savedPositions.Push(positions);
                        break;
                    case '|':
                        /* Option 1: Multiple branches
                        var secondBranch = savedPositions.Pop();
                        savedPositions.Push(positions);
                        positions = secondBranch; 
                        */
                        /* Option 2: Continue after end of second branch */
                        positions = savedPositions.Peek();

                        /* Option 3: Continue from before branching 
                        positions = savedPositions.Peek();
                        */
                        break;
                    case ')':
                        /* Option 1: Multiple branches
                        var firstBranch = savedPositions.Pop();
                        positions = positions.Union(firstBranch).ToList();
                        */
                        /* Option 2: Continue after end of second branch */
                        savedPositions.Pop();

                        /* Option 3: Continue from before branching 
                        positions = savedPositions.Pop();
                        */
                        break;
                    case 'E':
                    case 'W':
                    case 'S':
                    case 'N':
                        var newPositions = new List<(int X, int Y)>();
                        foreach (var position in positions)
                        {
                            var current = rooms[position];
                            (int X, int Y) newPosition = position + directionMap[direction];
                            if (!rooms.ContainsKey(newPosition))
                            {
                                //Console.WriteLine($"Adding room at {newPosition}");
                                rooms[newPosition] = new Room(newPosition);
                            }
                            if (!current.Adjacent.Contains(rooms[newPosition]))
                            {
                                //Console.WriteLine($"Adding doors coming from {direction} to {newPosition}. First: {current} going to {rooms[newPosition]}");
                                current.Adjacent.Add(rooms[newPosition]);
                                rooms[newPosition].Adjacent.Add(current);
                            }
                            newPositions.Add(newPosition);
                        }
                        positions = newPositions;
                        break;
                    default:
                        Console.WriteLine($"Other character in input: {direction}");
                        break;
                }
            }
            Console.WriteLine($"Remaining on the stack after geenration: {savedPositions.Count()}");
            return rooms;
        }

        class Path
        {
            public Room room;
            public List<Position> path = new List<Position>();
            public Path(Room room) { this.room = room; }
            public Path(Room room, List<Position> pathHere)
            {
                this.room = room;
                path = new List<Position>(pathHere)
                {
                    room.label
                };
            }
            public override string ToString() => $"{room.label}";
        }
        static (int, int) FurthestShortPath(Dictionary<(int X, int Y), Room> rooms)
        {
            var visited = new HashSet<Position>();
            var toVisit = new LinkedList<Path>();
            toVisit.AddFirst(new Path(rooms[(0, 0)]));

            var numRooms = rooms.Count();
            var roomsWithAtLeast1000Doors = 0;
            while (toVisit.Any())
            {
                var path = toVisit.First();
                toVisit.Remove(path);
                if (visited.Contains(path.room.label)) continue;
                visited.Add(path.room.label);

                if (path.path.Count() >= 1000) roomsWithAtLeast1000Doors++;
                if (visited.Count() == numRooms)
                {
                    Console.WriteLine($"Been to all rooms, shortest path to the last room is {path.path.Count}");
                    Console.WriteLine($"Remaining rooms to visit: {toVisit.Count()}");
                    return (path.path.Count, roomsWithAtLeast1000Doors);
                }
                var newRooms = path.room.Adjacent.Where(r => !visited.Contains(r.label));
                foreach (var room in newRooms.Select(r => new Path(r, path.path)))
                {
                    toVisit.AddLast(room);
                }
            }
            return (0,0);
        }
        public static void Main(string[] args)
        {
            //string[] input = System.IO.File.ReadAllLines("../../input.txt");

            var rooms = GenerateMap("^ENWWW(NEEE|SSE(EE|N))$");
            PrintRooms(rooms);
            Console.WriteLine($"Shortest path: {FurthestShortPath(rooms)} should be 10");

            rooms = GenerateMap("^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$");
            PrintRooms(rooms);
            Console.WriteLine($"Shortest path: {FurthestShortPath(rooms)} should be 18");

            rooms = GenerateMap("^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$");
            PrintRooms(rooms);
            Console.WriteLine($"Shortest path: {FurthestShortPath(rooms)} should be 23");

            rooms = GenerateMap("^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$");
            PrintRooms(rooms);
            Console.WriteLine($"Shortest path: {FurthestShortPath(rooms)} (should be 31)");

            rooms = GenerateMap("^E(NN|S)E$");
            PrintRooms(rooms);
            Console.WriteLine($"Shortest path: {FurthestShortPath(rooms)} (should be 4)");

            rooms = GenerateMap("^EEE(S|N)EEEEEEEEEEESSWWWWWWWWWWWN$");
            PrintRooms(rooms);
            Console.WriteLine($"Shortest path: {FurthestShortPath(rooms)} (should be 16)");

            rooms = GenerateMap("^EEE(NN|SSS)EEE$");
            PrintRooms(rooms);
            Console.WriteLine($"Shortest path: {FurthestShortPath(rooms)} (should be 9)");

            rooms = GenerateMap("^E(N|SS)EEE(E|SSS)$");
            PrintRooms(rooms);
            Console.WriteLine($"Shortest path: {FurthestShortPath(rooms)} (should be 9)");

            var input = System.IO.File.ReadAllLines("../../day20.txt");
            rooms = GenerateMap(input[0]);
            PrintRooms(rooms);
            Console.WriteLine($"Shortest path: {FurthestShortPath(rooms)} should be 4778");

            input = System.IO.File.ReadAllLines("../../input.txt");
            rooms = GenerateMap(input[0]);
            PrintRooms(rooms);
            Console.WriteLine($"Shortest path: {FurthestShortPath(rooms)}");
            Console.ReadKey();

        }
    }
}
