using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day07
    {
        public record Day07_Input(Dictionary<string, Int64> Files, Dictionary<string, Day07_Input> SubFolders, Day07_Input? Parent) {
            public Day07_Input() : this(new Dictionary<string, Int64>(), new Dictionary<string, Day07_Input>(), null) { }
            public Day07_Input(Day07_Input Parent) : base()
            {
                this.Files = new Dictionary<string, Int64>();
                this.SubFolders = new Dictionary<string, Day07_Input>();
                this.Parent = Parent;
            }
            public Int64 Size
            {
                get
                {
                    Int64 size = 0;
                    foreach (var file in this.Files) size += file.Value;
                    foreach (var folder in this.SubFolders) size += folder.Value.Size;
                    return size;
                }
            }

            public Int64 SizeOfFoldersSmallerThan(Int64 size)
            {
                Int64 totalsize = 0;
                foreach (var folder in this.SubFolders)
                {
                    Int64 foldersize = folder.Value.Size;
                    if (foldersize <= size) totalsize += foldersize;
                    totalsize += folder.Value.SizeOfFoldersSmallerThan(size);
                }
                return totalsize;
            }

            public Int64 SizeOfFolderThatIsSmallestButBiggerThan(Int64 size)
            {
                Int64 result = (this.Size >= size) ? this.Size : Int64.MaxValue;
                foreach (var folder in this.SubFolders)
                {
                    result = Math.Min(result, folder.Value.SizeOfFolderThatIsSmallestButBiggerThan(size));
                }
                return result;
            }
        }


        public static void Day07_Main()
        {
            var input = Day07_ReadInput();
            Console.WriteLine($"Day07 Part1: {Day07_Part1(input)}");
            Console.WriteLine($"Day07 Part2: {Day07_Part2(input)}");
        }

        public static Day07_Input Day07_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day07\\Day07_input.txt").ReadToEnd();
            }

            var result = new Day07_Input();
            var currentNode = result;
            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()))
            {
                if (line == "$ cd /") continue;

                string[]? args = line.Split(" ");
                if (args.First() == "$")
                {
                    if(args[1] == "cd")
                    {
                        if (args[2] == "..")
                        {
                            if (currentNode.Parent != null)
                            {
                                currentNode = currentNode.Parent;
                            }
                            else throw new NullReferenceException();
                        }
                        else
                        {
                            currentNode = currentNode.SubFolders[args[2]];
                        }
                    }
                    else //ls
                    {
                        // do nothing
                    }
                }
                else //must be ls results
                {
                    if (args[0] == "dir")
                    {
                        currentNode.SubFolders.Add(args[1], new Day07_Input(currentNode));
                    }
                    else
                    {
                        currentNode.Files.Add(args[1], int.Parse(args[0]));
                    }
                }
            }

            return result;
        }


        public static Int64 Day07_Part1(Day07_Input input)
        {
                       
            return input.SizeOfFoldersSmallerThan(100000);
        }

        public static Int64 Day07_Part2(Day07_Input input)
        {
            var leastToDelete = 30000000 - (70000000 - input.Size);
            return input.SizeOfFolderThatIsSmallestButBiggerThan(leastToDelete);
        }


    }
    public class Day07_Test
    {
        [Theory]
        [InlineData("$ cd /\r\n$ ls\r\ndir a\r\n14848514 b.txt\r\n8504156 c.dat\r\ndir d\r\n$ cd a\r\n$ ls\r\ndir e\r\n29116 f\r\n2557 g\r\n62596 h.lst\r\n$ cd e\r\n$ ls\r\n584 i\r\n$ cd ..\r\n$ cd ..\r\n$ cd d\r\n$ ls\r\n4060174 j\r\n8033020 d.log\r\n5626152 d.ext\r\n7214296 k", 95437)]
        public static void Day07Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day07.Day07_Part1(Day07.Day07_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("$ cd /\r\n$ ls\r\ndir a\r\n14848514 b.txt\r\n8504156 c.dat\r\ndir d\r\n$ cd a\r\n$ ls\r\ndir e\r\n29116 f\r\n2557 g\r\n62596 h.lst\r\n$ cd e\r\n$ ls\r\n584 i\r\n$ cd ..\r\n$ cd ..\r\n$ cd d\r\n$ ls\r\n4060174 j\r\n8033020 d.log\r\n5626152 d.ext\r\n7214296 k", 24933642)]
        public static void Day07Part2Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day07.Day07_Part2(Day07.Day07_ReadInput(rawinput)));
        }
    }
}
