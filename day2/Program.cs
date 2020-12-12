using System;
using System.IO;
using System.Collections;
using System.Linq;

namespace day2
{
    enum RuleMode {
        Part1,
        Part2
    }

    class Password
    {
        private Password(int value0, int value1, char c, string password)
        {
            _value0 = value0;
            _value1 = value1;
            _c = c;
            _password = password;
        }

        bool _part1Rule()
        {
            int count = _password.Count(x => x == _c);
            return count >= _value0 && count <= _value1;
        }

        bool _part2Rule()
        {
            bool pos0 = _password[_value0 - 1] == _c;
            bool pos1 = _password[_value1 - 1] == _c;
            return pos0 ^ pos1;
        }

        public bool IsValid(RuleMode mode)
        {
            return mode switch {
                RuleMode.Part1 => _part1Rule(),
                RuleMode.Part2 => _part2Rule(),
                _ => throw new NotSupportedException()
            };
        }

        public static Password Parse(string line)
        {
            string[] parts = line.Trim().Split();
            int[] min_max = parts[0].Split('-').Select(x => int.Parse(x)).ToArray();
            char c = parts[1][0];
            return new Password(min_max[0], min_max[1], c, parts[2]);
        }

        private int _value0;
        private int _value1;
        private char _c;
        private string _password;
    }
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines(args[0]).ToList();
            var passwords = lines.Select(line => Password.Parse(line));
            Console.WriteLine("Part 1: {0}", passwords.Count(password => password.IsValid(RuleMode.Part1)));
            Console.WriteLine("Part 2: {0}", passwords.Count(password => password.IsValid(RuleMode.Part2)));            
        }
    }
}
