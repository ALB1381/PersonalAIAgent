window.voiceInterop = {
    recognition: null,
    startListening: function (dotNetHelper) {
        // Handle browser prefixes
        const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
        if (!SpeechRecognition) {
            console.error("Speech recognition is not supported in this environment.");
            return;
        }

        this.recognition = new SpeechRecognition();
        this.recognition.continuous = false;
        this.recognition.interimResults = true; // Allows live updating of the text box

        this.recognition.onresult = (event) => {
            let interimTranscript = '';
            let finalTranscript = '';

            for (let i = event.resultIndex; i < event.results.length; ++i) {
                if (event.results[i].isFinal) {
                    finalTranscript += event.results[i][0].transcript;
                } else {
                    interimTranscript += event.results[i][0].transcript;
                }
            }

            if (finalTranscript !== '') {
                // Auto-send when the user stops speaking
                dotNetHelper.invokeMethodAsync('ProcessVoiceCommand', finalTranscript.trim());
            } else {
                // Show words as they are spoken
                dotNetHelper.invokeMethodAsync('UpdateTranscript', interimTranscript);
            }
        };

        this.recognition.onend = () => {
            // Optional: Handle cleanup when speech engine stops
        };

        this.recognition.start();
    },
    
    stopListening: function () {
        if (this.recognition) {
            this.recognition.stop();
        }
    }
};