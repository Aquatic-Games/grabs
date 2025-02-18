namespace grabs.Graphics;

public struct InstanceInfo
{
    public string AppName;
    
    public Backend BackendHint;

    public bool Debug;

    public InstanceInfo(string appName, Backend backendHint = Backend.Unknown, bool debug = false)
    {
        AppName = appName;
        BackendHint = backendHint;
        Debug = debug;
    }
}