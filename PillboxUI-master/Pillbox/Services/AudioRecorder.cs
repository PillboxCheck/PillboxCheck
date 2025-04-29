using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using NAudio.Wave;
using System.Threading;

namespace Pillbox.Services
{
    class AudioRecorder
    {
        // A threshold in normalized amplitude [0,1]. Adjust this value as needed.
        private const float SilenceThreshold = 0.02f;
        // Duration of continuous silence (in milliseconds) required to stop recording.
        private const int MinimumSilenceDurationMs = 3000;

        /// <summary>
        /// Records audio from the default microphone until silence is detected and saves it as a WAV file.
        /// </summary>
        /// <param name="outputFilePath">The file path where the WAV file will be saved.</param>
        /// <param name="maxRecordingTimeMs">Optional maximum recording time (in ms) as a fallback.</param>
        /// <returns>The path to the saved WAV file once recording completes.</returns>
        public Task<string> RecordWavUntilSilenceAsync(string outputFilePath, int maxRecordingTimeMs = 30000)
        {
            var tcs = new TaskCompletionSource<string>();
            bool recordingStopped = false;

            var waveIn = new WaveInEvent
            {
                // Configure the recording format (e.g., 16 kHz, 16-bit, mono)
                WaveFormat = new WaveFormat(16000, 16, 1)
            };

            var writer = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);

            // Track the last time significant sound was detected.
            DateTime lastSoundTime = DateTime.Now;
            DateTime recordingStartTime = DateTime.Now;

            // Handler for incoming audio data.
            waveIn.DataAvailable += (s, e) =>
            {
                // Write the captured audio data to file.
                writer.Write(e.Buffer, 0, e.BytesRecorded);
                writer.Flush();

                // Analyze the audio buffer for sound (using 16-bit PCM samples).
                int bytesPerSample = waveIn.WaveFormat.BitsPerSample / 8;
                int sampleCount = e.BytesRecorded / bytesPerSample;
                float maxSampleValue = 0;

                for (int index = 0; index < e.BytesRecorded; index += bytesPerSample)
                {
                    short sample = BitConverter.ToInt16(e.Buffer, index);
                    // Normalize the sample to a float in [-1,1]
                    float normalizedSample = Math.Abs(sample / 32768f);
                    if (normalizedSample > maxSampleValue)
                        maxSampleValue = normalizedSample;
                }

                // If the maximum sample exceeds the threshold, update the time of last sound.
                if (maxSampleValue > SilenceThreshold)
                {
                    lastSoundTime = DateTime.Now;
                }
            };

            // When recording stops, dispose resources and complete the Task.
            waveIn.RecordingStopped += (s, e) =>
            {
                writer.Dispose();
                waveIn.Dispose();
                tcs.TrySetResult(outputFilePath);
            };

            waveIn.StartRecording();

            // Start a monitoring task to check for silence.
            Task.Run(async () =>
            {
                while (!recordingStopped)
                {
                    // Check if enough time has passed without significant sound.
                    if ((DateTime.Now - lastSoundTime).TotalMilliseconds > MinimumSilenceDurationMs)
                    {
                        recordingStopped = true;
                        waveIn.StopRecording();
                        break;
                    }

                    // Also enforce a maximum recording time as a safety fallback.
                    if ((DateTime.Now - recordingStartTime).TotalMilliseconds > maxRecordingTimeMs)
                    {
                        recordingStopped = true;
                        waveIn.StopRecording();
                        break;
                    }

                    // Wait briefly before checking again.
                    await Task.Delay(100);
                }
            });

            return tcs.Task;
        }
    }
}
