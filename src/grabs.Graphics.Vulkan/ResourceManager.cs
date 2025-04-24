namespace grabs.Graphics.Vulkan;

// The Resource Manager keeps track of all Vulkan resources.
// It's against the Vulkan spec to destroy devices + instances without first destroying their child objects.
// The Resource Manager will automatically handle object destruction ensuring that the application conforms to the spec.
internal static class ResourceManager
{
    private static readonly Dictionary<VulkanInstance, HashSet<IDisposable>> _instanceResources;

    private static bool _isDisposingAllResources;

    static ResourceManager()
    {
        _instanceResources = [];
    }
    
    public static void RegisterInstanceResource(VulkanInstance instance, IDisposable resource)
    {
        if (!_instanceResources.TryGetValue(instance, out HashSet<IDisposable> resources))
        {
            resources = [];
            _instanceResources.Add(instance, resources);
        }

        resources.Add(resource);
    }

    public static void DeregisterInstanceResource(VulkanInstance instance, IDisposable resource)
    {
        // Special case where DisposeAllXResources is called, avoids removing while iterating.
        if (_isDisposingAllResources)
            return;
        
        _instanceResources[instance].Remove(resource);
    }

    public static void DisposeAllInstanceResources(VulkanInstance instance)
    {
        _isDisposingAllResources = true;
        
        foreach (IDisposable resource in _instanceResources[instance])
            resource.Dispose();

        _isDisposingAllResources = false;
        _instanceResources.Clear();
    }
}