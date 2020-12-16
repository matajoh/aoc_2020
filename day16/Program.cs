using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace day16
{
    record Range(int Min, int Max);
    record Rule(string Name, Range Low, Range High);
    record Tickets(int[] Mine, List<int[]> Nearby);
    static class Program
    {
        static Range ParseRange(string range)
        {
            var parts = range.Split('-');
            return new Range(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        static Rule ParseRule(string rule)
        {
            int start = 0;
            int end = rule.IndexOf(':');
            string name = rule.Substring(start, end - start);
            start = end + 2;
            end = rule.IndexOf(" or ");
            Range low = ParseRange(rule.Substring(start, end - start));
            start = end + 4;
            Range high = ParseRange(rule.Substring(start));
            return new Rule(name, low, high);
        }

        static int[] ParseTicket(string ticket)
        {
            return ticket.Split(',')
                         .Select(value => int.Parse(value))
                         .ToArray();
        }

        static void AddRange(this Dictionary<int, HashSet<string>> rules, Range range, string name)
        {
            for(int i=range.Min; i <= range.Max; ++i)
            {
                if(!rules.ContainsKey(i))
                {
                    rules[i] = new();
                }

                rules[i].Add(name);
            }
        }

        static (Dictionary<int, HashSet<string>>, Tickets) Load(string path)
        {
            Dictionary<int, HashSet<string>> rules = new();
            var lines = new Queue<string>(File.ReadLines(path));
            while(lines.Peek().Length > 0)
            {
                var rule = ParseRule(lines.Dequeue());
                rules.AddRange(rule.Low, rule.Name);
                rules.AddRange(rule.High, rule.Name);
            }

            Debug.Assert(lines.Dequeue() == "");
            Debug.Assert(lines.Dequeue() == "your ticket:");
            int[] mine = ParseTicket(lines.Dequeue());
            Debug.Assert(lines.Dequeue() == "");
            Debug.Assert(lines.Dequeue() == "nearby tickets:");
            List<int[]> nearby = new();
            while(lines.Count > 0)
            {
                nearby.Add(ParseTicket(lines.Dequeue()));
            }

            return (rules, new Tickets(mine, nearby));
        }

        static void Part1(Dictionary<int, HashSet<string>> rules, Tickets tickets)
        {
            List<int> invalidValues = new();
            foreach(var ticket in tickets.Nearby)
            {
                invalidValues.AddRange(ticket.Where(value => !rules.ContainsKey(value)));
            }

            Console.WriteLine("Part 1: {0}", invalidValues.Sum());
        }

        static void Part2(Dictionary<int, HashSet<string>> rules, Tickets tickets)
        {
            int numFields = tickets.Mine.Length;
            HashSet<string>[] fieldsAt = new HashSet<string>[numFields];
            HashSet<string> allFields = new HashSet<string>();
            foreach(var ruleFields in rules.Values)
            {
                allFields.UnionWith(ruleFields);
            }

            for(int i=0; i<numFields; ++i)
            {
                fieldsAt[i] = new(allFields);
            }

            foreach(var ticket in tickets.Nearby)
            {
                if(ticket.Any(value => !rules.ContainsKey(value)))
                {
                    continue;
                }

                for(int i=0; i<numFields; ++i)
                {
                    fieldsAt[i].IntersectWith(rules[ticket[i]]);
                }
            }

            int numAssigned = 0;
            string[] fields = new string[numFields]; 
            while(numAssigned < numFields)
            {
                string assigned = null;
                int assignIndex = -1;
                for(int i=0; i<numFields; ++i)
                {
                    if(fieldsAt[i].Count == 1)
                    {
                        assigned = fieldsAt[i].First();
                        assignIndex = i;
                        break;
                    }
                }

                fields[assignIndex] = assigned;
                numAssigned += 1;
                foreach(var indexFields in fieldsAt)
                {
                    indexFields.Remove(assigned);
                }
            }

            List<int> departureValues = new();
            for(int i=0; i<fields.Length; ++i)
            {
                if(fields[i].StartsWith("departure"))
                {
                    departureValues.Add(tickets.Mine[i]);
                }
            }

            Console.WriteLine("Part 2: {0}", departureValues.Aggregate(1L, (product, value) => value * product));
        }

        static void Main(string[] args)
        {
            (Dictionary<int, HashSet<string>> rules, Tickets tickets) = Load(args[0]);
            Part1(rules, tickets);
            Part2(rules, tickets);
        }
    }
}
