using System.Threading;
using grabs.Audio;
using grabs.Audio.Devices;
using grabs.Audio.Stream;

Wav wav = new Wav(@"C:\Users\ollie\Documents\Audacity\18 Show Me Love.wav");

AudioDevice device = new SdlDevice(48000, 2);
Context context = device.Context;
//context.MasterVolume = 0.8f;

AudioBuffer buffer = context.CreateBuffer(new BufferDescription(PcmType.Pcm, wav.Format), wav.GetPCM());

AudioSource source = context.CreateSource();
source.SubmitBuffer(buffer);
//source.Speed = 1.5f;
//source.Volume = 0.5f;
source.Looping = true;
source.Play();

while (true)
{
    Thread.Sleep(1000);
}

device.Dispose();