using System;
using System.Linq;

namespace Core
{
    public static class Algorithm
    {
        public static byte Encode(int number)
        {
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
            return output;
        }

        public static WaveFile Encode(WaveFile file)
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
                    data[i][j] = Encode(file.Data[i][j] >> 3);
                }
            }
            output.Data = data;
            return output;
        }

        public static WaveFile Decode(WaveFile file)
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
                    data[i][j] = Decode((byte)(file.Data[i][j] & 0xFF)) << 3;
                }
            }
            output.Data = data;
            return output;
        }
    }
}