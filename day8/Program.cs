using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace day8
{
    enum Operation
    {
        Accumulate,
        Jump,
        NoOperation
    };
    record Instruction(Operation Op, int Argument);
    record State(int Index, long Accumulator);
    static class Program
    {
        static Instruction parse(string instruction)
        {
            var parts = instruction.Split();
            switch (parts[0])
            {
                case "acc":
                    return new Instruction(Operation.Accumulate, int.Parse(parts[1]));

                case "jmp":
                    return new Instruction(Operation.Jump, int.Parse(parts[1]));

                case "nop":
                    return new Instruction(Operation.NoOperation, int.Parse(parts[1]));
            }

            throw new FormatException("Invalid instruction: " + instruction);
        }

        static int Next(this Instruction instruction, int index, bool swapJumpNoop=false)
        {
            switch (instruction.Op)
            {
                case Operation.Accumulate:
                    return index + 1;

                case Operation.NoOperation:
                    return swapJumpNoop ? index + instruction.Argument : index + 1;

                case Operation.Jump:
                    return swapJumpNoop ? index + 1 : index + instruction.Argument;
            }

            throw new InvalidOperationException("Unsupported operation: " + instruction.Op);
        }

        static State Execute(this Instruction instruction, State state)
        {
            switch (instruction.Op)
            {
                case Operation.Accumulate:
                    return new State(state.Index + 1, state.Accumulator + instruction.Argument);

                case Operation.Jump:
                    return new State(state.Index + instruction.Argument, state.Accumulator);

                case Operation.NoOperation:
                    return new State(state.Index + 1, state.Accumulator);
            }

            throw new InvalidOperationException("Unsupported operation: " + instruction.Op);
        }

        static void Part1(Instruction[] program)
        {
            State state = new State(0, 0);
            bool[] visited = new bool[program.Length];
            while (!visited[state.Index])
            {
                visited[state.Index] = true;
                state = program[state.Index].Execute(state);
            }

            Console.WriteLine("Part 1: {0}", state.Accumulator);
        }

        static IEnumerable<(int Index, T Value)> Enumerate<T>(this IEnumerable<T> values)
        {
            int index = 0;
            foreach (var value in values)
            {
                yield return (index, value);
                index += 1;
            }
        }

        static Dictionary<int, List<int>> Invert(this int[] jump_to)
        {
            var result = new Dictionary<int, List<int>>();
            foreach (var item in jump_to.Enumerate())
            {
                if (!result.ContainsKey(item.Value))
                {
                    result[item.Value] = new List<int>();
                }

                result[item.Value].Add(item.Index);
            }

            return result;
        }

        static IEnumerable<(int, HashSet<int>, int)> Jumps(this (int Index, HashSet<int> Visited, int Proposal) current, List<int> jumps, bool isProposal=false)
        {
            foreach(int next in jumps)
            {
                HashSet<int> visited = new HashSet<int>(current.Visited);
                visited.Add(current.Index);
                yield return (next, visited, isProposal ? next : current.Proposal);
            }
        }

        static void Part2(Instruction[] program)
        {
            int[] jump_to = program.Enumerate().Select(item => item.Value.Next(item.Index)).ToArray();
            int[] noop_to = program.Enumerate().Select(item => item.Value.Next(item.Index, true)).ToArray();
            var jump_from = jump_to.Invert();
            var noop_from = noop_to.Invert();

            Queue<(int Index, HashSet<int> Visited, int Proposal)> frontier = new();
            List<int> proposals = new List<int>();
            frontier.Enqueue((program.Length, new(), -1));
            int swap = -1;
            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if (current.Visited.Contains(current.Index))
                {
                    continue;
                }

                if (current.Index == 0)
                {
                    swap = current.Proposal;
                    break;
                }

                if (jump_from.ContainsKey(current.Index))
                {
                    foreach (var jump in current.Jumps(jump_from[current.Index]))
                    {
                        frontier.Enqueue(jump);
                    }
                }
                else if (noop_from.ContainsKey(current.Index) && current.Proposal == -1)
                {
                    foreach (var next in current.Jumps(noop_from[current.Index], true))
                    {
                        frontier.Enqueue(next);
                    }
                }
            }

            State state = new State(0, 0);
            if (program[swap].Op == Operation.Jump)
            {
                program[swap] = program[swap] with { Op = Operation.NoOperation };
            }
            else
            {
                program[swap] = program[swap] with { Op = Operation.Jump };
            }

            bool[] stateVisited = new bool[program.Length];
            while (state.Index < program.Length && !stateVisited[state.Index])
            {
                stateVisited[state.Index] = true;
                state = program[state.Index].Execute(state);
            }

            Console.WriteLine("Part 2: {0}", state.Accumulator);
        }

        static void Main(string[] args)
        {
            var program = File.ReadLines(args[0]).Select(line => parse(line)).ToArray();
            Part1(program);
            Part2(program);
        }
    }
}
