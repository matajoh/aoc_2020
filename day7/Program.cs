using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;


namespace day7
{
    public record BagCount(int Count, string Name);

    public record BagRule(string Name, List<BagCount> Contents);

    static class Program
    {
        static BagRule parseRule(string rule)
        {
            var index = rule.IndexOf("bags contain");
            var bag = rule.Substring(0, index).Trim();
            var contents = rule.Substring(index + 12).Trim();
            var bags = new List<BagCount>();
            if(contents.Equals("no other bags."))
            {
                return new BagRule(bag, bags);
            }

            contents = contents.Replace('.', ',');
            int start = 0;
            while(start < contents.Length)
            {
                int end = contents.IndexOf("bag", start);
                var desc = contents.Substring(start, end - start).Trim();
                var parts = desc.Split();
                int count = int.Parse(parts[0]);
                bags.Add(new BagCount(count, string.Join(" ", parts.Skip(1))));
                start = contents.IndexOf(',', end) + 1;
            }

            return new BagRule(bag, bags);
        }

        static bool contains(Dictionary<string, BagRule> rules, string current, string query)
        {
            if(current == query)
            {
                return true;
            }

            return rules[current].Contents.Any(bag => contains(rules, bag.Name, query));
        }

        static void Part1(Dictionary<string, BagRule> rules)
        {
            string query = "shiny gold";
            int count = rules.Where(rule => rule.Key != query)
                             .Count(rule => contains(rules, rule.Key, query));
            Console.WriteLine("Part 1: {0}", count);
        }
    
        static int count(Dictionary<string, BagRule> rules, string current)
        {
            return rules[current].Contents.Sum(bag => bag.Count * count(rules, bag.Name)) + 1;
        }

        static void Part2(Dictionary<string, BagRule> rules)
        {
            Console.WriteLine("Part 2: {0}", count(rules, "shiny gold") - 1);
        }

        static void Main(string[] args)
        {
            var rules = File.ReadLines(args[0])
                            .Select(rule => parseRule(rule))
                            .ToDictionary(rule => rule.Name);
            Part1(rules);
            Part2(rules);
        }
    }
}
