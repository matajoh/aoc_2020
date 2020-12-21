using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day21
{
    record Recipe(HashSet<string> Ingredients, HashSet<string> Allergens);
    static class Program
    {
        static Recipe Parse(string recipe)
        {
            int end = recipe.IndexOf('(');
            HashSet<string> ingredients = new(recipe.Substring(0, end - 1).Split());
            int start = end + 9;
            end = recipe.IndexOf(')');
            HashSet<string> allergens = new(recipe.Substring(start, end - start)
                                                  .Trim()
                                                  .Split(',')
                                                  .Select(allergen => allergen.Trim()));
            return new Recipe(ingredients, allergens);
        }
        static List<Recipe> Load(string path)
        {
            return File.ReadLines(path)
                       .Select(line => Parse(line))
                       .ToList();
        }

        static void Increment(this Dictionary<string, int> counts, IEnumerable<string> keys)
        {
            foreach(var key in keys){
                if(!counts.ContainsKey(key))
                {
                    counts[key] = 0;
                }

                counts[key] += 1;
            }
        }

        static Dictionary<string, int> DetermineIngredientUsage(List<Recipe> recipes)
        {
            Dictionary<string, int> ingredientUsage = new();
            foreach(var recipe in recipes)
            {
                ingredientUsage.Increment(recipe.Ingredients);
            }

            return ingredientUsage;
        }

        static Dictionary<string, HashSet<string>> DetermineAllergens(List<Recipe> recipes)
        {
            Dictionary<string, HashSet<string>> allergenIngredients = new();
            foreach(var recipe in recipes)
            {
                foreach(string allergen in recipe.Allergens)
                {
                    if(!allergenIngredients.ContainsKey(allergen))
                    {
                        allergenIngredients[allergen] = new(recipe.Ingredients);
                    }
                    else
                    {
                        allergenIngredients[allergen].IntersectWith(recipe.Ingredients);
                    }
                }
            }

            return allergenIngredients;
        }

        static void Part1(Dictionary<string, int> ingredientUsage, Dictionary<string, HashSet<string>> allergenIngredients)
        {
            HashSet<string> unsafeIngredients = new();
            foreach(var ingredients in allergenIngredients.Values)
            {
                unsafeIngredients.UnionWith(ingredients);
            }

            HashSet<string> safeIngredients = new(ingredientUsage.Keys);
            safeIngredients.ExceptWith(unsafeIngredients);
            Console.WriteLine("Part 1: {0}", safeIngredients.Sum(ingredient => ingredientUsage[ingredient]));
        }

        static void Part2(Dictionary<string, HashSet<string>> allergenIngredients)
        {
            Dictionary<string, string> ingredientAllergens = new();
            while(allergenIngredients.Count > 0)
            {
                string assignedIngredient = null;
                string assignedAllergen = null;
                foreach((string allergen, HashSet<string> ingredients) in allergenIngredients)
                {
                    if(ingredients.Count == 1)
                    {
                        assignedAllergen = allergen;
                        assignedIngredient = ingredients.First();
                        break;
                    }
                }

                ingredientAllergens[assignedIngredient] = assignedAllergen;
                allergenIngredients.Remove(assignedAllergen);
                foreach((string allergen, HashSet<string> ingredients) in allergenIngredients)
                {
                    ingredients.Remove(assignedIngredient);
                }
            }

            List<string> dangerousIngredients = ingredientAllergens.OrderBy(item => item.Value)
                                                                            .Select(item => item.Key)
                                                                            .ToList();
            Console.WriteLine("Part 2: {0}", string.Join(",", dangerousIngredients));
        }

        static void Main(string[] args)
        {
            List<Recipe> recipes = Load(args[0]);
            var ingredientUsage = DetermineIngredientUsage(recipes);
            var allergenIngredients = DetermineAllergens(recipes);
            Part1(ingredientUsage, allergenIngredients);
            Part2(allergenIngredients);
        }
    }
}
