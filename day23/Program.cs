using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace day23
{
    class Cup
    {
        public Cup(int label)
        {
            Label = label;
        }

        public int Label{get; private set;}
        public Cup Next{get; private set;}
        public Cup Prev{get; private set;}

        public void Connect(Cup next)
        {
            Next = next;
            next.Prev = this;
        }

        public Cup Pickup()
        {
            Cup first = Next;
            Cup last = first.Next.Next;
            Connect(last.Next);
            last.Connect(first);
            return first;
        }

        public void Place(Cup cups)
        {
            Cup first = cups;
            Cup last = cups.Prev;
            last.Connect(Next);
            Connect(first);
        }

        public IEnumerable<int> Labels()
        {
            yield return Label;
            for(Cup cup = Next; cup != this; cup = cup.Next)
            {
                yield return cup.Label;
            }
        }
    }

    class Cups
    {
        public Cups(string order) : this(order, order.Length)
        {            
        }

        public Cups(string order, int numCups)
        {
            // moving to zero-based for convenience
            List<int> initialOrder = order.Select(c => (int)(c - '0' - 1)).ToList();
            _cups = Enumerable.Range(0, numCups).Select(label => new Cup(label)).ToList();
            for(int i=initialOrder.Count; i<_cups.Count; ++i)
            {
                _cups[i].Connect(_cups[(i + 1) % _cups.Count]);
            }

            for(int i=0; i<initialOrder.Count; ++i)
            {
                int current = initialOrder[i];
                int next = initialOrder[(i + 1) % initialOrder.Count];
                int prev = initialOrder[(i + initialOrder.Count - 1) % initialOrder.Count];
                _cups[current].Connect(_cups[next]);
                _cups[prev].Connect(_cups[current]);
            }

            if(numCups > initialOrder.Count)
            {
                _cups.Last().Connect(_cups[initialOrder[0]]);
                _cups[initialOrder.Last()].Connect(_cups[initialOrder.Count % _cups.Count]);
            }

            _current = _cups[initialOrder[0]];
        }

        public void Move()
        {
            Cup pickup = _current.Pickup();
            HashSet<int> pickupLabels = new(pickup.Labels());
            int destLabel = (_current.Label + _cups.Count - 1) % _cups.Count;
            while(pickupLabels.Contains(destLabel))
            {
                destLabel = (destLabel + _cups.Count - 1) % _cups.Count;
            }

            _cups[destLabel].Place(pickup);
            _current = _current.Next;
        }

        public Cup Find(int label)
        {
            return _cups[label];
        }

        private Cup _current;
        private List<Cup> _cups;
    }

    static class Program
    {
        static void Part1(Cups cups)
        {
            for(int i=0; i<100; ++i)
            {
                cups.Move();
            }

            // zero based indexing
            Cup one = cups.Find(0);
            Console.WriteLine("Part 1: {0}", string.Join("", one.Labels().Select(label => label + 1).Skip(1)));
        }
        
        static void Part2(Cups cups)
        {
            for(int i=0; i<10000000; ++i)
            {
                cups.Move();
            }

            Cup one = cups.Find(0);
            long next0 = one.Next.Label + 1;
            long next1 = one.Next.Next.Label + 1;
            Console.WriteLine("Part 2: {0}", next0 * next1);
        }

        static void Main(string[] args)
        {
            string order = File.ReadAllText(args[0]);
            Cups cups = new Cups(order);
            Part1(cups);

            cups = new Cups(order, 1000000);
            Part2(cups);
        }
    }
}