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
            this._value0 = value0;
            this._value1 = value1;
            this._c = c;
            this._password = password;
        }

        bool _part1Rule()
        {
            int count = this._password.Count(x => x == this._c);
            return count >= this._value0 && count <= this._value1;
        }

        bool _part2Rule()
        {
            bool pos0 = this._password[this._value0 - 1] == this._c;
            bool pos1 = this._password[this._value1 - 1] == this._c;
            return pos0 ^ pos1;
        }

        public bool IsValid(RuleMode mode)
        {
            switch(mode)
            {
                case RuleMode.Part1:
                    return this._part1Rule();
                
                case RuleMode.Part2:
                    return this._part2Rule();
                
                default:
                    throw new NotSupportedException();
            }
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
        static bool isValid1(int min, int max, char c, string password)
        {
            int count = password.Count(x => x == c);
            return count >= min && count <= max;
        }
        static void part1(string[] lines)
        {
            int numValid = 0;
            foreach(string line in lines)
            {
                string[] parts = line.Trim().Split();
                int[] min_max = parts[0].Split('-').Select(x => int.Parse(x)).ToArray();
                char c = parts[1][0];
                if(isValid1(min_max[0], min_max[1], c, parts[2]))
                {
                    numValid += 1;
                }
            }

            Console.WriteLine("Part 1: {0}", numValid);
        }

        static void Main(string[] args)
        {
            var lines = File.ReadLines(args[0]).ToList();
            var passwords = lines.Select(line => Password.Parse(line));
            Console.WriteLine("Part 1: {0}", passwords.Count(password => password.IsValid(RuleMode.Part1)));
            Console.WriteLine("Part 2: {0}", passwords.Count(password => password.IsValid(RuleMode.Part2)));            
        }
    }
}
