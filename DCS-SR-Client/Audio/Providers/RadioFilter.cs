﻿using Ciribob.DCS.SimpleRadio.Standalone.Client.Settings;
using MathNet.Filtering;
using MathNet.Filtering.FIR;
using MathNet.Filtering.Windowing;
using NAudio.Dsp;
using NAudio.Wave;

namespace Ciribob.DCS.SimpleRadio.Standalone.Client.DSP
{
    public class RadioFilter : ISampleProvider
    {
        private readonly ISampleProvider _source;

        private OnlineFilter[] _filters;
        //    private Stopwatch _stopwatch;

        private Settings.SettingsStore _settings = Settings.SettingsStore.Instance;

        public RadioFilter(ISampleProvider sampleProvider)
        {
            _source = sampleProvider;
            _filters = new OnlineFilter[2];

            /**
             * From Coug4r
             * Apart from adding noise i'm only doing 3 things in this order:
                - Custom clipping (which basicly creates the overmodulation effect) 
                - Run the audio through bandpass filter 1
                - Run the audio through bandpass filter 2

                These are the values i use for the bandpass filters:
                1 - low 560, high 3900
                2 - low 100, high 4500

             */

            _filters[0] = OnlineFilter.CreateBandpass(ImpulseResponse.Finite, sampleProvider.WaveFormat.SampleRate, 560,
                3900);
            _filters[1] = OnlineFilter.CreateBandpass(ImpulseResponse.Finite, sampleProvider.WaveFormat.SampleRate, 100,
                4500);
        }

        public WaveFormat WaveFormat
        {
            get { return _source.WaveFormat; }
        }

        public int Read(float[] buffer, int offset, int sampleCount)
        {
            var samplesRead = _source.Read(buffer, offset, sampleCount);
            if (_settings.GetClientSetting(SettingsKeys.RadioEffects).BoolValue)
            {
                if (samplesRead > 0)
                {
                    for (var n = 0; n < sampleCount; n++)
                    {
                        var audio = (double) buffer[offset + n];
                        if (audio != 0)
                            // because we have silence in one channel (if a user picks radio left or right ear) we don't want to transform it or it'll play in both
                        {
                            for (int i = 0; i < _filters.Length; i++)
                            {
                                var filter = _filters[i];
                                audio = filter.ProcessSample(audio);

                                if (double.IsNaN(audio))
                                    audio = buffer[offset + n];
                            }


                            buffer[offset + n] = (float) audio;
                        }
                    }
                }
            }


            //   Console.WriteLine("Read:"+samplesRead+" Time - " + _stopwatch.ElapsedMilliseconds);
            //     _stopwatch.Restart();
//
            return samplesRead;
        }
    }
}