using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day13
    {
        public class Day13_Input : List<(Day13_ListOrValue, Day13_ListOrValue)> //Define input type
        { }
        public class Day13_ListOrValue
        {
            public bool isInteger;
            public List<Day13_ListOrValue> ?List;
            public int ?Value;

            public Day13_ListOrValue(string s)
            {
                if (s.StartsWith('['))
                {
                    isInteger = false;
                    this.Value = null;
                    this.List = new List<Day13_ListOrValue>();

                    var substring = "";
                    var bracketlevel = 0;
                    for (var i = 1; i < s.Length - 1; i++)
                    {
                        switch (s[i])
                        {
                            case '[':
                                bracketlevel++;
                                substring += s[i];
                                break;
                            case ']':
                                bracketlevel--;
                                substring += s[i];
                                break;
                            case ',':
                                if(bracketlevel == 0)
                                {
                                    if(substring != "") this.List.Add(new Day13_ListOrValue(substring));
                                    substring = "";
                                }
                                else substring += s[i];
                                break;
                            default:
                                substring += s[i];
                                break;
                        }
                    }
                    if (substring != "") this.List.Add(new Day13_ListOrValue(substring));
                }
                else
                {
                    isInteger = true;
                    this.Value = int.Parse(s);
                    List = null;
                }
            }

            public static bool operator <(Day13_ListOrValue a, Day13_ListOrValue b)
            {
                return (a!=b) ? b>a : false;
            }
            public static bool operator >(Day13_ListOrValue a, Day13_ListOrValue b)
            {
                if (a.Equals(b)) return false;

                if (a.isInteger && b.isInteger)
                {
                    return a.Value > b.Value;
                }
                else
                {
                    List<Day13_ListOrValue> aList;
                    List<Day13_ListOrValue> bList;

                    aList = (a.isInteger ? new Day13_ListOrValue("[" + a.Value.ToString() + "]").List : a.List);
                    bList = (b.isInteger ? new Day13_ListOrValue("[" + b.Value.ToString() + "]").List : b.List);

                    for (var i = 0; i < aList.Count; i++)
                        {
                            if (bList.Count <= i) return true;
                        if (!aList[i].Equals(bList[i])) return aList[i] > bList[i];
                        }
                    return false;
                }
                return false;
            }


            [DebuggerStepThrough()]
            public override string ToString()
            {
                if (this.isInteger)
                {
                    return (Value is null) ? "" : Value.ToString();
                }
                else
                {
                    return (List is null) ? "[]" : "[" + string.Join(",", List.Select(f=> f.ToString())) +"]";
                }
            }

            [DebuggerStepThrough()]
            public override bool Equals(object? obj)
            {
                if (obj == null) return false;
                if(obj.GetType() != this.GetType()) return false;
                return this.ToString() == obj.ToString();
            }

        }
        public static void Day13_Main()
        {
            var input = Day13_ReadInput();
            Console.WriteLine($"Day13 Part1: {Day13_Part1(input)}");
            Console.WriteLine($"Day13 Part2: {Day13_Part2(input)}");
        }

        public static Day13_Input Day13_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day13\\Day13_input.txt").ReadToEnd();
            }

            var result = new Day13_Input();

            foreach (string line in rawinput.Split("\r\n\r\n"))
            {
                var sides = line.Split("\r\n");

                result.Add((new Day13_ListOrValue(sides[0].Trim()), new Day13_ListOrValue(sides[1].Trim())));
            }

            return result;
        }


        public static int Day13_Part1(Day13_Input input)
        {
            var sum = 0;
           for(var i = 1;i<=input.Count; i++)
           {
                sum += (input[i - 1].Item1 > input[i - 1].Item2) ? 0 : i;
           }

           return sum;
            
        }

        public static int Day13_Part2(Day13_Input input)
        {
            Day13_ListOrValue? additionitem1 = new Day13_ListOrValue("[[2]]");
            Day13_ListOrValue? additionitem2 = new Day13_ListOrValue("[[6]]");
            var totalList = new List<Day13_ListOrValue>();
            foreach(var pair in input)
            {
                totalList.Add(pair.Item1);
                totalList.Add(pair.Item2);
            }
            totalList.Add(additionitem1);
            totalList.Add(additionitem2);

            totalList.Sort(new Day13_ListOrValueComparer());

            return (totalList.IndexOf(additionitem1)+1) * (totalList.IndexOf(additionitem2)+1);
        }

        public class Day13_ListOrValueComparer : Comparer<Day13_ListOrValue>
        {
            public override int Compare(Day13_ListOrValue? x, Day13_ListOrValue? y)
            {
                if(x is null || y is null) return 0;
                if(x.Equals(y)) return 0;
                return (x<y) ? -1 : 1;
            }
        }
    }
    public class Day13_Test
    {
        [Theory]
        [InlineData("[1,1,3,1,1]\r\n[1,1,5,1,1]",1)]
        [InlineData("[[1],[2,3,4]]\r\n[[1],4]", 1)]
        [InlineData("[9]\r\n[[8,7,6]]", 0)]
        [InlineData("[[4,4],4,4]\r\n[[4,4],4,4,4]", 1)]
        [InlineData("[7,7,7,7]\r\n[7,7,7]", 0)]
        [InlineData("[]\r\n[3]", 1)]
        [InlineData("[[[]]]\r\n[[]]", 0)]
        [InlineData("[1,[2,[3,[4,[5,6,7]]]],8,9]\r\n[1,[2,[3,[4,[5,6,0]]]],8,9]", 0)]
        [InlineData("[1,1,3,1,1]\r\n[1,1,5,1,1]\r\n\r\n[[1],[2,3,4]]\r\n[[1],4]\r\n\r\n[9]\r\n[[8,7,6]]\r\n\r\n[[4,4],4,4]\r\n[[4,4],4,4,4]\r\n\r\n[7,7,7,7]\r\n[7,7,7]\r\n\r\n[]\r\n[3]\r\n\r\n[[[]]]\r\n[[]]\r\n\r\n[1,[2,[3,[4,[5,6,7]]]],8,9]\r\n[1,[2,[3,[4,[5,6,0]]]],8,9]", 13)]
        public static void Day13Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day13.Day13_Part1(Day13.Day13_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("[1,1,3,1,1]\r\n[1,1,5,1,1]\r\n\r\n[[1],[2,3,4]]\r\n[[1],4]\r\n\r\n[9]\r\n[[8,7,6]]\r\n\r\n[[4,4],4,4]\r\n[[4,4],4,4,4]\r\n\r\n[7,7,7,7]\r\n[7,7,7]\r\n\r\n[]\r\n[3]\r\n\r\n[[[]]]\r\n[[]]\r\n\r\n[1,[2,[3,[4,[5,6,7]]]],8,9]\r\n[1,[2,[3,[4,[5,6,0]]]],8,9]", 140)]
        public static void Day13Part2Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day13.Day13_Part2(Day13.Day13_ReadInput(rawinput)));
        }
    }
}
