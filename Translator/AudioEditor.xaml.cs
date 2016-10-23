﻿using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TTSAutomate
{
    /// <summary>
    /// Interaction logic for AudioEditor.xaml
    /// </summary>
    public partial class AudioEditor : Window, INotifyPropertyChanged
    {
        StringBuilder sb = new StringBuilder();
        WaveFileReader wfr;
        WaveChannel32 sound0;

        float max = 0;
        float min = 1000;

        private int sampleRate = 0;

        public int SampleRate
        {
            get { return sampleRate; }
            set
            {
                sampleRate = value;
                OnPropertyChanged("SampleRate");
            }
        }

        private int bitsPerSample = 0;

        public int BitsPerSample
        {
            get { return bitsPerSample; }
            set
            {
                bitsPerSample = value;
                OnPropertyChanged("BitsPerSample");
            }
        }

        private int channels = 0;

        public int Channels
        {
            get { return channels; }
            set
            {
                channels = value;
                OnPropertyChanged("Channels");
            }
        }

        private String filename = "";

        public String FileName
        {
            get { return filename; }
            set
            {
                filename = value;
                wfr = new WaveFileReader(value);

                sound0 = new WaveChannel32(wfr);
                SampleRate = wfr.WaveFormat.SampleRate;
                BitsPerSample = wfr.WaveFormat.BitsPerSample;
                Duration = wfr.TotalTime;
                Channels = wfr.WaveFormat.Channels;
                pwfc.AddNewWaveForm(Color.FromRgb(67, 217, 150), SampleRate, BitsPerSample, Channels);
            }
        }

        private TimeSpan duration;

        public TimeSpan Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                OnPropertyChanged("Duration");
            }
        }


        int bufferSize = 1024;

        public AudioEditor()
        {
            InitializeComponent();
            // pwfc.AddNewWaveForm(Color.FromArgb(64, 255, 0, 0), sound1.TotalTime);
            FileName = @"J:\Videos\StopMotion\1SecondHum.wav";
            //FileName = @"J:\Videos\StopMotion\11.wav";
            //FileName = @"C:\temp\wav\system\CAP_Warn.wav";
            this.DataContext = this;
        }

        private void LoadSound(WaveChannel32 sound, int index)
        {
            int count = 0;
            byte[] buffer = new byte[bufferSize];
            int read = 0;
            sound.Sample += Sound0_Sample;


            while (sound.Position < sound.Length)
            {
                max =-1;
                min = 1;

                read = sound.Read(buffer, 0, bufferSize);
                sb.AppendFormat("{1}\t{2}\t{3}\t{0}\r\n", NAudio.Utils.Decibels.LinearToDecibels(max), index, count, max);
                pwfc.waveForms[index].AddValue(max, min);
                count++;
            }

            sound.Close();
            Debug.WriteLine("Sound is " + sound.TotalTime.TotalMilliseconds + "ms long");
            Debug.WriteLine("Sound is " + sound.Length + " bytes");
            Debug.WriteLine("Called addvalue " + count + " times");
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSound(sound0, 0);
            //LoadSound(sound1,1 );
            //Clipboard.SetText(sb.ToString());

            //var file = new AudioFileReader(@"C:\temp\wav\system\CAP_WARN.wav");
            //var trimmed = new OffsetSampleProvider(file);
            //trimmed.SkipOver = TimeSpan.FromMilliseconds(200);
            //trimmed.Take = TimeSpan.FromMilliseconds(1460);

            //WaveFileWriter.CreateWaveFile(@"c:\temp\trimmed.wav", new SampleToWaveProvider(trimmed));
            //var player = new WaveOutEvent();
            //player.Init(trimmed);
            //player.Play();
        }

        private void pwfc_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void pwfc_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var file = new AudioFileReader(FileName);
            var trimmed = new OffsetSampleProvider(file);
            trimmed.SkipOver =pwfc.SelectionStart;
            trimmed.Take = TimeSpan.FromMilliseconds(Math.Abs(pwfc.SelectionEnd.TotalMilliseconds - pwfc.SelectionStart.TotalMilliseconds));

            

            //WaveFileWriter.CreateWaveFile(@"c:\temp\trimmed.wav", new SampleToWaveProvider(trimmed));
            var player = new WaveOutEvent();
            player.Init(trimmed);
            player.Play();

        }

        private void Sound0_Sample(object sender, SampleEventArgs e)
        {
            max = Math.Max(max, e.Left);
            min = Math.Min(min, e.Left);


        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }



}
