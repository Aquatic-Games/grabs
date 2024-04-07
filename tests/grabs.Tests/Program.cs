using grabs;
using grabs.D3D11;

using Instance instance = new D3D11Instance();

Adapter[] adapters = instance.EnumerateAdapters();

Console.WriteLine(string.Join('\n', adapters));

using Device device = instance.CreateDevice();
