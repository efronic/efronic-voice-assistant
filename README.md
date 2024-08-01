# efronic-voice-assistant

## Overview

The [`efronic-voice-assistant`](command:_github.copilot.openSymbolFromReferences?%5B%22%22%2C%5B%7B%22uri%22%3A%7B%22%24mid%22%3A1%2C%22fsPath%22%3A%22c%3A%5C%5CCode%5C%5Cefronic-voice-assistant%5C%5Cefronic-voice-assistant.sln%22%2C%22_sep%22%3A1%2C%22external%22%3A%22file%3A%2F%2F%2Fc%253A%2FCode%2Fefronic-voice-assistant%2Fefronic-voice-assistant.sln%22%2C%22path%22%3A%22%2Fc%3A%2FCode%2Fefronic-voice-assistant%2Fefronic-voice-assistant.sln%22%2C%22scheme%22%3A%22file%22%7D%2C%22pos%22%3A%7B%22line%22%3A5%2C%22character%22%3A53%7D%7D%5D%5D "Go to definition") is a voice-controlled assistant platform which runs on a raspberry pi. It leverages various AI models and APIs to provide speech-to-text, text-to-speech, and natural language understanding capabilities. The project integrates with OpenAI's GPT-4, Whisper, and other AI services to deliver a comprehensive voice assistant experience.

## Features

- **Speech-to-Text**: Converts spoken language into text using the Whisper model.
- **Text-to-Speech**: Converts text responses back into spoken language.
- **Natural Language Understanding**: Uses OpenAI's GPT-4 model to understand and respond to user queries.
- **Wake Word Detection**: Activates the assistant using a predefined wake word.
- **Endpoint Detection**: Automatically detects the end of user speech to process the input.

## Configuration

The application uses several configuration files to manage settings for different environments:

- [`appsettings.json`](command:_github.copilot.openRelativePath?%5B%7B%22scheme%22%3A%22file%22%2C%22authority%22%3A%22%22%2C%22path%22%3A%22%2Fc%3A%2FCode%2Fefronic-voice-assistant%2Fappsettings.json%22%2C%22query%22%3A%22%22%2C%22fragment%22%3A%22%22%7D%5D "c:\Code\efronic-voice-assistant\appsettings.json"): Default configuration.
- [`appsettings.Development.json`](command:_github.copilot.openRelativePath?%5B%7B%22scheme%22%3A%22file%22%2C%22authority%22%3A%22%22%2C%22path%22%3A%22%2Fc%3A%2FCode%2Fefronic-voice-assistant%2Fappsettings.Development.json%22%2C%22query%22%3A%22%22%2C%22fragment%22%3A%22%22%7D%5D "c:\Code\efronic-voice-assistant\appsettings.Development.json"): Development-specific settings.
- [`appsettings.Production.json`](command:_github.copilot.openRelativePath?%5B%7B%22scheme%22%3A%22file%22%2C%22authority%22%3A%22%22%2C%22path%22%3A%22%2Fc%3A%2FCode%2Fefronic-voice-assistant%2Fappsettings.Production.json%22%2C%22query%22%3A%22%22%2C%22fragment%22%3A%22%22%7D%5D "c:\Code\efronic-voice-assistant\appsettings.Production.json"): Production-specific settings.
- [`appsettings_example_json.json`](command:_github.copilot.openRelativePath?%5B%7B%22scheme%22%3A%22file%22%2C%22authority%22%3A%22%22%2C%22path%22%3A%22%2Fc%3A%2FCode%2Fefronic-voice-assistant%2Fappsettings_example_json.json%22%2C%22query%22%3A%22%22%2C%22fragment%22%3A%22%22%7D%5D "c:\Code\efronic-voice-assistant\appsettings_example_json.json"): Example configuration file with placeholders.

### Example Configuration ([`appsettings_example_json.json`](command:_github.copilot.openRelativePath?%5B%7B%22scheme%22%3A%22file%22%2C%22authority%22%3A%22%22%2C%22path%22%3A%22%2Fc%3A%2FCode%2Fefronic-voice-assistant%2Fappsettings_example_json.json%22%2C%22query%22%3A%22%22%2C%22fragment%22%3A%22%22%7D%5D "c:\Code\efronic-voice-assistant\appsettings_example_json.json"))

```json
{
  "OPENAI_API_KEY": "sk-example-XXXXXXXXXXXXXXXXXXXXXXXXXXXX",
  "OPENAI_GPT_MODEL": "gpt-4",
  "OPENAI_API_ENDPOINT": "chat/completions",
  "WHISPER_MODEL": "whisper-1",
  "PV_ACCESS_KEY": "example-access-key",
  "OPENAI_BASE_URL": "https://api.openai.com/v1/",
  "WHISPER_API_URL": "https://api.openai.com/v1/audio/transcriptions",
  "AWS_ACCESS_KEY_ID": "EXAMPLEAWSACCESSKEYID",
  "AWS_SECRET_ACCESS_KEY": "EXAMPLEAWSSECRETACCESSKEY",
  "CHEETAH_ACCESS_KEY": "example-access-key",
  "CHEETAH_ENDPOINT_DURATION_SEC": "3.0f",
  "CHEETAH_ENABLE_AUTOMATIC_PUNCTUATION": "true",
  "CHEETAH_AUDIO_DEVICE_INDEX": "-1",
  "mpg123Path": "./models/mpg123.exe",
  "MS_COPILOT_API_KEY_1": "example-api-key-1",
  "MS_COPILOT_API_KEY_2": "example-api-key-2",
  "MS_COPILOT_API_ENDPOINT": "openai/deployments/gpt-4o/chat/completions?api-version=2023-03-15-preview",
  "MS_COPILOT_BASE_URL": "https://example-voice-assistant.openai.azure.com/",
  "MS_COPILOT_GPT_MODEL": "gpt-4o",
  "Options": {
    "Tokens": "./models/sherpa-onnx-streaming-zipformer-en-2023-06-26/tokens.txt",
    "Provider": "cpu",
    "Encoder": "./models/sherpa-onnx-streaming-zipformer-en-2023-06-26/encoder-epoch-99-avg-1-chunk-16-left-128.onnx",
    "Decoder": "./models/sherpa-onnx-streaming-zipformer-en-2023-06-26/decoder-epoch-99-avg-1-chunk-16-left-128.onnx",
    "Joiner": "./models/sherpa-onnx-streaming-zipformer-en-2023-06-26/joiner-epoch-99-avg-1-chunk-16-left-128.onnx"
  }
}
```

## Project Structure (will be refined further)

```
.gitignore
.vscode/
AIClient.cs
appsettings_example_json.json
appsettings.Development.json
appsettings.json
appsettings.Production.json
AudioPlayer.cs
bin/
ConfigurationReloader.cs
efronic-voice-assistant.csproj
efronic-voice-assistant.sln
Hey-Wiz_en_linux_v3_0_0.ppn
Hey-Wiz_en_windows_v3_0_0.ppn
MicToText.cs
models/
obj/
OnnxOptions.cs
PicovoiceHandler.cs
Program.cs
PwmController.cs
run-paraformer.sh
run-transducer.sh
SpeechSynthesizer.cs
SpeechToTextController.cs
WaveHeader.cs
WhisperClient.cs
```

## Key Classes

- **[`Program`](command:_github.copilot.openSymbolFromReferences?%5B%22%22%2C%5B%7B%22uri%22%3A%7B%22%24mid%22%3A1%2C%22fsPath%22%3A%22c%3A%5C%5CCode%5C%5Cefronic-voice-assistant%5C%5CProgram.cs%22%2C%22_sep%22%3A1%2C%22external%22%3A%22file%3A%2F%2F%2Fc%253A%2FCode%2Fefronic-voice-assistant%2FProgram.cs%22%2C%22path%22%3A%22%2Fc%3A%2FCode%2Fefronic-voice-assistant%2FProgram.cs%22%2C%22scheme%22%3A%22file%22%7D%2C%22pos%22%3A%7B%22line%22%3A2%2C%22character%22%3A14%7D%7D%5D%5D "Go to definition")**: Entry point of the application. Manages initialization and main event loop.
- **`MicToText`**: Handles microphone input and converts speech to text.
- **[`SpeechSynthesizer`](command:_github.copilot.openSymbolFromReferences?%5B%22%22%2C%5B%7B%22uri%22%3A%7B%22%24mid%22%3A1%2C%22fsPath%22%3A%22c%3A%5C%5CCode%5C%5Cefronic-voice-assistant%5C%5CProgram.cs%22%2C%22_sep%22%3A1%2C%22external%22%3A%22file%3A%2F%2F%2Fc%253A%2FCode%2Fefronic-voice-assistant%2FProgram.cs%22%2C%22path%22%3A%22%2Fc%3A%2FCode%2Fefronic-voice-assistant%2FProgram.cs%22%2C%22scheme%22%3A%22file%22%7D%2C%22pos%22%3A%7B%22line%22%3A8%2C%22character%22%3A19%7D%7D%5D%5D "Go to definition")**: Converts text to speech.
- **[`AIClient`](command:_github.copilot.openSymbolFromReferences?%5B%22%22%2C%5B%7B%22uri%22%3A%7B%22%24mid%22%3A1%2C%22fsPath%22%3A%22c%3A%5C%5CCode%5C%5Cefronic-voice-assistant%5C%5CProgram.cs%22%2C%22_sep%22%3A1%2C%22external%22%3A%22file%3A%2F%2F%2Fc%253A%2FCode%2Fefronic-voice-assistant%2FProgram.cs%22%2C%22path%22%3A%22%2Fc%3A%2FCode%2Fefronic-voice-assistant%2FProgram.cs%22%2C%22scheme%22%3A%22file%22%7D%2C%22pos%22%3A%7B%22line%22%3A10%2C%22character%22%3A19%7D%7D%5D%5D "Go to definition")**: Communicates with OpenAI's GPT-4 model.
- **[`WhisperClient`](command:_github.copilot.openSymbolFromReferences?%5B%22%22%2C%5B%7B%22uri%22%3A%7B%22%24mid%22%3A1%2C%22fsPath%22%3A%22c%3A%5C%5CCode%5C%5Cefronic-voice-assistant%5C%5CProgram.cs%22%2C%22_sep%22%3A1%2C%22external%22%3A%22file%3A%2F%2F%2Fc%253A%2FCode%2Fefronic-voice-assistant%2FProgram.cs%22%2C%22path%22%3A%22%2Fc%3A%2FCode%2Fefronic-voice-assistant%2FProgram.cs%22%2C%22scheme%22%3A%22file%22%7D%2C%22pos%22%3A%7B%22line%22%3A14%2C%22character%22%3A19%7D%7D%5D%5D "Go to definition")**: Interfaces with the Whisper model for speech-to-text conversion.
- **[`PicovoiceHandler`](command:_github.copilot.openSymbolFromReferences?%5B%22%22%2C%5B%7B%22uri%22%3A%7B%22%24mid%22%3A1%2C%22fsPath%22%3A%22c%3A%5C%5CCode%5C%5Cefronic-voice-assistant%5C%5CProgram.cs%22%2C%22_sep%22%3A1%2C%22external%22%3A%22file%3A%2F%2F%2Fc%253A%2FCode%2Fefronic-voice-assistant%2FProgram.cs%22%2C%22path%22%3A%22%2Fc%3A%2FCode%2Fefronic-voice-assistant%2FProgram.cs%22%2C%22scheme%22%3A%22file%22%7D%2C%22pos%22%3A%7B%22line%22%3A13%2C%22character%22%3A19%7D%7D%5D%5D "Go to definition")**: Manages wake word detection.

## Running the Application

To run the application, use the following command:

```sh
dotnet run
```

Ensure that you have the necessary API keys and models configured in the [`appsettings.json`](command:_github.copilot.openRelativePath?%5B%7B%22scheme%22%3A%22file%22%2C%22authority%22%3A%22%22%2C%22path%22%3A%22%2Fc%3A%2FCode%2Fefronic-voice-assistant%2Fappsettings.json%22%2C%22query%22%3A%22%22%2C%22fragment%22%3A%22%22%7D%5D "c:\Code\efronic-voice-assistant\appsettings.json") file.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any improvements or bug fixes.

## Contact

For any questions or support, please contact the project maintainers.

---

This README provides an overview of the [`efronic-voice-assistant`](command:_github.copilot.openSymbolFromReferences?%5B%22%22%2C%5B%7B%22uri%22%3A%7B%22%24mid%22%3A1%2C%22fsPath%22%3A%22c%3A%5C%5CCode%5C%5Cefronic-voice-assistant%5C%5Cefronic-voice-assistant.sln%22%2C%22_sep%22%3A1%2C%22external%22%3A%22file%3A%2F%2F%2Fc%253A%2FCode%2Fefronic-voice-assistant%2Fefronic-voice-assistant.sln%22%2C%22path%22%3A%22%2Fc%3A%2FCode%2Fefronic-voice-assistant%2Fefronic-voice-assistant.sln%22%2C%22scheme%22%3A%22file%22%7D%2C%22pos%22%3A%7B%22line%22%3A5%2C%22character%22%3A53%7D%7D%5D%5D "Go to definition") project, its features, configuration, and how to run it. For more detailed information, refer to the source code and configuration files.