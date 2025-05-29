# Britney

Voice assistant project for our Software Engineering course in the Introduction to AI subject taught by Sir Bennett Tanyag.

Group members:
- Jacob Tarun [@MarshmallowAndroid](https://github.com/MarshmallowAndroid)
- Ian Alvarez [@proj-jra](https://github.com/proj-jra)
- Marc Briones [@Pluggedaxe](https://github.com/Pluggedaxe)
- Kyle Fabila [@elykfabl](https://github.com/elykfabl)

## Building
Open the solution, then build **GooeyWpf**.

## Running

This project makes use of the NVIDIA CUDA Toolkit for acceleration. Download it [here](https://developer.nvidia.com/cuda-downloads?target_os=Windows&target_arch=x86_64) for your version of Windows.
Otherwise, CPU fallback will be used which is not ideal.

### Whisper dependencies
Download a model of your choice [here](https://huggingface.co/ggerganov/whisper.cpp/tree/main).
Models with `en` will support only English. Otherwise, the model has translation support.

This project was tested on `ggml-medium-q8_0`.

### Piper dependencies
Download the [latest release of `piper`](https://github.com/rhasspy/piper/releases/latest) for Windows, `amd64`.

Additionally, this project requires the `semaine` voice from piper. You need to download two files:
* [en_GB-semaine-medium.onnx](https://huggingface.co/rhasspy/piper-voices/resolve/v1.0.0/en/en_GB/semaine/medium/en_GB-semaine-medium.onnx?download=true)
* [en_GB-semaine-medium.onnx.json](https://huggingface.co/rhasspy/piper-voices/resolve/v1.0.0/en/en_GB/semaine/medium/en_GB-semaine-medium.onnx.json?download=true.json)

To run the application, find the executable and ensure your directory structure matches the following:
```
<GooeyWpf.exe directory>
|   GooeyWpf.dll
|   GooeyWpf.exe
|   ...
|
+---Piper
|       piper.exe
|       onnxruntime.dll
|       ...
|
+---PiperVoice
|       en_GB-semaine-medium.onnx
|       en_GB-semaine-medium.onnx.json
|
\---WhisperCppModel
        ggml-xxxxxxxx-xxxx.bin
```

## Credits
* [whisper.net](https://github.com/sandrohanea/whisper.net), C# bindings for [whisper.cpp](https://github.com/ggerganov/whisper.cpp) for voice transcription.
* [piper](https://github.com/rhasspy/piper) for voice synthesis.
