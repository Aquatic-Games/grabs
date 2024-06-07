using System;
using System.IO;
using System.Threading;
using grabs.Audio;
using grabs.Audio.Devices;

AudioFormat format = new AudioFormat(DataType.F32, 48000, Channels.Stereo);
byte[] data = File.ReadAllBytes(@"C:\Users\ollie\Music\TESTFILES\nixonspace-32bitfloat.raw");

AudioDevice device = new SdlDevice(48000, 2);
Context context = device.Context;

AudioBuffer buffer = context.CreateBuffer(format, new ReadOnlySpan<byte>(data));

AudioSource source = context.CreateSource();
source.SubmitBuffer(buffer);

while (true)
{
    Thread.Sleep(1000);
}

device.Dispose();