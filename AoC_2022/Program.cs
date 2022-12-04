// See https://aka.ms/new-console-template for more information
using AoC_2022;
using System.Diagnostics;

Console.WriteLine("AoC 2022");
var sw = new Stopwatch();
sw.Start();
Day01.Day01_Main();
Day02.Day02_Main();
Day03.Day03_Main();
Day04.Day04_Main();

sw.Stop();
Console.WriteLine($"Code run under {sw.ElapsedMilliseconds}ms");
Console.ReadLine();