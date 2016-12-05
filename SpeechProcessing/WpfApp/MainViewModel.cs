using Core;
using Linh.Mvvm;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp
{
    public class MainViewModel : BindableBase
    {
        private WaveFile convertedFile;
        private WaveFile originalFile;

        public WaveFile ConvertedFile
        {
            get { return convertedFile; }
            set { SetProperty(ref convertedFile, value, nameof(ConvertedFile)); }
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
                        var encoded = Algorithm.Encode(originalFile);
                        ConvertedFile = Algorithm.Decode(encoded);
                        ConvertedFile.SaveWave("decoded.wav");
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
                    MediaPlayer mplayer = new MediaPlayer();
                    mplayer.Open(new Uri(convertedFile.Path, UriKind.RelativeOrAbsolute));
                    mplayer.Play();
                });
            }
        }

        public ICommand PlayOriginalCommand
        {
            get
            {
                return new DelegateCommand<object>((param) =>
                {
                    MediaPlayer mplayer = new MediaPlayer();
                    mplayer.Open(new Uri(originalFile.Path, UriKind.RelativeOrAbsolute));
                    mplayer.Play();
                });
            }
        }
    }
}