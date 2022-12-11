using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day11
    {
        public class Day11_Input : List<Day11_Monkey> //Define input type
        {
        }

        public class Day11_Monkey
        {
            public int Index;

            public Queue<Int64> Items;

            public Func<Int64, Int64> Operation;

            public MonkeyTest TestValues;

            public Int64 InspectCount ;

            public Day11_Monkey(string MonkeyInputString)
            {
                var lines = MonkeyInputString.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()).ToArray();

                Index = int.Parse(lines[0].Substring(7, lines[0].Length - 8));

                Items = new Queue<Int64>();
                string[] rawnumbers = Regex.Split(lines[1], @"\D+");
                foreach (string value in rawnumbers)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        Items.Enqueue(int.Parse(value));
                    }
                }

                var operationArgs = lines[2].Split(" ");
                switch (operationArgs[4])
                {
                    case "+":
                        Operation = (old) =>
                        {
                            return old + int.Parse(operationArgs[5]);
                        };
                        break;
                    case "*":
                        if (operationArgs[5] == "old")
                        {
                            Operation = (old) =>
                            {
                                return old * old;
                            };
                        }
                        else
                        {
                            Operation = (old) =>
                            {
                                return old * int.Parse(operationArgs[5]); ;
                            };
                        }
                        break;
                    default:
                        throw new Exception(); 
                }

                TestValues = new MonkeyTest(int.Parse(lines[3].Split(" ")[3]), int.Parse(lines[4].Split(" ")[5]), int.Parse(lines[5].Split(" ")[5]));

                InspectCount = 0;
            }
        }

        public record struct MonkeyTest(int Divisable, int ThrowToIfTrue, int ThrowToIfFalse);
        public static void Day11_Main()
        {
            var input = Day11_ReadInput();
            Console.WriteLine($"Day11 Part1: {Day11_Part1(input)}");
            input = Day11_ReadInput();
            Console.WriteLine($"Day11 Part2: {Day11_Part2(input)}");
        }

        public static Day11_Input Day11_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day11\\Day11_input.txt").ReadToEnd();
            }

            var result = new Day11_Input();

            foreach (string block in rawinput.Split("\r\n\r\n"))
            {
                result.Add(new Day11_Monkey(block));
            }

            return result;
        }


        public static Int64 Day11_Part1(Day11_Input input)
        {
            for(var round = 1; round <= 20; round++)
            {
                foreach(var monkey in input)
                {
                    while (monkey.Items.Count > 0)
                    {
                        var InspectItem = monkey.Items.Dequeue();
                        var NewWorryLevel = monkey.Operation(InspectItem);
                        NewWorryLevel = NewWorryLevel / 3;
                        if (NewWorryLevel % monkey.TestValues.Divisable == 0)
                        {
                            input[monkey.TestValues.ThrowToIfTrue].Items.Enqueue(NewWorryLevel);
                        }
                        else
                        {
                            input[monkey.TestValues.ThrowToIfFalse].Items.Enqueue(NewWorryLevel);
                        }
                        monkey.InspectCount++;
                    }
                }
            }

            return input.OrderByDescending(f => f.InspectCount).Take(2).Select(f => f.InspectCount).Aggregate((f, g) => f * g);
        }

        public static Int64 Day11_Part2(Day11_Input input)
        {
            Int64 Divider = 1;
            foreach (var monkey in input)
            {
                Divider *= monkey.TestValues.Divisable;
            }

            for (var round = 1; round <= 10000; round++)
            {
                foreach (var monkey in input)
                {
                    while (monkey.Items.Count > 0)
                    {
                        var InspectItem = monkey.Items.Dequeue();
                        var NewWorryLevel = monkey.Operation(InspectItem);
                        NewWorryLevel = NewWorryLevel % Divider;
                        if (NewWorryLevel % monkey.TestValues.Divisable == 0)
                        {
                            input[monkey.TestValues.ThrowToIfTrue].Items.Enqueue(NewWorryLevel);
                        }
                        else
                        {
                            input[monkey.TestValues.ThrowToIfFalse].Items.Enqueue(NewWorryLevel);
                        }
                        monkey.InspectCount++;
                    }
                }
            }

            return input.OrderByDescending(f => f.InspectCount).Take(2).Select(f => f.InspectCount).Aggregate((f, g) => f * g);
        }


    }
    public class Day11_Test
    {
        [Theory]
        [InlineData("Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1", 10605)]
        public static void Day11Part1Test(string rawinput, Int64 expectedValue)
        {
            Assert.Equal(expectedValue, Day11.Day11_Part1(Day11.Day11_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1", 2713310158)]
        public static void Day11Part2Test(string rawinput, Int64 expectedValue)
        {
            Assert.Equal(expectedValue, Day11.Day11_Part2(Day11.Day11_ReadInput(rawinput)));
        }
    }
}
