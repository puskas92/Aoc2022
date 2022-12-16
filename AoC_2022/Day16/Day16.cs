using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day16
    {
        public class Day16_Input : Dictionary<string, Day16_Pipe> { }

        public class Day16_Pipe
        {
            public string Name { get; set; }
            public int FlowRate { get; set; }
            public List<Day16_Pipe> LeadTo { get; set; }

            public Day16_Pipe(string Name, int FlowRate)
            {
                this.Name = Name;
                this.FlowRate = FlowRate;
                LeadTo = new List<Day16_Pipe>();
            }
        }
        public static void Day16_Main()
        {
            var input = Day16_ReadInput();
            Console.WriteLine($"Day16 Part1: {Day16_Part1(input)}");
            Console.WriteLine($"Day16 Part2: {Day16_Part2(input)}");
        }

        public static Day16_Input Day16_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day16\\Day16_input.txt").ReadToEnd();
            }

            var result = new Day16_Input();

            var lines = rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim());
            foreach (string line in lines)
            {
                var words = line.Split(" ");
                result.Add(words[1], new Day16_Pipe(words[1], int.Parse(words[4].Split('=')[1].TrimEnd(';'))));
            }
            foreach (string line in lines)
            {
                var words = line.Split(" ");
                var leadTo = words.Skip(9).Select(f => f.TrimEnd(','));
                if (words is null || leadTo is null) break;
                foreach (var lead in leadTo)
                {
                    result[words[1]].LeadTo.Add(result[lead]);
                }
            }

            return result;
        }


        public static int Day16_Part1(Day16_Input input)
        {
            var LowestDistanceMap = GetLowestDistanceMap(input);
            var maxPressure = IterateThroughAllPermuationGetPressure("AA", 0, 0, new List<string>(), input, LowestDistanceMap);
            return maxPressure;
        }

        private static int IterateThroughAllPermuationGetPressure(string Pos, int time, int press, List<string> Visited, Day16_Input input, Dictionary<string, Dictionary<string, int>> LowestDistanceMap )
        {
            Visited.Add(Pos);
            var maxPressure = press;
            foreach(var toCheckPos in input.Keys)
            {
                if (Visited.Contains(toCheckPos)) continue;
                if (input[toCheckPos].FlowRate == 0) continue;
                var newtime = time + LowestDistanceMap[Pos][toCheckPos] +1;  // +1 because we open it
                if (newtime > 30) continue;
                var newpress = press + (30 - newtime) * input[toCheckPos].FlowRate;
                var NewVisited = new List<string>(Visited);
                var pressureToCheck = IterateThroughAllPermuationGetPressure(toCheckPos, newtime, newpress, NewVisited, input, LowestDistanceMap);
                maxPressure = Math.Max(maxPressure, pressureToCheck);
            }
            return maxPressure;
        }

        private static Dictionary<string,int> IterateThroughAllPermuationGetList(string Pos, int time, int press, List<string> Visited, Day16_Input input, Dictionary<string, Dictionary<string, int>> LowestDistanceMap)
        {
            Visited.Add(Pos);
            var visitedString = string.Join(',' , Visited);
            var resultList = new Dictionary<string,int>();
            resultList.Add(visitedString, press);

            foreach (var toCheckPos in input.Keys)
            {
                if (Visited.Contains(toCheckPos)) continue;
                if (input[toCheckPos].FlowRate == 0) continue;
                var newtime = time + LowestDistanceMap[Pos][toCheckPos] + 1;  // +1 because we open it
                if (newtime > 30) continue;
                var newpress = press + (30 - newtime) * input[toCheckPos].FlowRate;
                var NewVisited = new List<string>(Visited);
                NewVisited.Sort();
                visitedString = string.Join(',', NewVisited);
                var subResultList = IterateThroughAllPermuationGetList(toCheckPos, newtime, newpress, NewVisited, input, LowestDistanceMap);
                foreach(var subResult in subResultList)
                {
                    if (!resultList.ContainsKey(subResult.Key)) resultList.Add(subResult.Key, subResult.Value);
                    else resultList[subResult.Key] = Math.Max(resultList[subResult.Key], subResult.Value);
                }
            }
            return resultList;
        }

        private static Dictionary<string, Dictionary<string, int>> GetLowestDistanceMap(Day16_Input input)
        {
            //Could be halved, due to symmetrical distance.
            var LowestDistanceMap = new Dictionary<string, Dictionary<string, int>>();
            foreach (var pos in input)
            {
                LowestDistanceMap.Add(pos.Key, new Dictionary<string, int>());

                var ToCheckQueue = new PriorityQueue<Day16_Status, int>();
                var Visited = new List<string>();
                ToCheckQueue.Enqueue(new Day16_Status(pos.Key), 0);
                while (ToCheckQueue.Count > 0)
                {
                    var ToCheck = ToCheckQueue.Dequeue();
                    if (Visited.Contains(ToCheck.Position)) continue;
                    Visited.Add(ToCheck.Position);
                    LowestDistanceMap[pos.Key].Add(ToCheck.Position, ToCheck.Distance);
                    foreach (var toVisit in input[ToCheck.Position].LeadTo)
                    {
                        if (Visited.Contains(toVisit.Name)) continue;
                        ToCheckQueue.Enqueue(new Day16_Status(toVisit.Name, ToCheck.Distance + 1), ToCheck.Distance + 1);
                    }
                }

            }

            return LowestDistanceMap;
        }

        public struct Day16_Status
        {
            public string Position = "";

            public int Distance = 0;

   
            public Day16_Status(string Position)
            {
                this.Position = Position;
            }

            public Day16_Status(string Position, int Distance)
            {
                this.Position = Position;
                this.Distance = Distance;
            }

        }
        public static int Day16_Part2(Day16_Input input)
        {
            var LowestDistanceMap = GetLowestDistanceMap(input);
            var ListOfElephant = IterateThroughAllPermuationGetList("AA", 4, 0, new List<string>(), input, LowestDistanceMap);
            var maxPressure = int.MinValue;

            //my first solution took around 15min to run
            //foreach(var state in ListOfElephant)
            //{
            //    var Visited = state.Key.Split(',').ToList();
            //    maxPressure =  Math.Max(maxPressure, IterateThroughAllPermuationGetPressure("AA", 4, state.Value, Visited, input, LowestDistanceMap));
            //}

            var SortedListOfPressure = ListOfElephant.OrderByDescending(f => f.Value).ToArray();
            foreach (var pressure in SortedListOfPressure)
            {
                foreach(var pressure2 in SortedListOfPressure)
                {
                    var isCorrect = true;
                    foreach(var pipe in pressure2.Key.Split(',')){
                        if (pipe == "AA") continue;
                        if (pressure.Key.Contains(pipe))
                        {
                            isCorrect = false;
                            break;
                        }
                    }
                    if (isCorrect)
                    {
                        maxPressure = Math.Max(maxPressure, pressure.Value + pressure2.Value);
                        break;
                    }
                }
            }


            return maxPressure;
        }


    }
    public class Day16_Test
    {
        [Theory]
        [InlineData("Valve AA has flow rate=0; tunnels lead to valves DD, II, BB\r\nValve BB has flow rate=13; tunnels lead to valves CC, AA\r\nValve CC has flow rate=2; tunnels lead to valves DD, BB\r\nValve DD has flow rate=20; tunnels lead to valves CC, AA, EE\r\nValve EE has flow rate=3; tunnels lead to valves FF, DD\r\nValve FF has flow rate=0; tunnels lead to valves EE, GG\r\nValve GG has flow rate=0; tunnels lead to valves FF, HH\r\nValve HH has flow rate=22; tunnel leads to valve GG\r\nValve II has flow rate=0; tunnels lead to valves AA, JJ\r\nValve JJ has flow rate=21; tunnel leads to valve II", 1651)]
        public static void Day16Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day16.Day16_Part1(Day16.Day16_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("Valve AA has flow rate=0; tunnels lead to valves DD, II, BB\r\nValve BB has flow rate=13; tunnels lead to valves CC, AA\r\nValve CC has flow rate=2; tunnels lead to valves DD, BB\r\nValve DD has flow rate=20; tunnels lead to valves CC, AA, EE\r\nValve EE has flow rate=3; tunnels lead to valves FF, DD\r\nValve FF has flow rate=0; tunnels lead to valves EE, GG\r\nValve GG has flow rate=0; tunnels lead to valves FF, HH\r\nValve HH has flow rate=22; tunnel leads to valve GG\r\nValve II has flow rate=0; tunnels lead to valves AA, JJ\r\nValve JJ has flow rate=21; tunnel leads to valve II", 1707)]
        public static void Day16Part2Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day16.Day16_Part2(Day16.Day16_ReadInput(rawinput)));
        }
    }
}
