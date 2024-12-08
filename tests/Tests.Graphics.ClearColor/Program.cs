using grabs.Graphics;

InstanceDescription desc = new InstanceDescription();

using Instance instance = Instance.Create(in desc, null);

Adapter[] adapters = instance.EnumerateAdapters();
foreach (Adapter adapter in adapters)
    Console.WriteLine(adapter);