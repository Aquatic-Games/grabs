using System;
using System.IO;
using System.Threading;
using grabs.Audio;
using grabs.Audio.Devices;

AudioFormat format1 = new AudioFormat(DataType.I16, 44100, Channels.Stereo);
AudioFormat format2 = new AudioFormat(DataType.F32, 48000, Channels.Stereo);

byte[] data1 = File.ReadAllBytes(@"C:\Users\ollie\Music\TESTFILES\Insomnia-16bitshort.raw");
byte[] data2 = File.ReadAllBytes(@"C:\Users\ollie\Music\TESTFILES\house2-f32.raw");

AudioDevice device = new SdlDevice(48000, 2);
Context context = device.Context;

AudioBuffer buffer1 = context.CreateBuffer(format1, new ReadOnlySpan<byte>(data1));
AudioBuffer buffer2 = context.CreateBuffer(format2, new ReadOnlySpan<byte>(data2));

AudioSource source1 = context.CreateSource();
source1.SubmitBuffer(buffer1);
source1.Play();

AudioSource source2 = context.CreateSource();
source2.SubmitBuffer(buffer2);
//source2.Play();

while (true)
{
    Thread.Sleep(1000);
}

device.Dispose();