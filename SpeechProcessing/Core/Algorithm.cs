using System;
using System.Diagnostics;
using System.Linq;
using static System.Math;

namespace Core
{
    public static class Algorithm
    {
        private const double A = 87.6;
        private static double Z = 1 + Log(A);

        public static byte Encode(int number)
        {
            number = number >> 3;
            byte sign = (byte)(number < 0 ? 0x00 : 0x80);
            if (sign == 0)
            {
                number = (~number & 0xFFF) | (number & 0x1000);
            }
            number &= 0xFFF;
            ushort mask = 0x800;
            int a = 7;
            while (a > 0)
            {
                if ((number & mask) != 0)
                {
                    break;
                }
                else
                {
                    a--;
                    mask >>= 1;
                }
            }
            mask = (ushort)(0x0F << (a == 0 ? a + 1 : a));
            return (byte)((sign | a << 4 | (number & mask) >> (a == 0 ? a + 1 : a)) ^ 0x55);
        }

        public static byte EncodeLog(int number)
        {
            double x = number / 32768.0;
            double y;
            if (Abs(x) < 1 / A)
            {
                y = A * x / Z;
            }
            else
            {
                y = Sign(x) * (1 + Log(A * Abs(x))) / Z;
            }
            return (byte)((y + 1) * 128);
        }

        public static int DecodeLog(byte number)
        {
            double y = number / 128.0 - 1;
            double x;
            if (Abs(y) < 1 / Z)
            {
                x = y * Z / A;
            }
            else
            {
                x = Sign(y) * Pow(E, Abs(y) * Z - 1) / A;
            }
            return (int)(x * 32768);
        }

        public static int Decode(byte number)
        {
            number ^= 0x55;
            var sign = (number & 0x80) == 0;
            int output = sign ? -0x1000 : 0x0000;
            int lsb = (number & 0x70) >> 4;
            if (lsb > 0)
            {
                output |= (1 << (lsb + 4));
            }
            output |= (number & 0xF) << (lsb == 0 ? lsb + 1 : lsb);
            output |= (1 << (lsb > 0 ? lsb - 1 : 0));
            if (output < 0)
            {
                output = (output & -0x1000) | (~output & 0xFFF);
            }
            return output << 3;
        }

        public static WaveFile Encode(WaveFile file, Func<int, byte> func)
        {
            WaveFile output = new WaveFile
            {
                AudioFormat = file.AudioFormat,
                BitsPerSample = file.BitsPerSample,
                BlockAlign = file.BlockAlign,
                DataSize = file.DataSize,
                FileSize = file.FileSize,
                FmtChunkSize = file.FmtChunkSize,
                Format = file.Format,
                NumChannels = file.NumChannels,
                SampleRate = file.SampleRate,
                ByteRate = file.ByteRate
            };
            int[][] data = new int[file.Data.Length][];
            for (int i = 0; i < file.Data.Length; i++)
            {
                data[i] = new int[file.Data[i].Length];
                for (int j = 0; j < file.Data[i].Length; j++)
                {
                    data[i][j] = func(file.Data[i][j]);
                }
            }
            output.Data = data;
            return output;
        }

        public static WaveFile Decode(WaveFile file, Func<byte, int> func)
        {
            WaveFile output = new WaveFile
            {
                AudioFormat = file.AudioFormat,
                BitsPerSample = file.BitsPerSample,
                BlockAlign = file.BlockAlign,
                DataSize = file.DataSize,
                FileSize = file.FileSize,
                FmtChunkSize = file.FmtChunkSize,
                Format = file.Format,
                NumChannels = file.NumChannels,
                SampleRate = file.SampleRate,
                ByteRate = file.ByteRate
            };
            int[][] data = new int[file.Data.Length][];
            for (int i = 0; i < file.Data.Length; i++)
            {
                data[i] = new int[file.Data[i].Length];
                for (int j = 0; j < file.Data[i].Length; j++)
                {
                    data[i][j] = func((byte)(file.Data[i][j] & 0xFF));
                }
            }
            output.Data = data;
            return output;
        }
    }
}