using System.Text.Json;
using Vosk;
using NAudio.Wave;

namespace PersonalAIAgent.Core.Services
{
    public class VoskVoiceService : IDisposable
    {
        private readonly object _stateLock = new();
        private readonly object _recognizerLock = new();
        private IWaveIn? _waveIn;
        private readonly VoskRecognizer _recognizer;
        private readonly Model _model;
        private int _disposeStarted;
        private bool _isListening;

        public Action<string>? OnPartialTranscript { get; set; }
        public Action<string>? OnFinalTranscript { get; set; }
        public Action? OnStopped { get; set; }

        public VoskVoiceService(string? modelPath = null)
        {
            Vosk.Vosk.SetLogLevel(-1);

            modelPath ??= Path.Combine(AppContext.BaseDirectory, "vosk-model-small-en-us-0.15");
            if (!Directory.Exists(modelPath))
            {
                throw new DirectoryNotFoundException($"The Vosk model directory was not found: {modelPath}");
            }

            _model = new Model(modelPath);
            _recognizer = new VoskRecognizer(_model, 16000.0f);
        }

        public void StartListening()
        {
            lock (_stateLock)
            {
                ThrowIfDisposed();
                if (_isListening) return;

                var waveIn = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(16000, 1)
                };
                waveIn.DataAvailable += WaveInDataAvailable;
                _waveIn = waveIn;
                _isListening = true;

                try
                {
                    waveIn.StartRecording();
                }
                catch
                {
                    _isListening = false;
                    _waveIn = null;
                    waveIn.DataAvailable -= WaveInDataAvailable;
                    waveIn.Dispose();
                    throw;
                }
            }
        }

        public void StopListening()
        {
            IWaveIn? waveIn;
            lock (_stateLock)
            {
                if (!_isListening) return;

                _isListening = false;
                waveIn = _waveIn;
                _waveIn = null;
                waveIn!.DataAvailable -= WaveInDataAvailable;
            }

            try
            {
                waveIn.StopRecording();
            }
            finally
            {
                waveIn.Dispose();
            }

            lock (_recognizerLock)
            {
                var finalJson = _recognizer.FinalResult();
                var text = ExtractTextFromJson(finalJson, "text");
                if (!string.IsNullOrWhiteSpace(text))
                {
                    OnFinalTranscript?.Invoke(text);
                }
            }

            OnStopped?.Invoke();
        }

        private void WaveInDataAvailable(object? sender, WaveInEventArgs e)
        {
            lock (_stateLock)
            {
                if (!_isListening || !ReferenceEquals(sender, _waveIn)) return;
            }

            lock (_recognizerLock)
            {
                if (_recognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
                {
                    PublishTranscript(_recognizer.Result(), "text", OnFinalTranscript);
                }
                else
                {
                    PublishTranscript(_recognizer.PartialResult(), "partial", OnPartialTranscript);
                }
            }
        }

        private void PublishTranscript(string json, string key, Action<string>? callback)
        {
            var text = ExtractTextFromJson(json, key);
            if (!string.IsNullOrWhiteSpace(text))
            {
                callback?.Invoke(text);
            }
        }

        private static string ExtractTextFromJson(string json, string key)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.TryGetProperty(key, out var element) &&
                       element.ValueKind == JsonValueKind.String
                    ? element.GetString() ?? string.Empty
                    : string.Empty;
            }
            catch (JsonException)
            {
                return string.Empty;
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposeStarted, 1) != 0) return;

            StopListening();
            _recognizer.Dispose();
            _model.Dispose();
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (Volatile.Read(ref _disposeStarted) != 0)
            {
                throw new ObjectDisposedException(nameof(VoskVoiceService));
            }
        }
    }
}
