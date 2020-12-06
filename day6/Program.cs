using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace day6
{
    class Program
    {
        static int process(string path, bool intersect)
        {
            List<HashSet<char>> answers = new List<HashSet<char>>();
            HashSet<char> current = null;
            foreach(var line in File.ReadLines(path))
            {
                if(line.Length == 0)
                {
                    answers.Add(current);
                    current = null;
                }
                else
                {
                    HashSet<char> person = new HashSet<char>(line);
                    if(current == null){
                        current = person;
                    }else if(intersect){
                        current.IntersectWith(person);
                    }else{
                        current.UnionWith(person);
                    }
                }
            }

            answers.Add(current);
            return answers.Sum(answer=>answer.Count);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Part 1: {0}", process(args[0], false));
            Console.WriteLine("Part 2: {0}", process(args[0], true));
        }
    }
}
