# Cherry-Lite (Work in Progress)

Cherry-Lite is an advanced AI system inspired by Jarvis, based on the LAMBot-AI framework. This project combines the main features of LAMBot-AI with additional functionalities powered by xtts-api-server, ChatGPT (or Ollama), Whisper.net, and plugin support.

## Main Features

- **Intent Recognition**: Uses machine learning to classify user inputs into predefined intents.
- **Action Execution**: Executes specific actions, such as launching programs, based on recognized intents.
- **Response Generation**: Provides predefined responses for each recognized intent.
- **Model Persistence**: Saves and loads the trained model to avoid the need for retraining.

## Additional Features

- **Text-to-Speech Integration**: Utilizes xtts-api-server for converting text responses into speech.
- **Advanced NLP**: Incorporates ChatGPT or Ollama for more sophisticated natural language processing and understanding.
- **Speech Recognition**: Integrates with Whisper.net for accurate speech-to-text conversion.
- **Plugin Support**: Allows for extensibility through various plugins to add new functionalities.

## Getting Started

### Prerequisites

- .NET SDK
- Visual Studio or any other C# IDE
- xtts-api-server
- Whisper.net

### Installation

1. **Clone the repository:**

    ```sh
    git clone https://github.com/yourusername/Cherry-Lite.git
    cd Cherry-Lite
    ```

2. **Install the required NuGet packages:**

    ```sh
    dotnet add package Microsoft.ML
    dotnet add package Microsoft.ML.DataView
    dotnet add package Newtonsoft.Json
    ```

3. **Set up additional services:**

    - Follow the instructions to set up [xtts-api-server](https://github.com/daswer123/xtts-api-server)
    - Set up [Whisper.net](https://github.com/sandrohanea/whisper.net)

### Usage

1. **Prepare the `intents.json` file:**

    Create a file named `intents.json` in the project directory with the following content:

    ```json
    {
      "intents": [
        {
          "tag": "greeting",
          "patterns": [
            "Hi",
            "Hey",
            "How are you",
            "Is anyone there?",
            "Hello",
            "Good day"
          ],
          "responses": [
            "Hey :-)",
            "Hello, thanks for visiting",
            "Hi there, what can I do for you?",
            "Hi there, how can I help?"
          ]
        },
        {
          "tag": "goodbye",
          "patterns": [
            "Bye",
            "See you later",
            "Goodbye",
            "Have a nice day"
          ],
          "responses": [
            "Goodbye!",
            "See you later!",
            "Have a great day!",
            "Bye! Come back soon."
          ]
        },
        {
          "tag": "start_program",
          "patterns": [
            "Open Notepad",
            "Start Notepad",
            "Launch Notepad",
            "Run Notepad"
          ],
          "responses": [
            "Starting Notepad",
            "Opening Notepad",
            "Launching Notepad"
          ],
          "actions": [
            "notepad.exe"
          ]
        },
        {
          "tag": "start_browser",
          "patterns": [
            "Open browser",
            "Start browser",
            "Launch browser",
            "Run browser"
          ],
          "responses": [
            "Starting browser",
            "Opening browser",
            "Launching browser"
          ],
          "actions": [
            "C:\\Program Files\\Mozilla Firefox\\firefox.exe"
          ]
        }
      ]
    }
    ```

2. **Run the application:**

    ```sh
    dotnet run
    ```

3. **Interact with Cherry-Lite:**

    - Type `Hi` or `Hello` to receive a greeting.
    - Type `Open Notepad` to launch Notepad.
    - Type `Open browser` to launch your default browser.
    - Type `exit` to quit the chatbot.

## Project Structure

- **Program.cs**: Main program file containing the logic for training, saving, loading the model, and handling user interactions.
- **intents.json**: JSON file containing the intents, patterns, responses, and actions.
- **Plugins**: Directory for additional plugins to extend functionalities.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgements

- [ML.NET](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet) - Machine Learning framework for .NET
- [Newtonsoft.Json](https://www.newtonsoft.com/json) - JSON framework for .NET
- [xtts-api-server](https://github.com/daswer123/xtts-api-server) - Text-to-speech API server
- [Whisper.net](https://github.com/sandrohanea/whisper.net) - Speech recognition framework
