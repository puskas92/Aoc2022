using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day20
    {
        public class Day20_Input
        {
            public LinkedList<Int64> LinkedListStorage;
            public List<(Int64, LinkedListNode<Int64>)> NormalList;

            public Day20_Input()
            {
                LinkedListStorage = new LinkedList<Int64>();
                NormalList = new List<(Int64, LinkedListNode<Int64>)>();
            }

            public void Add(Int64 number)
            {
                var node = LinkedListStorage.AddLast(number);
                NormalList.Add((number, node));
            }

            public void Mixing()
            {
                for(var j = 0;j<NormalList.Count; j++)
                {
                    var num = NormalList[j].Item1;
                    var node = NormalList[j].Item2;
                    // if (Math.Abs(num) == NormalList.Count()) continue;
                    if (num > 0)
                    {
                        var nextnode = node;
                       
                        for (var i = 0; i < (num % (LinkedListStorage.Count - 1)) ; i++)
                        {
                            if (nextnode is null) throw new Exception();
                            nextnode = nextnode.Next ?? LinkedListStorage.First; 
                        }

                        if (nextnode is null) throw new Exception();
                        //  if (nextnode.Value == node.Value) continue;
                        var newNode = LinkedListStorage.AddAfter(nextnode, num);
                        LinkedListStorage.Remove(node);

                        NormalList[j] = (num, newNode);
                    }
                    else if(num<0)
                    {
                        var nextnode = node;
                       
                        for (var i = 0; i < ((-num) % (LinkedListStorage.Count-1)); i++)
                        {
                            if (nextnode is null) throw new Exception();
                            nextnode = nextnode.Previous ?? LinkedListStorage.Last;
                        }

                        if (nextnode is null) throw new Exception();
                        //if (nextnode.Value == node.Value) continue;
                        var newNode = LinkedListStorage.AddBefore(nextnode, num);
                        LinkedListStorage.Remove(node);

                        NormalList[j] = (num, newNode);
                    }
                    //0 does not move
                }
            }
        }
        public static void Day20_Main()
        {
            var input = Day20_ReadInput();
            Console.WriteLine($"Day20 Part1: {Day20_Part1(input)}");
            var input2 = Day20_ReadInput("", 811589153);
            Console.WriteLine($"Day20 Part2: {Day20_Part2(input2)}");
        }

        public static Day20_Input Day20_ReadInput(string rawinput = "", Int64 encryptKey = 1)
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day20\\Day20_input.txt").ReadToEnd();
            }

            var result = new Day20_Input();

            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()))
            {
                result.Add(int.Parse(line) * encryptKey);
            }

            return result;
        }


        public static int Day20_Part1(Day20_Input input)
        {
            input.Mixing();
            Int64 sum = 0;
            var nextnode = input.NormalList.Find(f => f.Item1 == 0).Item2;
            var nextstep = (input.NormalList.Count()< 1000) ? 1000 % input.NormalList.Count() : 1000;
            for (var i = 1; i <= 3; i++)
            {
                for (var j = 1; j <= nextstep; j++)
                {
                    if (nextnode is null) throw new Exception();
                    nextnode = nextnode.Next ?? input.LinkedListStorage.First;
                }
                if (nextnode is null) throw new Exception();
                sum += nextnode.Value;
            }

            return (int)sum;
        }

        public static Int64 Day20_Part2(Day20_Input input)
        {
            for (var i = 1; i<=10; i++)
            {
                input.Mixing();
            }
  
            Int64 sum = 0;
            var nextnode = input.NormalList.Find(f => f.Item1 == 0).Item2;
            var nextstep = (input.NormalList.Count()< 1000) ? 1000 : 1000 % input.NormalList.Count() ;
            for (var i = 1; i <= 3; i++)
            {
                for (var j = 1; j <= nextstep; j++)
                {
                    if (nextnode is null) throw new Exception();
                    nextnode = nextnode.Next ?? input.LinkedListStorage.First;
                }
                if (nextnode is null) throw new Exception();
                sum += nextnode.Value;
            }

            return sum;
        }


    }
    public class Day20_Test
    {
        [Theory]
        [InlineData("5\r\n1\r\n-1\r\n0\r\n2", 0)]
        [InlineData("1\r\n2\r\n-3\r\n3\r\n-2\r\n0\r\n4", 3)]
        public static void Day20Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day20.Day20_Part1(Day20.Day20_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("1\r\n2\r\n-3\r\n3\r\n-2\r\n0\r\n4", 1623178306)]
        public static void Day20Part2Test(string rawinput, Int64 expectedValue)
        {
            Assert.Equal(expectedValue, Day20.Day20_Part2(Day20.Day20_ReadInput(rawinput, 811589153)));
        }
    }
}
