using grabs.Core;
using grabs.Graphics;
using grabs.Graphics.Vulkan;

GrabsLog.LogMessage += (severity, message, line, file) => Console.WriteLine($"{severity}: {message}");

Instance.RegisterBackend<VulkanBackend>();

using Instance instance = Instance.Create(new InstanceInfo("test", true));