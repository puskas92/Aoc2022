using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day04
    {
        public class Day04_Input : List<(Day04_section,Day04_section)> //Define input type
        {
        }

        public record Day04_section(int StartPoint, int EndPoint)
        {
            public bool isFullyContainsEachOther(Day04_section intersectWith)
            {
                return (this.StartPoint <= intersectWith.StartPoint && this.EndPoint >= intersectWith.EndPoint)
                    || (this.StartPoint >= intersectWith.StartPoint && this.EndPoint <= intersectWith.EndPoint);
            }

            public bool isOverlapsEachOther(Day04_section intersectWith)
            {
                return (this.StartPoint >= intersectWith.StartPoint && this.StartPoint <= intersectWith.EndPoint)
                    || (this.EndPoint >= intersectWith.StartPoint && this.EndPoint <= intersectWith.EndPoint)
                    || (intersectWith.StartPoint >= this.StartPoint && intersectWith.StartPoint <= this.EndPoint)
                    || (intersectWith.EndPoint >= this.StartPoint && intersectWith.EndPoint <= this.EndPoint);
            }
        }
        public static void Day04_Main()
        {
            var input = Day04_ReadInput();
            Console.WriteLine($"Day04 Part1: {Day04_Part1(input)}");
            Console.WriteLine($"Day04 Part2: {Day04_Part2(input)}");
        }

        public static Day04_Input Day04_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day04\\Day04_input.txt").ReadToEnd();
            }

            var result = new Day04_Input();

            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()))
            {
                var numbers = Regex.Split(line, @"\D+").Select(f=>int.Parse(f)).ToArray();
                if (numbers.Count() != 4) throw new UnreachableException();
                result.Add((new Day04_section(numbers[0], numbers[1]), new Day04_section(numbers[2], numbers[3])));
            }

            return result;
        }


        public static int Day04_Part1(Day04_Input input)
        {

            return input.Count(f=> f.Item1.isFullyContainsEachOther(f.Item2));
        }

        public static int Day04_Part2(Day04_Input input)
        {
            return input.Count(f => f.Item1.isOverlapsEachOther(f.Item2));
        }


    }
    public class Day04_Test
    {
        [Theory]
        [InlineData("2-4,6-8\r\n2-3,4-5\r\n5-7,7-9\r\n2-8,3-7\r\n6-6,4-6\r\n2-6,4-8", 2)]
        public static void Day04Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day04.Day04_Part1(Day04.Day04_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("2-4,6-8\r\n2-3,4-5\r\n5-7,7-9\r\n2-8,3-7\r\n6-6,4-6\r\n2-6,4-8", 4)]
        public static void Day04Part2Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day04.Day04_Part2(Day04.Day04_ReadInput(rawinput)));
        }
    }
}
