namespace grabs.Graphics;

public struct InstanceInfo
{
    public Backend BackendHint;

    public string AppName;

    public bool Debug;

    public InstanceInfo(Backend backendHint, string appName, bool debug = false)
    {
        BackendHint = backendHint;
        AppName = appName;
        Debug = debug;
    }
}