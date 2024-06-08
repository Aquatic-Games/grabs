using System;
using System.Threading;
using grabs.Audio;
using grabs.Audio.Devices;
using grabs.Audio.Stream;

Wav wav = new Wav(@"C:\Users\ollie\Music\Always There MONO.wav");

AudioDevice device = new SdlDevice(48000, 2);
Context context = device.Context;

AudioBuffer buffer = context.CreateBuffer(wav.Format, new ReadOnlySpan<byte>(wav.GetPCM()));

AudioSource source = context.CreateSource();
source.SubmitBuffer(buffer);
source.Play();

while (true)
{
    Thread.Sleep(1000);
}

device.Dispose();