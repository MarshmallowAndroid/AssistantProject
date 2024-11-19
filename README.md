# Britney

Voice assistant project for our Software Engineering course in Introduction to AI subject taught by Sir Bennett Tanyag.

Group members:
- Jacob Tarun [@MarshmallowAndroid](https://github.com/MarshmallowAndroid)
- Ian Alvarez [@proj-jra](https://github.com/proj-jra)
- Marc Briones [@Pluggedaxe](https://github.com/Pluggedaxe)
- Kyle Fabila

## Building
Open the solution, then build **GooeyWpf**.

## Running

### Whisper dependencies
Download a model of your choice [here](https://huggingface.co/ggerganov/whisper.cpp/tree/main)

Models with `en` supports only English. Otherwise, the model has translation support.

This project was tested on `ggml-medium-q8_0`.

Find the built executable and place the model in a directory named `WhisperCppModel`.

### Piper dependencies
Download the [latest release of `piper`](https://github.com/rhasspy/piper/releases/latest) for Windows, `amd64`.

Extract the contents of the ZIP file into a directory with the built executable named `Piper`.

This project requires the `semaine` voice from piper. You need to download two files:
* [en_GB-semaine-medium.onnx](https://huggingface.co/rhasspy/piper-voices/resolve/v1.0.0/en/en_GB/semaine/medium/en_GB-semaine-medium.onnx?download=true)
* [en_GB-semaine-medium.onnx.json](https://huggingface.co/rhasspy/piper-voices/resolve/v1.0.0/en/en_GB/semaine/medium/en_GB-semaine-medium.onnx.json?download=true.json)

Place these two files in a directory with the build executable named `PiperVoice`.

## Credits
* [whisper.net](https://github.com/sandrohanea/whisper.net), C# bindings for [whisper.cpp](https://github.com/ggerganov/whisper.cpp) for voice transcription
* [piper](https://github.com/rhasspy/piper) for voice synthesis.