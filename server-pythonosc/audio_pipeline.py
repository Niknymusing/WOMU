import pyaudio
import wave
import numpy as np

def record_audio(length):
    RECORD_SECONDS = length or 10

    FRAMES_PERBUFF = 2048 
    FORMAT = pyaudio.paInt16 # 16 bit int
    CHANNELS = 1 
    SAMPLE_RATE = 44100
    
    p = pyaudio.PyAudio()

    #available audio devices
    info = p.get_host_api_info_by_index(0)
    numdevices = info.get('deviceCount')
    for i in range(0, numdevices):
            if (p.get_device_info_by_host_api_device_index(0, i).get('maxInputChannels')) > 0:
                print("Input Device id ", i, " - ", p.get_device_info_by_host_api_device_index(0, i).get('name'))

    stream = p.open(format=FORMAT,
                    channels=CHANNELS,
                    rate=SAMPLE_RATE,
                    input=True,
                    frames_per_buffer=FRAMES_PERBUFF,
                    input_device_index=1) #see output of available audio devices to select one.

    print("* recording started")

    frames = []
    nchunks = int(RECORD_SECONDS * SAMPLE_RATE / FRAMES_PERBUFF)
    for i in range(0, nchunks):
        data = stream.read(FRAMES_PERBUFF)
        frames.append(data) 

    print("* recording stopped")

    #saving numpy array
    numpydata = np.hstack(frames)

    stream.stop_stream()
    stream.close()
    p.terminate()

    #saving audio file
    wf = wave.open('audio_sample.wav', 'wb')
    wf.setnchannels(CHANNELS)
    wf.setsampwidth(p.get_sample_size(FORMAT))
    wf.setframerate(SAMPLE_RATE)
    wf.writeframes(b''.join(frames))
    wf.close()

    return numpydata