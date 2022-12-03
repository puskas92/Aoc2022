using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day03
    {
        public class Day03_Input : List<string> //Define input type
        {
        }
        public static void Day03_Main()
        {
            var input = Day03_ReadInput();
            Console.WriteLine($"Day03 Part1: {Day03_Part1(input)}");
            Console.WriteLine($"Day03 Part2: {Day03_Part2(input)}");
        }

        public static Day03_Input Day03_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day03\\Day03_input.txt").ReadToEnd();
            }

            var result = new Day03_Input();

            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()))
            {
                result.Add(line);
            }

            return result;
        }


        public static int Day03_Part1(Day03_Input input)
        {
            var sumScore = 0;

            foreach(var line in input)
            {
               var intersect =  line.Take(line.Length / 2).Intersect(line.TakeLast(line.Length / 2)).First();
                if (char.IsAsciiLetterLower(intersect)) sumScore += intersect - 'a'+1;
                else sumScore += intersect - 'A' + 27;
            }

            return sumScore;
        }

        public static int Day03_Part2(Day03_Input input)
        {
            var sumScore = 0;

            for(var i = 0; i<input.Count; i += 3)
            {
                var intersect1 = input[i].Intersect(input[i + 1]);
                var intersect2 = intersect1.Intersect(input[i + 2]).First();
                if (char.IsAsciiLetterLower(intersect2)) sumScore += intersect2 - 'a' + 1;
                else sumScore += intersect2 - 'A' + 27;
            }

            return sumScore;
        }


    }
    public class Day03_Test
    {
        [Theory]
        [InlineData("vJrwpWtwJgWrhcsFMMfFFhFp", 16)]
        [InlineData("jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL", 38)]
        [InlineData("PmmdzqPrVvPwwTWBwg", 42)]
        [InlineData("wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn", 22)]
        [InlineData("ttgJtRGJQctTZtZT", 20)]
        [InlineData("CrZsJsPPZsGzwwsLwLmpwMDw", 19)]
        [InlineData("vJrwpWtwJgWrhcsFMMfFFhFp\r\njqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL\r\nPmmdzqPrVvPwwTWBwg\r\nwMqvLMZHhHMvwLHjbvcjnnSBnvTQFn\r\nttgJtRGJQctTZtZT\r\nCrZsJsPPZsGzwwsLwLmpwMDw", 157)]
        public static void Day03Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day03.Day03_Part1(Day03.Day03_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("vJrwpWtwJgWrhcsFMMfFFhFp\r\njqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL\r\nPmmdzqPrVvPwwTWBwg", 18)]
        [InlineData("wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn\r\nttgJtRGJQctTZtZT\r\nCrZsJsPPZsGzwwsLwLmpwMDw", 52)]
        [InlineData("vJrwpWtwJgWrhcsFMMfFFhFp\r\njqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL\r\nPmmdzqPrVvPwwTWBwg\r\nwMqvLMZHhHMvwLHjbvcjnnSBnvTQFn\r\nttgJtRGJQctTZtZT\r\nCrZsJsPPZsGzwwsLwLmpwMDw", 70)]
        public static void Day03Part2Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day03.Day03_Part2(Day03.Day03_ReadInput(rawinput)));
        }
    }
}
