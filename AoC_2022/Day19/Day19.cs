using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day19
    {
        public class Day19_Input : List<Day19_Blueprint> //Define input type
        {
        }

        public class Day19_Blueprint
        {
            public int id;
            public int OreRobotCostOfOre;
            public int ClayRobotCostOfOre;
            public (int, int) ObsidianRobotCostOfOreClay;
            public (int, int) GeodeRobotCostOfOreObisidian;

            public Day19_Blueprint(string input)
            {
                //Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 4 ore. Each obsidian robot costs 4 ore and 17 clay. Each geode robot costs 4 ore and 20 obsidian.
                var args = input.Split(' ');
                id = int.Parse(args[1].TrimEnd(':'));
                OreRobotCostOfOre = int.Parse(args[6]);
                ClayRobotCostOfOre = int.Parse(args[12]);
                ObsidianRobotCostOfOreClay = (int.Parse(args[18]), int.Parse(args[21]));
                GeodeRobotCostOfOreObisidian = (int.Parse(args[27]), int.Parse(args[30]));
            }

            public int MaximumGeodes(int maxMinute)
            {
                var maximumGeodes = 0;
                var maxOreProductionNeed = Math.Max(Math.Max( Math.Max(OreRobotCostOfOre, ClayRobotCostOfOre), ObsidianRobotCostOfOreClay.Item1), GeodeRobotCostOfOreObisidian.Item1);

                var QueueOfStatus = new Queue<Day19_WorkStatus>();
                QueueOfStatus.Enqueue(new Day19_WorkStatus());

                while (QueueOfStatus.Count > 0)
                {
                    var MakeRobot = false;
                    var ToSimulate = QueueOfStatus.Dequeue();
                    var HelpingOre = ToSimulate.Ore;
                    var HelpingClay = ToSimulate.Clay;
                    var HelpingObsidian = ToSimulate.Obsidian;

                    ToSimulate.Minute += 1;
                    ToSimulate.Ore += ToSimulate.OreRobot;
                    ToSimulate.Clay += ToSimulate.ClayRobot;
                    ToSimulate.Obsidian += ToSimulate.ObsidianRobot;
                    ToSimulate.Geode += ToSimulate.GeodeRobot;

                    if(ToSimulate.Minute == maxMinute)
                    {
                        maximumGeodes = Math.Max(maximumGeodes, ToSimulate.Geode);
                        continue;
                    }

                    if (ToSimulate.OreRobot < (maxOreProductionNeed) && HelpingOre >= OreRobotCostOfOre) 
                    { 
                        var newstatus = new Day19_WorkStatus(ToSimulate);
                        newstatus.Ore -= OreRobotCostOfOre;
                        newstatus.OreRobot += 1;
                        QueueOfStatus.Enqueue(newstatus);
                        MakeRobot = true;
                    }

                    if (HelpingOre >= GeodeRobotCostOfOreObisidian.Item1 && HelpingObsidian >= GeodeRobotCostOfOreObisidian.Item2)
                    {
                        var newstatus = new Day19_WorkStatus(ToSimulate);
                        newstatus.Ore -= GeodeRobotCostOfOreObisidian.Item1;
                        newstatus.Obsidian -= GeodeRobotCostOfOreObisidian.Item2;
                        newstatus.GeodeRobot += 1;
                        QueueOfStatus.Enqueue(newstatus);
                        //ToSimulate.Ore -= GeodeRobotCostOfOreObisidian.Item1;
                        //ToSimulate.Obsidian -= GeodeRobotCostOfOreObisidian.Item2;
                        //ToSimulate.GeodeRobot += 1;
                        //QueueOfStatus.Enqueue(ToSimulate);
                        MakeRobot = true;
                    }
                    else
                    {
                        if (ToSimulate.ObsidianRobot < (GeodeRobotCostOfOreObisidian.Item2) && HelpingOre >= ObsidianRobotCostOfOreClay.Item1 && HelpingClay >= ObsidianRobotCostOfOreClay.Item2)
                        {
                            var newstatus = new Day19_WorkStatus(ToSimulate);
                            newstatus.Ore -= ObsidianRobotCostOfOreClay.Item1;
                            newstatus.Clay -= ObsidianRobotCostOfOreClay.Item2;
                            newstatus.ObsidianRobot += 1;
                            QueueOfStatus.Enqueue(newstatus);
                            //ToSimulate.Ore -= ObsidianRobotCostOfOreClay.Item1;
                            //ToSimulate.Clay -= ObsidianRobotCostOfOreClay.Item2;
                            //ToSimulate.ObsidianRobot += 1;
                            //QueueOfStatus.Enqueue(ToSimulate);
                            MakeRobot = true;
                        }
                        else
                        {
                            if (ToSimulate.ClayRobot < (ObsidianRobotCostOfOreClay.Item2 + 1) && HelpingOre >= ClayRobotCostOfOre)
                            {
                                var newstatus = new Day19_WorkStatus(ToSimulate);
                                newstatus.Ore -= ClayRobotCostOfOre;
                                newstatus.ClayRobot += 1;
                                QueueOfStatus.Enqueue(newstatus);
                                MakeRobot = true;
                            }

                            if (ToSimulate.OreRobot < maxOreProductionNeed || !MakeRobot)
                            {
                                QueueOfStatus.Enqueue(ToSimulate);
                            }
                        }


                       
                    }

                }
                Console.WriteLine(id + ": " + maximumGeodes);
                return maximumGeodes;
            }

            public int MaximumGeodes2(int maxMinute)
            {
                var maximumGeodes = 0;

                var QueueOfStatus = new PriorityQueue< Day19_WorkStatus, int>();
                QueueOfStatus.Enqueue( new Day19_WorkStatus(), 0);

                while (QueueOfStatus.Count > 0)
                {
                    var ToSimulate = QueueOfStatus.Dequeue();

                    //test when geoderobot can be done
                    if (ToSimulate.ObsidianRobot > 0) // && ToSimulate.OreRobot > 0 - but this always true
                    {
                        var timeToObsidian = (int)Math.Ceiling((double)(GeodeRobotCostOfOreObisidian.Item2 - ToSimulate.Obsidian) / ToSimulate.ObsidianRobot);
                        var timeToOre = (int)Math.Ceiling((double)(GeodeRobotCostOfOreObisidian.Item1 - ToSimulate.Ore) / ToSimulate.OreRobot);
                        var waitTime = Math.Max(timeToObsidian, timeToOre) + 2; // +1 because it also had to produce

                        if (ToSimulate.Minute + waitTime <= maxMinute) // or only < ?
                        {
                            var newstatus = new Day19_WorkStatus(ToSimulate);
                            newstatus.Minute += waitTime;
                            newstatus.Ore += waitTime* newstatus.OreRobot;
                            newstatus.Clay += waitTime* newstatus.ClayRobot;
                            newstatus.Obsidian += waitTime* newstatus.ObsidianRobot;
                            newstatus.Geode += waitTime * newstatus.GeodeRobot;
                            newstatus.Ore -= GeodeRobotCostOfOreObisidian.Item1;
                            newstatus.Obsidian -= GeodeRobotCostOfOreObisidian.Item2;
                            newstatus.GeodeRobot += 1;
                            newstatus.Geode += 1;
                            QueueOfStatus.Enqueue(newstatus, newstatus.Minute);
                        }
                      
                    }

                    //test when obsidianrobot can be done
                    if (ToSimulate.ClayRobot > 0) // && ToSimulate.OreRobot > 0 - but this always true
                    {
                        var timeToClay = (int)Math.Ceiling((double)(ObsidianRobotCostOfOreClay.Item2 - ToSimulate.Clay) / ToSimulate.ClayRobot);
                        var timeToOre = (int)Math.Ceiling((double)(ObsidianRobotCostOfOreClay.Item1 - ToSimulate.Ore) / ToSimulate.OreRobot);
                        var waitTime = Math.Max(timeToClay, timeToOre) + 2; // +1 because it also had to produce

                        if (ToSimulate.Minute + waitTime <= maxMinute) // or only < ?
                        {
                            var newstatus = new Day19_WorkStatus(ToSimulate);
                            newstatus.Minute += waitTime;
                            newstatus.Ore += waitTime * newstatus.OreRobot;
                            newstatus.Clay += waitTime * newstatus.ClayRobot;
                            newstatus.Obsidian += waitTime * newstatus.ObsidianRobot;
                            newstatus.Geode += waitTime * newstatus.GeodeRobot;
                            newstatus.Ore -= ObsidianRobotCostOfOreClay.Item1;
                            newstatus.Clay -= ObsidianRobotCostOfOreClay.Item2;
                            newstatus.ObsidianRobot += 1;
                            newstatus.Obsidian += 1;
                            QueueOfStatus.Enqueue(newstatus, newstatus.Minute);
                        }

                    }

                    //test when clay robot can be done
                    var waitTimeClay = (int)Math.Ceiling((double)((ClayRobotCostOfOre - ToSimulate.Ore) / ToSimulate.OreRobot)) + 2; // +1 because it also had to produc
                    if (ToSimulate.Minute + waitTimeClay <= maxMinute) // or only < ?
                    {
                        var newstatus = new Day19_WorkStatus(ToSimulate);
                        newstatus.Minute += waitTimeClay;
                        newstatus.Ore += waitTimeClay * newstatus.OreRobot;
                        newstatus.Clay += waitTimeClay * newstatus.ClayRobot;
                        newstatus.Obsidian += waitTimeClay * newstatus.ObsidianRobot;
                        newstatus.Geode += waitTimeClay * newstatus.GeodeRobot;
                        newstatus.Ore -= ClayRobotCostOfOre;
                        newstatus.ClayRobot += 1;
                        newstatus.Clay += 1;
                        QueueOfStatus.Enqueue(newstatus, newstatus.Minute);
                    }

                    //test when ore robot can be done
                    var waitTimeOre = (int)Math.Ceiling((double)((OreRobotCostOfOre - ToSimulate.Ore) / ToSimulate.OreRobot)) + 2; // +1 because it also had to produc
                    if (ToSimulate.Minute + waitTimeOre <= maxMinute) // or only < ?
                    {
                        var newstatus = new Day19_WorkStatus(ToSimulate);
                        newstatus.Minute += waitTimeOre;
                        newstatus.Ore += waitTimeOre * newstatus.OreRobot;
                        newstatus.Clay += waitTimeOre * newstatus.ClayRobot;
                        newstatus.Obsidian += waitTimeOre * newstatus.ObsidianRobot;
                        newstatus.Geode += waitTimeOre * newstatus.GeodeRobot;
                        newstatus.Ore -= OreRobotCostOfOre;
                        newstatus.OreRobot += 1;
                        newstatus.Ore += 1;
                        QueueOfStatus.Enqueue(newstatus , newstatus.Minute);
                    }

                    //test if nothing will be done until the end
                    maximumGeodes = Math.Max(maximumGeodes, (maxMinute - ToSimulate.Minute) * ToSimulate.GeodeRobot);
                }

                return maximumGeodes;
            }

            private struct Day19_WorkStatus
            {
                public int Minute;
                public int Ore;
                public int Clay;
                public int Obsidian;
                public int Geode;
                public int OreRobot;
                public int ClayRobot;
                public int ObsidianRobot;
                public int GeodeRobot;

                public Day19_WorkStatus()
                {
                    Minute = 0;
                    Ore = 0;
                    Clay = 0;
                    Obsidian = 0;
                    Geode = 0;
                    OreRobot = 1;
                    ClayRobot = 0;
                    ObsidianRobot = 0;
                    GeodeRobot = 0;
                }

                public Day19_WorkStatus(Day19_WorkStatus status)
                {
                    Minute = status.Minute;
                    Ore = status.Ore;
                    Clay = status.Clay;
                    Obsidian = status.Obsidian;
                    Geode = status.Geode;
                    OreRobot = status.OreRobot;
                    ClayRobot = status.ClayRobot;
                    ObsidianRobot = status.ObsidianRobot;
                    GeodeRobot = status.GeodeRobot;
                }
            }

        }
        public static void Day19_Main()
        {
            var input = Day19_ReadInput();
            Console.WriteLine($"Day19 Part1: {Day19_Part1(input)}");
            Console.WriteLine($"Day19 Part2: {Day19_Part2(input)}");
        }

        public static Day19_Input Day19_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day19\\Day19_input.txt").ReadToEnd();
            }

            var result = new Day19_Input();

            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()))
            {
                result.Add(new Day19_Blueprint(line));
            }

            return result;
        }


        public static int Day19_Part1(Day19_Input input)
        {
            return input.Sum(f=>f.id*f.MaximumGeodes(24));
        }

        public static Int64 Day19_Part2(Day19_Input input)
        {
            return input.Take(3).Select(f => f.MaximumGeodes(32)).Aggregate((f, g) => f * g);
        }


    }
    public class Day19_Test
    {
        [Theory]
        [InlineData("Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.", 9)]
        [InlineData("Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.", 12)]
        public static void Day19Test24(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day19.Day19_ReadInput(rawinput).First().MaximumGeodes(24));
        }

        [Theory]
        [InlineData("Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.\r\nBlueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.", 33)]
        public static void Day19Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day19.Day19_Part1(Day19.Day19_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.", 56)]
        [InlineData("Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.", 62)]
        public static void Day19Test32(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day19.Day19_ReadInput(rawinput).First().MaximumGeodes(32));
        }
    }
}
