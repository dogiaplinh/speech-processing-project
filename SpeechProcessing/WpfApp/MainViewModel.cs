using Core;
using Linh.Mvvm;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp
{
    public class MainViewModel : BindableBase
    {
        private WaveFile convertedFile;
        private int method = 0;
        private WaveFile originalFile;

        public WaveFile ConvertedFile
        {
            get { return convertedFile; }
            set { SetProperty(ref convertedFile, value, nameof(ConvertedFile)); }
        }

        public int Method
        {
            get { return method; }
            set { SetProperty(ref method, value, nameof(Method)); }
        }

        public ICommand OpenFileCommand
        {
            get
            {
                return new DelegateCommand<object>((param) =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "WAV Files (*.wav)|*.wav";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        OriginalFile = new WaveFile(openFileDialog.FileName);
                        if (method == 0)
                        {
                            var encoded = Algorithm.Encode(originalFile, Algorithm.EncodeLog);
                            ConvertedFile = Algorithm.Decode(encoded, Algorithm.DecodeLog);
                            ConvertedFile.SaveWave(originalFile.Path.Replace(".wav", "-decoded.wav"));
                        }
                        else
                        {
                            var encoded = Algorithm.Encode(originalFile, Algorithm.Encode);
                            ConvertedFile = Algorithm.Decode(encoded, Algorithm.Decode);
                            ConvertedFile.SaveWave(originalFile.Path.Replace(".wav", "-decoded.wav"));
                        }
                    }
                });
            }
        }

        public WaveFile OriginalFile
        {
            get { return originalFile; }
            set { SetProperty(ref originalFile, value, nameof(OriginalFile)); }
        }

        public ICommand PlayConvertedCommand
        {
            get
            {
                return new DelegateCommand<object>((param) =>
                {
                    if (convertedFile != null)
                    {
                        MediaPlayer mplayer = new MediaPlayer();
                        mplayer.Open(new Uri(convertedFile.Path, UriKind.RelativeOrAbsolute));
                        mplayer.Play();
                    }
                    else
                    {
                        MessageBox.Show("Hãy mở file wav trước");
                    }
                });
            }
        }

        public ICommand PlayOriginalCommand
        {
            get
            {
                return new DelegateCommand<object>((param) =>
                {
                    if (originalFile != null)
                    {
                        MediaPlayer mplayer = new MediaPlayer();
                        mplayer.Open(new Uri(originalFile.Path, UriKind.RelativeOrAbsolute));
                        mplayer.Play();
                    }
                    else
                    {
                        MessageBox.Show("Hãy mở file wav trước");
                    }
                });
            }
        }
    }
}