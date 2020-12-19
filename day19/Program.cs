using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace day19
{
    class TreeNode
    {
        public TreeNode(int index, char match)
        {
            Index = index;
            Match = match;
            IsLeaf = true;
        }

        public TreeNode(int index, List<int>[] rules)
        {
            Index = index;
            Rules = rules;
            IsLeaf = false;
        }

        public bool IsLeaf { get; private set; }
        public int Index { get; private set; }
        public char Match { get; private set; }
        public List<int>[] Rules { get; private set; }

        public static TreeNode Parse(string line)
        {
            int start = 0;
            int end = line.IndexOf(':');
            int index = int.Parse(line.Substring(start, end - start));
            start = end + 2;
            if (line[start] == '"')
            {
                return new TreeNode(index, line[start + 1]);
            }
            else
            {
                var rules = line.Substring(start)
                                .Split('|')
                                .Select(rule => rule.Trim()
                                                    .Split()
                                                    .Select(node => int.Parse(node))
                                                    .ToList())
                                .ToArray();
                return new TreeNode(index, rules);
            }
        }


    }
    static class Program
    {
        static List<int> ApplyRule(this Dictionary<int, TreeNode> tree, List<int> nodes, string message, List<int> tokens)
        {
            foreach (var index in nodes)
            {
                List<int> next = tree.Match(index, message, tokens);
                if (next.Count == 0)
                {
                    return next;
                }

                tokens = next;
            }

            return tokens;
        }

        static List<int> Match(this Dictionary<int, TreeNode> tree, int index, string message, List<int> tokens)
        {
            List<int> next = new();

            TreeNode node = tree[index];

            if (node.IsLeaf)
            {
                next.AddRange(tokens.Where(token => token < message.Length && message[token] == node.Match)
                                    .Select(token => token + 1));
                return next;
            }

            foreach (var rule in node.Rules)
            {
                next.AddRange(tree.ApplyRule(rule, message, tokens));
            }

            return next;
        }

        static bool Match(this Dictionary<int, TreeNode> tree, string message)
        {
            var tokens = tree.Match(0, message, new List<int> { 0 });
            return tokens.Any(token => token == message.Length);
        }

        static (Dictionary<int, TreeNode>, List<string>) Load(string path)
        {
            var lines = new Queue<string>(File.ReadLines(path));
            List<TreeNode> nodes = new();
            while (lines.Peek().Length > 0)
            {
                nodes.Add(TreeNode.Parse(lines.Dequeue()));
            }

            lines.Dequeue();

            List<string> messages = lines.ToList();
            return (nodes.ToDictionary(node => node.Index, node => node), messages);
        }

        static void Main(string[] args)
        {
            (Dictionary<int, TreeNode> tree, List<string> messages) = Load(args[0]);
            Console.WriteLine("Part 1: {0}", messages.Count(message => tree.Match(message)));

            tree[8] = TreeNode.Parse("8: 42 | 42 8");
            tree[11] = TreeNode.Parse("11: 42 31 | 42 11 31");

            Console.WriteLine("Part 2: {0}", messages.Count(message => tree.Match(message)));
        }
    }
}
