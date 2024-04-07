using grabs;
using grabs.D3D11;

using Instance instance = new D3D11Instance();

Console.WriteLine(string.Join('\n', instance.EnumerateAdapters()));