using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day12
{
    record Instruction(char Action, int Value);
    record Position(int East, int North);
    record Ship(Position Position, char Facing);
    static class Program
    {
        static List<Instruction> load(string path)
        {
            return File.ReadLines(path)
                       .Select(line => new Instruction(line[0], int.Parse(line.Substring(1))))
                       .ToList();
        }

        static Position MoveCardinal(this Position position, char direction, int value)
        {
            switch(direction){
                case 'N':
                    return position with {North=position.North + value};
                
                case 'E':
                    return position with {East=position.East + value};
                
                case 'S':
                    return position with {North=position.North - value};
                
                case 'W':
                    return position with {East=position.East - value};

                default:
                    throw new InvalidOperationException("Invalid direction");
            }
        }

        static Dictionary<char, int> FacingToAngle = new Dictionary<char, int>(){
            {'E', 0}, {'N', 90}, {'W', 180}, {'S', 270}
        };

        static Dictionary<int, char> AngleToFacing = new Dictionary<int, char>(){
            {0, 'E'}, {90, 'N'}, {180, 'W'}, {270, 'S'}
        };

        static Ship Turn(this Ship state, int value)
        {
            int angle = FacingToAngle[state.Facing] + value;
            if(angle < 0)
            {
                angle += 360;
            }

            if(angle >= 360)
            {
                angle -= 360;
            }

            char facing = AngleToFacing[angle];
            return state with {Facing=facing};
        }

        static Ship Execute(this Instruction instruction, Ship ship)
        {
            switch(instruction.Action){
                case 'N':
                case 'E':
                case 'S':
                case 'W':
                    return ship with {Position = ship.Position.MoveCardinal(instruction.Action, instruction.Value)};
                
                case 'F':
                    return ship with {Position = ship.Position.MoveCardinal(ship.Facing, instruction.Value)};
                
                case 'L':
                    return ship.Turn(instruction.Value);
                
                case 'R':
                    return ship.Turn(-instruction.Value);
                
                default:
                    throw new InvalidOperationException("Invalid action");
            }
        }

        static Position MoveToWaypoint(this Position ship, Position waypoint, int value)
        {
            return ship with {East = ship.East + value * waypoint.East, North = ship.North + value * waypoint.North};
        }

        static Position Rotate(this Position waypoint, int value)
        {
            switch(value)
            {
                case 90:
                    return new Position(-waypoint.North, waypoint.East);
                
                case 180:
                    return waypoint with {East = -waypoint.East, North = -waypoint.North};
                
                case 270:
                    return new Position(waypoint.North, -waypoint.East);
                
                default:
                    throw new InvalidOperationException("Invalid angle");
            }
        }

        static (Position, Position) Execute(this Instruction instruction, Position ship, Position waypoint)
        {
            switch(instruction.Action){
                case 'N':
                case 'E':
                case 'S':
                case 'W':
                    return (ship, waypoint.MoveCardinal(instruction.Action, instruction.Value));
                
                case 'F':
                    return (ship.MoveToWaypoint(waypoint, instruction.Value), waypoint);
                
                case 'L':
                    return (ship, waypoint.Rotate(instruction.Value));
                
                case 'R':
                    return (ship, waypoint.Rotate(360 - instruction.Value));

                default:
                    throw new InvalidOperationException("Invalid action");
            }
        }

        static int Manhattan(this Position state)
        {
            return Math.Abs(state.North) + Math.Abs(state.East);
        }

        static void Part1(List<Instruction> instructions)
        {
            Ship state = new Ship(new Position(0, 0), 'E');
            foreach(var instruction in instructions)
            {
                state = instruction.Execute(state);
            }

            Console.WriteLine("Part 1: {0}", state.Position.Manhattan());
        }

        static void Part2(List<Instruction> instructions)
        {
            Position ship = new Position(0, 0);
            Position waypoint = new Position(10, 1);
            foreach(var instruction in instructions)
            {
                (ship, waypoint) = instruction.Execute(ship, waypoint);
            }

            Console.WriteLine("Part 2: {0}", ship.Manhattan());
        }

        static void Main(string[] args)
        {
            List<Instruction> instructions = load(args[0]);
            Part1(instructions);
            Part2(instructions);
        }
    }
}
