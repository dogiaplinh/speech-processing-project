using Core;
using Linh.Mvvm;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows.Input;

namespace WpfApp
{
    public class MainViewModel : BindableBase
    {
        private WaveFile file;

        public WaveFile File
        {
            get { return file; }
            set { SetProperty(ref file, value, nameof(File)); }
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
                        File = new WaveFile(openFileDialog.FileName);
                    }
                });
            }
        }
    }
}