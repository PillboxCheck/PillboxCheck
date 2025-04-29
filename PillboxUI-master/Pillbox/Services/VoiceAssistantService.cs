using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Globalization;
using System.Windows.Forms;
using Whisper.net;
using Whisper.net.Ggml;
using System.Configuration;
using KokoroSharp;
using KokoroSharp.Core;

namespace Pillbox.Services
{
    public class VoiceAssistantService
    {
        private SpeechRecognitionEngine _speechEngine;
        private bool _isActivated = false;
        private const string ActivationKeyword = "hello pillboxcheck";
        private const string DeactivationKeyword = "bye pillboxcheck";
        private NotifyIcon _notifyIcon;
        private Grammar _commandGrammar;
        private Grammar _questionGrammar;
        private KokoroTTS _tts; // Load or download the model (~320MB for full precision)
        private KokoroVoice _heartVoice;

        private AudioRecorder recoder;

        // Event to notify commands and questions recognized.
        public event EventHandler<string> CommandRecognized;
        public event EventHandler<string> QuestionRecognized;

        public bool IsActivated() => _isActivated;

        public VoiceAssistantService()
        {
            InitializeSpeechRecognition();
            InitializeNotifyIcon();
            recoder = new AudioRecorder();
            _tts =  KokoroTTS.LoadModel();
            _heartVoice = KokoroVoiceManager.GetVoice("af_heart");
        }

        public void SpeakTTS(string text)
        {
            // Use the TTS engine to speak the text.
            _tts.SpeakFast(text, _heartVoice);
        }
        private void InitializeNotifyIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                Icon = SystemIcons.Information 
            };
        }


        private void InitializeSpeechRecognition()
        {
            // Create recognizer with a specific culture, e.g., en-US.
            _speechEngine = new SpeechRecognitionEngine(new CultureInfo("en-GB"));

            // Build grammar with the matching culture
            var choices = new Choices(new string[] {
                ActivationKeyword,
                DeactivationKeyword,
                "open dashboard", 
                "show dashboard",
                "open inventory", 
                "show inventory",
                "open history",   
                "show history",
                "open settings",      
                "show settings",
                "open chat",      
                "show chat",
                "close application",
                "question",
                "help"
            });

            var grammarBuilder = new GrammarBuilder(choices)
            {
                Culture = new CultureInfo("en-GB")
            };

            _commandGrammar = new Grammar(grammarBuilder);
            _speechEngine.LoadGrammar(_commandGrammar);

            // Prepare a dictation grammar for capturing full questions
            _questionGrammar = new DictationGrammar();
            _questionGrammar.Name = "questionGrammar";

            _speechEngine.SetInputToDefaultAudioDevice();

            _speechEngine.SpeechRecognized += SpeechEngine_SpeechRecognized;
        }
        private async void SpeechEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Normalize recognized text
            string recognizedText = e.Result.Text.ToLower();

            // Check if the assistant is in deactivated state
            if (!_isActivated)
            {
                // Look for the activation keyword
                if (recognizedText.Contains(ActivationKeyword.ToLower()))
                {
                    _isActivated = true;
                    // Provide user feedback that the assistant is now active.
                    SpeakTTS("Hello, I am your voice assistant. How can I help you today?");

                    _notifyIcon.ShowBalloonTip(1000, "PillBoxCheck", "Voice Commands Activated", ToolTipIcon.Info);
                }
                // If not activated, ignore other commands.
                return;
            }
            else
            {
                // Check for switching to question mode.
                if (recognizedText.Contains("question"))
                {
                   
                    _notifyIcon.ShowBalloonTip(1000, "PillBoxCheck", "Please ask your question.", ToolTipIcon.Info);
                    string audioFilePath = await RecordQuestionAsync();
                    string questionText = await WhisperFunctions.TranscribeAudioAsync(audioFilePath);
                    QuestionRecognized?.Invoke(this, questionText);
                    return;
                }
                // Check for a deactivation command if you want to exit the active state.
                if (recognizedText.Contains(DeactivationKeyword.ToLower()))
                {
                    _isActivated = false;
                    _notifyIcon.ShowBalloonTip(1000, "PillBoxCheck", "Voice Commands Deactivated", ToolTipIcon.Info);
                    return;
                }

                // When activated, pass the command on for processing.
                CommandRecognized?.Invoke(this, recognizedText);
            }
        }
        //private static async Task DownloadModel(string fileName, GgmlType ggmlType)
        //{
        //    Console.WriteLine($"Downloading Model {fileName}");
        //    using var modelStream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(ggmlType);
        //    using var fileWriter = File.OpenWrite(fileName);
        //    await modelStream.CopyToAsync(fileWriter);
        //}
        private async Task<string> RecordQuestionAsync()
        {
            string audioFilePath = "question.wav";

            audioFilePath= await recoder.RecordWavUntilSilenceAsync(audioFilePath, 30000);

            return audioFilePath;
        }

    

        // Start voice recognition.
        public void Start()
        {
            _speechEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        // Stop voice recognition.
        public void Stop()
        {
            _speechEngine.RecognizeAsyncStop();
        }
    }
}
