using grabs;

InstanceInfo info = new InstanceInfo(Backend.Vulkan, "grabs.Tests", true);

Instance instance = Instance.Create(info);

instance.Dispose();