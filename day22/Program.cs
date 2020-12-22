using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace day22
{
    static class Program
    {
        static (Queue<int>, Queue<int>) Load(string path)
        {
            Queue<string> lines = new Queue<string>(File.ReadLines(path));
            lines.Dequeue();
            Queue<int> deck0 = new();
            while(lines.Peek().Length > 0)
            {
                deck0.Enqueue(int.Parse(lines.Dequeue()));
            }

            lines.Dequeue();
            lines.Dequeue();
            Queue<int> deck1 = new();
            while(lines.Count > 0)
            {
                deck1.Enqueue(int.Parse(lines.Dequeue()));
            }

            return (deck0, deck1);
        }

        static string ToHash(Queue<int> deck0, Queue<int> deck1)
        {
            return string.Join('|', string.Join(',', deck0), string.Join(',', deck1));
        }

        static int Score(this Queue<int> deck)
        {
            int score = 0;
            for(int multiplier=deck.Count; multiplier > 0; --multiplier)
            {
                int card = deck.Dequeue();
                score += multiplier * card;
                deck.Enqueue(card);
            }

            return score;
        }

        static int PlayGame(Queue<int> deck0, Queue<int> deck1, bool recurse)
        {
            HashSet<string> rounds = new();
            string hash = ToHash(deck0, deck1);
            while(!rounds.Contains(hash))
            {
                rounds.Add(hash);
                if(deck0.Count == 0)
                {
                    return 1;
                }

                if(deck1.Count == 0)
                {
                    return 0;
                }

                int card0 = deck0.Dequeue();
                int card1 = deck1.Dequeue();
                int winner;
                if(deck0.Count >= card0 && deck1.Count >= card1 && recurse)
                {
                    winner = PlayGame(new Queue<int>(deck0.Take(card0)),
                                      new Queue<int>(deck1.Take(card1)),
                                      true);
                }
                else
                {
                    winner = card0 > card1 ? 0 : 1;
                }

                if(winner == 0)
                {
                    deck0.Enqueue(card0);
                    deck0.Enqueue(card1);
                }
                else
                {
                    deck1.Enqueue(card1);
                    deck1.Enqueue(card0);
                }

                hash = ToHash(deck0, deck1);
            }

            return 0;
        }

        static void Part1(Queue<int> deck0, Queue<int> deck1)
        {
            int winner = PlayGame(deck0, deck1, false);
            Console.WriteLine("Part 1: {0}", winner == 0 ? deck0.Score() : deck1.Score());
        }

        static void Part2(Queue<int> deck0, Queue<int> deck1)
        {
            int winner = PlayGame(deck0, deck1, true);
            Console.WriteLine("Part 2: {0}", winner == 0 ? deck0.Score() : deck1.Score());
        }

        static void Main(string[] args)
        {
            (Queue<int> deck0, Queue<int> deck1) = Load(args[0]);
            Part1(deck0, deck1);
            (deck0, deck1) = Load(args[0]);
            Part2(deck0, deck1);            
        }
    }
}
