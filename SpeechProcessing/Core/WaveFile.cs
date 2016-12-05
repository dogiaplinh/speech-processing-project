using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Core
{
    public class WaveFile
    {
        public WaveFile()
        {
        }

        public WaveFile(string path)
        {
            Path = path;
            LoadWave();
        }

        public short AudioFormat { get; set; }

        public short BitsPerSample { get; set; }

        public short BlockAlign { get; set; }

        public int ByteRate { get; set; }

        [JsonIgnore]
        public int[][] Data { get; set; }

        public int DataSize { get; set; }

        public int FileSize { get; set; }

        public int FmtChunkSize { get; set; }

        public string Format { get; set; }

        public short NumChannels { get; set; }

        public string Path { get; set; }

        public int SampleRate { get; set; }

        private void LoadChunk(FileStream fs)
        {
            ASCIIEncoding Encoder = new ASCIIEncoding();

            byte[] bChunkID = new byte[4];
            /* read the first 4 bytes, which should be the name */
            fs.Read(bChunkID, 0, 4);
            string sChunkID = Encoder.GetString(bChunkID); // decode the name

            byte[] ChunkSize = new byte[4];
            /* the next 4 bytes code the size */
            fs.Read(ChunkSize, 0, 4);

            if (sChunkID.Equals("RIFF"))
            {
                // what to do with the RIFF chunk:
                // save size in FileSize
                FileSize = BitConverter.ToInt32(ChunkSize, 0);
                // determine the format
                byte[] Format = new byte[4];
                fs.Read(Format, 0, 4);
                // should be "WAVE" as string
                this.Format = Encoder.GetString(Format);
            }

            if (sChunkID.Equals("fmt "))
            {
                // in the fmtChunk: Save size in FmtChunkSize
                FmtChunkSize = BitConverter.ToInt32(ChunkSize, 0);
                // readout all the other header information
                byte[] AudioFormat = new byte[2];
                fs.Read(AudioFormat, 0, 2);
                this.AudioFormat = BitConverter.ToInt16(AudioFormat, 0);
                byte[] NumChannels = new byte[2];
                fs.Read(NumChannels, 0, 2);
                this.NumChannels = BitConverter.ToInt16(NumChannels, 0);
                byte[] SampleRate = new byte[4];
                fs.Read(SampleRate, 0, 4);
                this.SampleRate = BitConverter.ToInt32(SampleRate, 0);
                byte[] ByteRate = new byte[4];
                fs.Read(ByteRate, 0, 4);
                this.ByteRate = BitConverter.ToInt32(ByteRate, 0);
                byte[] BlockAlign = new byte[2];
                fs.Read(BlockAlign, 0, 2);
                this.BlockAlign = BitConverter.ToInt16(BlockAlign, 0);
                byte[] BitsPerSample = new byte[2];
                fs.Read(BitsPerSample, 0, 2);
                this.BitsPerSample = BitConverter.ToInt16(BitsPerSample, 0);
            }

            if (sChunkID == "data")
            {
                // dataChunk: Save size in DataSize
                DataSize = BitConverter.ToInt32(ChunkSize, 0);

                // the first index of data specifies the audio channel, the 2. the sample
                Data = new int[NumChannels][];
                // temporary array for reading in bytes of one channel per sample
                byte[] temp = new byte[BlockAlign / NumChannels];
                // for every channel, initialize data array with the number of samples
                for (int i = 0; i < NumChannels; i++)
                {
                    Data[i] = new int[DataSize / (NumChannels * BitsPerSample / 8)];
                }

                // traverse all samples
                for (int i = 0; i < Data[0].Length; i++)
                {
                    // iterate over all samples per channel
                    for (int j = 0; j < NumChannels; j++)
                    {
                        // read the correct number of bytes per sample and channel
                        if (fs.Read(temp, 0, BlockAlign / NumChannels) > 0)
                        {   // depending on how many bytes were used,
                            // interpret amplite as Int16 or Int32
                            if (BlockAlign / NumChannels == 2)
                                Data[j][i] = BitConverter.ToInt16(temp, 0);
                            else
                                Data[j][i] = BitConverter.ToInt32(temp, 0);
                        }
                        /* else
                         * other values than 2 or 4 are not treated here
                        */
                    }
                }
            }
        }

        private void LoadWave()
        {
            FileStream fs = File.OpenRead(Path); // open Wave file
            LoadChunk(fs); // read RIFF chunk
            LoadChunk(fs); // read fmt chunk
            LoadChunk(fs); // read data chunk
        }

        public void SaveWave(string outputFile)
        {
            if (Path == null)
                Path = outputFile;
            var encoder = Encoding.ASCII;
            using (var writer = new BinaryWriter(File.Create(outputFile), encoder))
            {
                writer.Write(encoder.GetBytes("RIFF"));
                writer.Write(FileSize);
                writer.Write(encoder.GetBytes("WAVE"));
                writer.Write(encoder.GetBytes("fmt "));
                writer.Write(FmtChunkSize);
                writer.Write(AudioFormat);
                writer.Write(NumChannels);
                writer.Write(SampleRate);
                writer.Write(ByteRate);
                writer.Write(BlockAlign);
                writer.Write(BitsPerSample);
                writer.Write(encoder.GetBytes("data"));
                writer.Write(DataSize);
                for (int i = 0; i < Data[0].Length; i++)
                {
                    for (int j = 0; j < NumChannels; j++)
                    {
                        if (BlockAlign / NumChannels == 2)
                            writer.Write((short)Data[j][i]);
                        else writer.Write(Data[j][i]);
                    }
                }
            }
        }
    }
}