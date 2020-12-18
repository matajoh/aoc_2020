using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace day18
{
    static class Program
    {
        static void SkipWhitespace(this Queue<char> tokens)
        {
            while(tokens.Count > 0 && char.IsWhiteSpace(tokens.Peek()))
            {
                tokens.Dequeue();
            }
        }

        static char NextToken(this Queue<char> tokens)
        {
            tokens.SkipWhitespace();
            char token = tokens.Dequeue();
            tokens.SkipWhitespace();
            return token;
        }

        static long NextInt(this Queue<char> tokens)
        {
            char number = tokens.NextToken();
            return (long)(number - '0');
        }

        static void Consume(this Queue<char> tokens, char expected)
        {
            char token = tokens.NextToken();
            Debug.Assert(token == expected);
        }

        static long NextOperand(this Queue<char> tokens, bool infix)
        {
            long operand;
            if(tokens.Peek() == '(')
            {
                tokens.Consume('(');
                if(infix)
                {
                    operand = EvaluateInfix(tokens);
                }
                else
                {
                    operand = EvaluateInOrder(tokens);
                }
                tokens.Consume(')');
            }
            else
            {
                operand = tokens.NextInt();
            }

            if(infix)
            {
                if(tokens.Count > 0 && tokens.Peek() == '+')
                {
                    tokens.Consume('+');
                    operand += tokens.NextOperand(true);
                }
            }

            return operand;
        }

        static long EvaluateInOrder(Queue<char> tokens)
        {
            long result = tokens.NextOperand(false);
            while(tokens.Count > 0 && tokens.Peek() != ')')
            {
                char op = tokens.NextToken();
                long operand = tokens.NextOperand(false);
                result = op == '+' ? result + operand : result * operand;
            }

            return result;
        }

        static long EvaluateInfix(Queue<char> tokens)
        {
            long result = tokens.NextOperand(true);
            while(tokens.Count > 0 && tokens.Peek() != ')')
            {
                tokens.Consume('*');
                long operand = tokens.NextOperand(true);
                result *= operand;
            }

            return result;
        }

        static long Evaluate(string path, bool infix)
        {
            long sum = 0;
            foreach(string line in File.ReadLines(path))
            {
                Queue<char> tokens = new(line.ToCharArray());
                sum += infix ? EvaluateInfix(tokens) : EvaluateInOrder(tokens);
            }

            return sum;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Part 1: {0}", Evaluate(args[0], false));
            Console.WriteLine("Part 2: {0}", Evaluate(args[0], true));
        }
    }
}
