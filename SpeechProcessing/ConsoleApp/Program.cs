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
            //string filePath = Environment.CurrentDirectory + "\\" + fileName;
            //WaveFile waveFile = new WaveFile(filePath);
            //Console.WriteLine(JsonConvert.SerializeObject(waveFile));
            const int min = -30000;
            const int max = 30000;
            const int step = 1000;
            for (int i = min; i < max; i += step)
            {
                int value = i >> 3;
                int encoded = Algorithm.Encode(value);
                byte encoded2 = Algorithm.EncodeLog(i);
                int decoded = Algorithm.Decode((byte)encoded);
                int decoded2 = Algorithm.DecodeLog(encoded2);
                //Console.WriteLine($"{Convert.ToString(i, 2)} {Convert.ToString(encoded ^ 0x55, 2)} {Convert.ToString(decoded, 2)}");
                Console.WriteLine($"{value} {encoded ^ 0x55} {encoded2} {decoded} {decoded2}");
            }
        }
    }
}