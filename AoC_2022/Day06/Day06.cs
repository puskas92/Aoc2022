using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day06
    {
        public static void Day06_Main()
        {
            var input = Day06_ReadInput();
            Console.WriteLine($"Day06 Part1: {Day06_Part1(input)}");
            Console.WriteLine($"Day06 Part2: {Day06_Part2(input)}");
        }

        public static string Day06_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day06\\Day06_input.txt").ReadToEnd();
            }

            return rawinput;
        }


        public static int Day06_Part1(string input)
        {
            return Day06_FindDistinctSequence(input, 4);
        }


        public static int Day06_Part2(string input)
        {
            return Day06_FindDistinctSequence(input, 14);
        }

        private static int Day06_FindDistinctSequence(string input, int length)
        {
            for (var i = (length-1); i < input.Length; i++)
            {
                if (input.Substring(i - (length-1), length).Distinct().Count() == length) return i + 1;
            }
            return -1;
        }

    }
    public class Day06_Test
    {
        [Theory]
        [InlineData("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 7)]
        [InlineData("bvwbjplbgvbhsrlpgdmjqwftvncz", 5)]
        [InlineData("nppdvjthqldpwncqszvftbrmjlhg", 6)]
        [InlineData("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 10)]
        [InlineData("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw",11)]
        public static void Day06Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day06.Day06_Part1(Day06.Day06_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 19)]
        [InlineData("bvwbjplbgvbhsrlpgdmjqwftvncz", 23)]
        [InlineData("nppdvjthqldpwncqszvftbrmjlhg", 23)]
        [InlineData("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 29)]
        [InlineData("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 26)]
        public static void Day06Part2Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day06.Day06_Part2(Day06.Day06_ReadInput(rawinput)));
        }
    }
}
