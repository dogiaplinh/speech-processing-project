using Core;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ConsoleApp
{
    internal class Program
    {
        private static string fileName = "A96.wav";

        private static void Main(string[] args)
        {
            string filePath = Environment.CurrentDirectory + "\\" + fileName;
            WaveFile waveFile = new WaveFile(filePath);
            Console.WriteLine(JsonConvert.SerializeObject(waveFile));
        }
    }
}