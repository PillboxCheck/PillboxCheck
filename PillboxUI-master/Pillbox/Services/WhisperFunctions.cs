﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whisper.net.Ggml;
using Whisper.net.Logger;
using Whisper.net;


namespace Pillbox.Services
{
    class WhisperFunctions
    {
        // This examples shows how to use Whisper.net to create a transcription from an audio file with 16Khz sample rate.
        public static async Task<string> TranscribeAudioAsync(string wavFileName)
        {
            // We declare three variables which we will use later, ggmlType, modelFileName and wavFileName
            var ggmlType = GgmlType.Base;
            var modelFileName = "ggml-base.bin";

            // This section detects whether the "ggml-base.bin" file exists in our project disk. If it doesn't, it downloads it from the internet
            if (!File.Exists(modelFileName))
            {
                await DownloadModel(modelFileName, ggmlType);
            }

            // Optional logging from the native library
            using var whisperLogger = LogProvider.AddConsoleLogging(WhisperLogLevel.Debug);

            // This section creates the whisperFactory object which is used to create the processor object.
            using var whisperFactory = WhisperFactory.FromPath("ggml-base.bin");

            // This section creates the processor object which is used to process the audio file, it uses language `auto` to detect the language of the audio file.
            using var processor = whisperFactory.CreateBuilder()
                .WithLanguage("auto")
                .Build();

            using var fileStream = File.OpenRead(wavFileName);

            // This section processes the audio file and prints the results (start time, end time and text) to the console.
            string output = "";
            await foreach (var result in processor.ProcessAsync(fileStream))
            {
                output += $"{result.Text}";
            }
            return output;
        }

        private static async Task DownloadModel(string fileName, GgmlType ggmlType)
        {
            Console.WriteLine($"Downloading Model {fileName}");
            using var modelStream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(ggmlType);
            using var fileWriter = File.OpenWrite(fileName);
            await modelStream.CopyToAsync(fileWriter);
        }
    }
}
