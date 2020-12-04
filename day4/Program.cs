using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/**
byr (Birth Year) - four digits; at least 1920 and at most 2002.
iyr (Issue Year) - four digits; at least 2010 and at most 2020.
eyr (Expiration Year) - four digits; at least 2020 and at most 2030.
hgt (Height) - a number followed by either cm or in:
If cm, the number must be at least 150 and at most 193.
If in, the number must be at least 59 and at most 76.
hcl (Hair Color) - a # followed by exactly six characters 0-9 or a-f.
ecl (Eye Color) - exactly one of: amb blu brn gry grn hzl oth.
pid (Passport ID) - a nine-digit number, including leading zeroes.
cid (Country ID) - ignored, missing or not.
*/

namespace day4
{
    class Program
    {
        static string[] RequiredKeys = {"byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"};
        static HashSet<string> EyeColors = new HashSet<string>(new string[]{"amb", "blu", "brn", "gry", "grn", "hzl", "oth"});
        static HashSet<char> HexChars = new HashSet<char>(new char[]{'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'});


        static bool yearRule(string value, int min, int max)
        {
            int year;
            if(int.TryParse(value, out year))
            {
                return year >= min && year <= max;
            }

            return false;
        }

        static bool heightRule(string value)
        {
            int length;
            if(int.TryParse(value.Substring(0, value.Length - 2), out length))
            {
                string units = value.Substring(value.Length - 2);
                if(units == "cm"){
                    return length >= 150 && length <= 196;
                }
                
                if(units == "in"){
                    return length >= 59 && length <= 76;
                }
            }

            return false;
        }

        static bool hairColorRule(string value)
        {
            if(value[0] != '#' || value.Length < 7){
                return false;
            }

            return value.Skip(1).All(c => HexChars.Contains(c));
        }

        static bool eyeColorRule(string value)
        {
            return EyeColors.Contains(value);
        }

        static bool idRule(string value)
        {
            if(value.Length != 9)
            {
                return false;
            }

            return value.All(c => char.IsDigit(c));
        }

        static bool checkValidStrict(Dictionary<string, string> passport)
        {
            bool isValid = checkValid(passport);
            if(!isValid)
            {
                return false;
            }

            isValid = isValid && yearRule(passport["byr"], 1920, 2002);
            isValid = isValid && yearRule(passport["iyr"], 2010, 2020);
            isValid = isValid && yearRule(passport["eyr"], 2020, 2030);
            isValid = isValid && heightRule(passport["hgt"]);
            isValid = isValid && hairColorRule(passport["hcl"]);
            isValid = isValid && eyeColorRule(passport["ecl"]);
            isValid = isValid && idRule(passport["pid"]);
            return isValid;
        }

        static bool checkValid(Dictionary<string, string> passport)
        {
            bool isValid = true;
            foreach(var key in RequiredKeys){
                if(!passport.ContainsKey(key)){
                    isValid = false;
                    break;
                }
            }

            return isValid;
        }

        static int ValidatePassports(string path, Func<Dictionary<string, string>, bool> validator)
        {
            int numValid = 0;
            Dictionary<string, string> current = new Dictionary<string, string>();
            foreach(var line in File.ReadLines(path))
            {
                if(line.Length == 0){
                    if(validator(current)){
                        numValid += 1;
                    }

                    current.Clear();
                    continue;
                }

                foreach(var part in line.Split())
                {
                    var bits = part.Split(':');
                    current[bits[0]] = bits[1];
                }
            }

            if(validator(current)){
                numValid += 1;
            }

            return numValid;
        }
        static void Main(string[] args)
        {
            int numValid = ValidatePassports(args[0], passport => checkValid(passport));            
            Console.WriteLine("Part 1: {0}", numValid);

            numValid = ValidatePassports(args[0], passport => checkValidStrict(passport));
            Console.WriteLine("Part 2: {0}", numValid);
        }
    }
}
