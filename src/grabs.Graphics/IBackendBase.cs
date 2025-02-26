namespace grabs.Graphics;

/// <summary>
/// Represents a registerable and creatable backend.
/// </summary>
public interface IBackendBase
{
    /// <summary>
    /// <b>Do not use this method. Use <see cref="grabs.Graphics.Instance.Create(in InstanceInfo)"/> instead.</b>
    /// Create an instance for this backend.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public Instance CreateInstance(ref readonly InstanceInfo info);
}