using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;
using Xunit.Abstractions;

namespace AoC_2022
{
    public static class Day15
    {
        public class Day15_Input : List<Day15_SensorBeaconPair> {}

        public record Day15_SensorBeaconPair(int SensorX, int SensorY, int BeaconX, int BeaconY);
        public static void Day15_Main()
        {
            var input = Day15_ReadInput();
            Console.WriteLine($"Day15 Part1: {Day15_Part1(input, 2000000)}");
            Console.WriteLine($"Day15 Part2: {Day15_Part2(input, 4000000)}");
        }

        public static Day15_Input Day15_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day15\\Day15_input.txt").ReadToEnd();
            }

            var result = new Day15_Input();

            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()))
            {
                var numbers = line.Split(" ")
                    .Where(f => f.Contains("="))
                    .Select(f => int.Parse(f.Split("=")[1].TrimEnd(',').TrimEnd(':')))
                    .ToArray();
                result.Add(new Day15_SensorBeaconPair(numbers[0], numbers[1], numbers[2], numbers[3]));
            }

            return result;
        }


        public static int Day15_Part1(Day15_Input input , int row)
        {
            List<(int, int)> UnionPosInRow = GetCoveredRangesInRow(input, row);

            var KnownPosCount = 0;
            foreach (var UnionRange in UnionPosInRow)
            {
                KnownPosCount += (UnionRange.Item2 - UnionRange.Item1) + 1;
            }

            var BeaconsInThatRow = new List<int>();
            foreach (var SensBeacon in input)
            {
                if (SensBeacon.BeaconY == row)
                {
                    if (!BeaconsInThatRow.Contains(SensBeacon.BeaconX)) BeaconsInThatRow.Add(SensBeacon.BeaconX);
                }
                if (SensBeacon.SensorY == row)
                {
                    throw new Exception();
                }
            }
            KnownPosCount -= BeaconsInThatRow.Count;
            return KnownPosCount;
        }

        private static List<(int, int)> GetCoveredRangesInRow(Day15_Input input, int row)
        {
            var knownPosInRow = new List<(int, int)>();
            foreach (var SensBeacon in input)
            {
                var distance = Math.Abs(SensBeacon.BeaconX - SensBeacon.SensorX) + Math.Abs(SensBeacon.BeaconY - SensBeacon.SensorY);
                var relativeX = distance - Math.Abs(SensBeacon.SensorY - row);
                if (relativeX < 0) continue; // does not reach that row
                var rowEnd1 = SensBeacon.SensorX - relativeX;
                var rowEnd2 = SensBeacon.SensorX + relativeX;
                knownPosInRow.Add((rowEnd1, rowEnd2));
            }

            List<(int, int)> UnionPosInRow;
            bool isChanged;
            (isChanged, UnionPosInRow) = UnionRanges(knownPosInRow);
            return UnionPosInRow;
        }

        public static (bool, List<(int, int)>) UnionRanges(List<(int, int)> knownPosInRow)
        {
            bool isChanged = false;
            var UnionPosInRow = new List<(int, int)>();
           
            UnionPosInRow.Add(knownPosInRow.First());
            for (var j = 1; j < knownPosInRow.Count; j++)
            {
                var i = 0;
                bool toAdd = true;
                while (i < UnionPosInRow.Count)
                {
                    if (knownPosInRow[j].Item1 >= UnionPosInRow[i].Item1 && knownPosInRow[j].Item2 <= UnionPosInRow[i].Item2)
                    {
                        toAdd = false;
                        //fully in -> do nothing
                        isChanged = true;
                    }
                    else if (knownPosInRow[j].Item1 <= UnionPosInRow[i].Item1 && knownPosInRow[j].Item2 >= UnionPosInRow[i].Item2)
                    {
                        toAdd = false;
                        // full overlap -> known replace union
                        UnionPosInRow.RemoveAt(i);
                        UnionPosInRow.Add(knownPosInRow[j]);
                        i--;
                        isChanged = true;
                    }
                    else if (knownPosInRow[j].Item1 >= UnionPosInRow[i].Item1 && knownPosInRow[j].Item1 <= UnionPosInRow[i].Item2)
                    {
                        toAdd = false;
                        UnionPosInRow[i] = (UnionPosInRow[i].Item1, knownPosInRow[j].Item2);
                        isChanged = true;
                    }
                    else if (knownPosInRow[j].Item2 >= UnionPosInRow[i].Item1 && knownPosInRow[j].Item2 <= UnionPosInRow[i].Item2)
                    {
                        toAdd = false;
                        UnionPosInRow[i] = (knownPosInRow[j].Item1, UnionPosInRow[i].Item2);
                        isChanged = true;
                    }
                    i++;
                }

                if(toAdd == true)
                {
                    UnionPosInRow.Add(knownPosInRow[j]);
                }
            }
            if (isChanged == true) (isChanged, UnionPosInRow) = UnionRanges(UnionPosInRow);

            return (isChanged, UnionPosInRow);
        }

        public static Int64 Day15_Part2(Day15_Input input, int coordlimit)
        {
            for (var row = 0; row <= coordlimit; row++)
            {
                var CoveredRanges = GetCoveredRangesInRow(input, row);
                if (CoveredRanges.Count == 1)
                {
                    if (CoveredRanges.First().Item1 > 0) return (Int64)CoveredRanges.First().Item1 * (Int64)4000000 + (Int64)row; 
                    else if (CoveredRanges.First().Item2 < coordlimit) return (Int64)CoveredRanges.First().Item2 * (Int64)4000000 + (Int64)row;
                    

                }
                else
                {
                    CoveredRanges.Sort((f,g) => f.CompareTo(g));
                    if (CoveredRanges.First().Item1 > 0) return (Int64)CoveredRanges.First().Item1 * (Int64)4000000 + (Int64)row;
                    for (var i = 1; i < CoveredRanges.Count; i++)
                    {
                        if (CoveredRanges[i].Item1 - CoveredRanges[i-1].Item2 > 1) return ((Int64)CoveredRanges[i].Item1 - 1) * (Int64)4000000 + (Int64)row;
                    }
                    if (CoveredRanges.Last().Item2 < coordlimit) return (Int64)CoveredRanges.Last().Item2 * (Int64)4000000 + (Int64)row;
                }

            }

            return 0;
        }


    }
    public class Day15_Test
    {
        [Theory]
        [InlineData("Sensor at x=2, y=18: closest beacon is at x=-2, y=15\r\nSensor at x=9, y=16: closest beacon is at x=10, y=16\r\nSensor at x=13, y=2: closest beacon is at x=15, y=3\r\nSensor at x=12, y=14: closest beacon is at x=10, y=16\r\nSensor at x=10, y=20: closest beacon is at x=10, y=16\r\nSensor at x=14, y=17: closest beacon is at x=10, y=16\r\nSensor at x=8, y=7: closest beacon is at x=2, y=10\r\nSensor at x=2, y=0: closest beacon is at x=2, y=10\r\nSensor at x=0, y=11: closest beacon is at x=2, y=10\r\nSensor at x=20, y=14: closest beacon is at x=25, y=17\r\nSensor at x=17, y=20: closest beacon is at x=21, y=22\r\nSensor at x=16, y=7: closest beacon is at x=15, y=3\r\nSensor at x=14, y=3: closest beacon is at x=15, y=3\r\nSensor at x=20, y=1: closest beacon is at x=15, y=3", 10, 26)]
        public static void Day15Part1Test(string rawinput, int row, int expectedValue)
        {
            Assert.Equal(expectedValue, Day15.Day15_Part1(Day15.Day15_ReadInput(rawinput), row));
        }

        [Theory]
        [InlineData("Sensor at x=2, y=18: closest beacon is at x=-2, y=15\r\nSensor at x=9, y=16: closest beacon is at x=10, y=16\r\nSensor at x=13, y=2: closest beacon is at x=15, y=3\r\nSensor at x=12, y=14: closest beacon is at x=10, y=16\r\nSensor at x=10, y=20: closest beacon is at x=10, y=16\r\nSensor at x=14, y=17: closest beacon is at x=10, y=16\r\nSensor at x=8, y=7: closest beacon is at x=2, y=10\r\nSensor at x=2, y=0: closest beacon is at x=2, y=10\r\nSensor at x=0, y=11: closest beacon is at x=2, y=10\r\nSensor at x=20, y=14: closest beacon is at x=25, y=17\r\nSensor at x=17, y=20: closest beacon is at x=21, y=22\r\nSensor at x=16, y=7: closest beacon is at x=15, y=3\r\nSensor at x=14, y=3: closest beacon is at x=15, y=3\r\nSensor at x=20, y=1: closest beacon is at x=15, y=3", 20, 56000011)]
        public static void Day15Part2Test(string rawinput, int coordlimit, int expectedValue)
        {
            Assert.Equal(expectedValue, Day15.Day15_Part2(Day15.Day15_ReadInput(rawinput), coordlimit));
        }


        [Theory]
        [MemberData(nameof(IntersectData))]
        public static void Day15IntersectTest(List<(int, int)> input, List<(int, int)> expectedValue)
        {
            Assert.Equal(expectedValue, Day15.UnionRanges(input).Item2);
        }

        public static IEnumerable<object[]> IntersectData(){
            yield return new object[] { new List<(int, int)> { (1, 2), (4, 5) }, new List<(int, int)> { (1, 2), (4, 5) } };
            yield return new object[] { new List<(int, int)> { (1, 2), (2, 3) }, new List<(int, int)> { (1, 3) } };
            yield return new object[] { new List<(int, int)> { (1, 3), (2, 4) }, new List<(int, int)> { (1, 4) } };
            yield return new object[] { new List<(int, int)> { (2, 4), (1, 3) }, new List<(int, int)> { (1, 4) } };
            yield return new object[] { new List<(int, int)> { (1, 3), (2, 3) }, new List<(int, int)> { (1, 3) } };
            yield return new object[] { new List<(int, int)> { (1, 2), (1, 3) }, new List<(int, int)> { (1, 3) } };
            yield return new object[] { new List<(int, int)> { (1, 4), (2, 3) }, new List<(int, int)> { (1, 4) } };
        }

    }
}
