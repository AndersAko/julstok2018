using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

namespace Advent2018_common
{
    public class ArmyGroup
    {
        public int Number;
        public int HitPoints;
        public int AttackDamage;
        public string AttackType;
        public int Initiative;
        public bool Selected = false;

        public List<string> Weaknesses = new List<string>();
        public List<string> Immunities = new List<string>();

        public ArmyGroup Target = null;
        public virtual Int64 Power
        {
            get
            {
                return Number * AttackDamage;
            }
        }
        public ArmyGroup(int num, int hp, int dmg, string type, int initiative, string weaknesses)
        {
            Number = num;
            HitPoints = hp;
            AttackDamage = dmg;
            AttackType = type;
            Initiative = initiative;

            foreach (var weakness in weaknesses.TrimStart("( ".ToCharArray()).Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                var weakTypes = weakness.Split(" ,)".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (weakTypes[0] == "immune")
                {
                    for (int i = 2; i < weakTypes.Count(); i++)
                    {
                        Immunities.Add(weakTypes[i]);
                    }
                }
                else if (weakTypes[0] == "weak")
                {
                    for (int i = 2; i < weakTypes.Count(); i++)
                    {
                        Weaknesses.Add(weakTypes[i]);
                    }
                }
            }
        }
        public ArmyGroup(ArmyGroup fromCopy)
        {
            Number = fromCopy.Number;
            HitPoints = fromCopy.HitPoints;
            AttackDamage = fromCopy.AttackDamage;
            AttackType = fromCopy.AttackType;
            Initiative = fromCopy.Initiative;
            Weaknesses = new List<string>(fromCopy.Weaknesses);
            Immunities = new List<string>(fromCopy.Immunities);
        }
        public ArmyGroup Clone()
        {
            return (ArmyGroup)this.MemberwiseClone();
        }
        // Amount of damage dealt to target
        public virtual Int64 Damage(ArmyGroup target)
        {
            if (target.Immunities.Contains(AttackType)) return 0;
            if (target.Weaknesses.Contains(AttackType)) return Power * 2;
            else return Power;
        }
        public override string ToString()
        {
            return $"{Number} {HitPoints}hp Attack:{AttackDamage} {AttackType} Initiative:{Initiative} Weakness:{String.Join(",", Weaknesses)} Immune:{String.Join(",", Immunities)}";
        }
    }
    public class ImmuneSystem : ArmyGroup
    {
        public ImmuneSystem(int num, int hp, int dmg, string type, int initiative, string weaknesses) : base(num, hp, dmg, type, initiative, weaknesses)
        {

        }
        public ImmuneSystem(ImmuneSystem immune) : base(immune) { }

        public override Int64 Damage(ArmyGroup target)
        {
            if (target.Immunities.Contains(AttackType)) return 0;
            if (target.Weaknesses.Contains(AttackType)) return Power * 2;
            else return Power;
        }
        public override Int64 Power
        {
            get
            {
                return Number * (AttackDamage + MainClass.Boost);
            }
        }
    }
    public class Infection : ArmyGroup
    {
        public Infection(int num, int hp, int dmg, string type, int initiative, string weaknesses) : base(num, hp, dmg, type, initiative, weaknesses)
        {

        }
        public Infection(Infection infection) : base(infection) { }
    }
    public struct BoostLevel
    {
        public int level;
        public bool highEnough;
        public BoostLevel(int l, bool e)
        {
            level = l;
            highEnough = e;
        }
    }

    /*
     * Immune System:
       17 units each with 5390 hit points (weak to radiation, bludgeoning) with an attack that does 4507 fire damage at initiative 2
       989 units each with 1274 hit points (immune to fire; weak to bludgeoning, slashing) with an attack that does 25 slashing damage at initiative 3

       Infection:
       801 units each with 4706 hit points (weak to radiation) with an attack that does 116 bludgeoning damage at initiative 1
       4485 units each with 2961 hit points (immune to radiation; weak to fire, cold) with an attack that does 12 slashing damage at initiative 4
    */


    class MainClass
    {
        //static Regex groupDescriptionRx = new Regex(@"(?<num>\d+) units each with (?<hp>\d+) hit points (?<weakness>\((immune|weak) to[ ;,\w]+\))? with an attack that does (?<dmg>\d+) (?<type>\w) damage at initiative (?<init>\d+)");
        static Regex groupDescriptionRx = new Regex(@"(?<num>\d+) units each with (?<hp>\d+) hit points(?<weakness> \((immune|weak) to[ ;,\w]+\))? with an attack that does (?<dmg>\d+) (?<type>\w*) damage at initiative (?<init>\d+)");
        public static int Boost = 0;

        public static void Main(string[] args)
        {
            string[] input = System.IO.File.ReadAllLines("../../input.txt");

            bool infection = false;
            var OriginalReindeerSystem = new List<ArmyGroup>();

            foreach (var line in input)
            {
                if (line.StartsWith("Immune", StringComparison.Ordinal)) continue;
                if (line.StartsWith("Infection", StringComparison.Ordinal))
                {
                    infection = true;
                    continue;
                }
                Match match = groupDescriptionRx.Match(line);
                if (!match.Success) continue;
                ArmyGroup army;
                if (infection) army = new Infection(Convert.ToInt32(match.Groups["num"].ToString()), Convert.ToInt32(match.Groups["hp"].ToString()), Convert.ToInt32(match.Groups["dmg"].ToString()),
                                         match.Groups["type"].ToString(), Convert.ToInt32(match.Groups["init"].ToString()), match.Groups["weakness"].ToString());
                else army = new ImmuneSystem(Convert.ToInt32(match.Groups["num"].ToString()), Convert.ToInt32(match.Groups["hp"].ToString()), Convert.ToInt32(match.Groups["dmg"].ToString()),
                         match.Groups["type"].ToString(), Convert.ToInt32(match.Groups["init"].ToString()), match.Groups["weakness"].ToString());
                OriginalReindeerSystem.Add(army);

                Console.WriteLine($"Added: {army}");
            }

            List<ArmyGroup> ReindeerSystem;
            Boost = 0;
            ReindeerSystem = RunCombat(OriginalReindeerSystem);

            BoostLevel LowBoost = new BoostLevel(0, false);

            Boost = 10000;
            ReindeerSystem = RunCombat(OriginalReindeerSystem);
            BoostLevel HighBoost = new BoostLevel(10000, !ReindeerSystem.Any(a => a is Infection));

            do
            {
                ReindeerSystem = new List<ArmyGroup>(OriginalReindeerSystem.Select(a => a.Clone()));
                if (!HighBoost.highEnough)
                {
                    HighBoost.level += HighBoost.level - LowBoost.level;
                    Boost = HighBoost.level;
                    ReindeerSystem = RunCombat(ReindeerSystem);
                    HighBoost.highEnough = !ReindeerSystem.Any(a => a is Infection);
                }
                else if (LowBoost.highEnough)
                {
                    LowBoost.level -= HighBoost.level - LowBoost.level;
                    Boost = LowBoost.level;
                    ReindeerSystem = RunCombat(ReindeerSystem);
                    LowBoost.highEnough = !ReindeerSystem.Any(a => a is Infection);
                }
                else if (!LowBoost.highEnough && HighBoost.highEnough)
                {
                    Boost = HighBoost.level - (HighBoost.level - LowBoost.level) / 2;
                    ReindeerSystem = RunCombat(ReindeerSystem);
                    if (!ReindeerSystem.Any(a => a is Infection))
                    {
                        HighBoost.highEnough = true;
                        HighBoost.level = Boost;
                    }
                    else
                    {
                        LowBoost.highEnough = false;
                        LowBoost.level = Boost;
                    }
                }
                else Console.WriteLine($"Strange: {LowBoost}, {HighBoost}");
                Console.WriteLine($"Searching... {LowBoost.level} {LowBoost.highEnough} to {HighBoost.level} {HighBoost.highEnough}");
            } while (HighBoost.level - LowBoost.level > 1);

            Console.WriteLine($"Probably lowest boost value: {HighBoost.level}");
            for (int i=HighBoost.level -10 ; i < HighBoost.level+5; i++)
            {
                Boost = i;
                ReindeerSystem = RunCombat(OriginalReindeerSystem);
            }

            Console.ReadKey();
        }

        private static List<ArmyGroup> RunCombat(List<ArmyGroup> OriginalReindeerSystem)
        {
            var ReindeerSystem = new List<ArmyGroup>(OriginalReindeerSystem.Select(a => a.Clone()));
            int TotalUnits = ReindeerSystem.Sum(a => a.Number);

            int Round = 1;
            while (ReindeerSystem.Any(a => a is ImmuneSystem) && ReindeerSystem.Any(a => a is Infection))
            {
                //Console.WriteLine($"Round {Round++}");
                //Console.WriteLine("Immune System:");
                //foreach (var immune in ReindeerSystem.Where(a => a is ImmuneSystem))
                //{
                //    Console.WriteLine($"{immune}");
                //}
                //Console.WriteLine("Infection:");
                //foreach (var infections in ReindeerSystem.Where(a => a is Infection))
                //{
                //    Console.WriteLine($"{infections}");
                //}
                //Console.WriteLine();

                // Target selection
                foreach (var group in ReindeerSystem.OrderByDescending(g => g.Power).ThenByDescending(g => g.Initiative))
                {
                    var target = ReindeerSystem.Where(a => a.GetType() != group.GetType() && !a.Selected).OrderByDescending(a => group.Damage(a))
                                .ThenByDescending(a => a.Power).ThenByDescending(a => a.Initiative).FirstOrDefault();
                    if (target != null && group.Damage(target) == 0) target = null;
                    group.Target = target;
                    if (target != null) target.Selected = true;
                }

                // Atack phase
                foreach (var group in ReindeerSystem.OrderByDescending(g => g.Initiative))
                {
                    if (group.Target != null && group.Number >0)
                    {
                        //Console.WriteLine($"{group} attacked {group.Target}");
                        //Console.WriteLine($"  dealing {group.Damage(group.Target)} killing {group.Damage(group.Target) / group.Target.HitPoints} units of {group.Target.Number}.");
                        group.Target.Number -= (int)(group.Damage(group.Target) / (long)group.Target.HitPoints);
                    }
                }

                ReindeerSystem = ReindeerSystem.Where(a => a.Number > 0).ToList();
                foreach (var a in ReindeerSystem)
                {
                    a.Selected = false;
                }
                if (TotalUnits == ReindeerSystem.Sum(a => a.Number)) break;
                TotalUnits = ReindeerSystem.Sum(a => a.Number);
            }
            Console.WriteLine($"{Boost} - {ReindeerSystem.Count()} groups of {String.Join("; ",ReindeerSystem.GroupBy(a=>a.GetType()).Select(g => $"{g.First().GetType()} {g.Sum(a => a.Number)}"))} " +
            	$"remain with a total of {ReindeerSystem.Sum(a => a.Number)} units");
            return ReindeerSystem;
        }
    }
}
