# Cherry-Lite (Work in Progress)
A few weeks ago, I developed Cherry A.I. as an app for myself, based on Ollama, XTTS, and Llama3. You can use this app on any device, whether it's a PC or a smartphone. Unlike Cherry-Lite, which offers all the features of Cherry A.I. but lacks an app or an attractive web interface, Cherry A.I. provides a full user experience. Cherry-Lite is like Cherry A.I., but without the interface.

![Cherry-A.i. Preview](https://aestheticdreams.ai/images/cherryai.png)

Cherry-Lite is an advanced AI system inspired by Jarvis, based on the LAMBot-AI framework. This project combines the main features of LAMBot-AI with additional functionalities powered by xtts-api-server, Ollama, Whisper.net, and plugin support.

## Cherry-Lite Peview
![Cherry-Lite Preview](https://aestheticdreams.ai/images/cherrylite.gif)

## NOT TESTED ON LINUX OR MAC 
Cherry-Lite is designed and optimized specifically for Windows. Compatibility with Linux and Mac systems has not been tested or verified.

## Main Features

- **Intent Recognition**: Uses machine learning to classify user inputs into predefined intents.
- **Action Execution**: Executes specific actions, such as launching programs, based on recognized intents.
- **Response Generation**: Provides predefined responses for each recognized intent.
- **Model Persistence**: Saves and loads the trained model to avoid the need for retraining.

## Additional Features

- **Whisper Transcription**: Added the ability to transcribe speech using Whisper.
- **Text-to-Speech Integration**: Utilizes xtts-api-server for converting text responses into speech.
- **Advanced NLP**: Incorporates Ollama for more sophisticated natural language processing and understanding.
- **Service Detection**: Improved detection and configuration for Ollama and XTTS services.
- **Audio Playback**: Uses NAudio to play text-to-speech audio responses.
- **Plugin Support (Coming soon)**: Allows for extensibility through various plugins to add new functionalities.

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
    dotnet add package NAudio.Lite
    ```

3. **Set up additional services:**

    - Follow the instructions to set up [xtts-api-server](https://github.com/daswer123/xtts-api-server)
    - Install [Ollama](https://ollama.ai)

### Prerequisites: Install and Run Ollama

1. **Install Ollama:**
   - Visit the [Ollama website](https://ollama.ai) to download and install the necessary package.

2. **Start Ollama:**
   - Open a terminal or command prompt and run:
     ```bash
     ollama start
     ```

### Prerequisites: Install and Run XTTS API Server

1. **Install the XTTS API Server:**
   - **Set up a Python virtual environment:**
     ```bash
     python -m venv venv
     source venv/bin/activate  # On Windows use `venv\Scripts\activate`
     ```
   - **Install the XTTS API Server:**
     ```bash
     pip install xtts-api-server
     ```
   - **Install PyTorch and Torchaudio for GPU support (optional but recommended):**
     ```bash
     pip install torch==2.1.1+cu118 torchaudio==2.1.1+cu118 --index-url https://download.pytorch.org/whl/cu118
     ```

2. **Run the XTTS API Server:**
   - Start the server:
     ```bash
     python -m xtts_api_server
     ```
   - The server runs by default on `localhost:8020`. To customize the host and port:
     ```bash
     python -m xtts_api_server --host 0.0.0.0 --port 8020
     ```

3. **Using Docker (Alternative Method):**
   - Clone the repository and use Docker Compose:
     ```bash
     git clone https://github.com/daswer123/xtts-api-server
     cd xtts-api-server/docker
     docker compose build
     docker compose up
     ```
   - This will also run the server on `localhost:8020` by default.

4. **Ensure a Voice Sample is Added:**
   - Add a voice sample named `cherry.wav` to the `speakers` folder inside the XTTS API Server directory. This is crucial for the Cherry-Lite assistant to generate responses correctly.

5. **Access the API Documentation:**
   - Visit [http://localhost:8020/docs](http://localhost:8020/docs) to explore the API endpoints and test the server.

For detailed instructions, refer to the [XTTS API Server GitHub repository](https://github.com/daswer123/xtts-api-server).

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
            "I will start Notepad.",
            "I am starting Notepad.",
            "Notepad is being started.",
            "Let me start Notepad."
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
            "I will start the browser.",
            "I am starting the browser.",
            "The browser is being started.",
            "Let me start the browser."
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
    - Type anything else and Ollama will respond.
    - Type `exit` to quit the chatbot.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgements

- [ML.NET](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet) - Machine Learning framework for .NET
- [Newtonsoft.Json](https://www.newtonsoft.com/json) - JSON framework for .NET
- [xtts-api-server](https://github.com/daswer123/xtts-api-server) - Text-to-speech API server
- [Whisper.net](https://github.com/sandrohanea/whisper.net) - Speech recognition framework
- [NAudio](https://github.com/naudio/NAudio) - Audio playback framework for .NET
