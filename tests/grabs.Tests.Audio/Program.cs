using System;
using System.IO;
using grabs.Audio;

AudioFormat format = new AudioFormat(DataType.I16, 48000, Channels.Stereo);
byte[] data = File.ReadAllBytes(@"C:\Users\ollie\Music\TESTFILES\nixonspace-16bitshort.raw");

Context context = new Context(48000);
AudioBuffer buffer = context.CreateBuffer(format, new ReadOnlySpan<byte>(data));

AudioSource source = context.CreateSource();
source.SubmitBuffer(buffer);