namespace grabs.Graphics;

public struct DepthStencilDescription
{
    public bool DepthEnabled;

    public bool DepthWrite;

    public ComparisonFunction ComparisonFunction;

    public DepthStencilDescription(bool depthEnabled, bool depthWrite, ComparisonFunction comparisonFunction)
    {
        DepthEnabled = depthEnabled;
        DepthWrite = depthWrite;
        ComparisonFunction = comparisonFunction;
    }

    public static DepthStencilDescription Disabled =>
        new DepthStencilDescription(false, false, ComparisonFunction.Never);

    public static DepthStencilDescription DepthLessEqual =>
        new DepthStencilDescription(true, true, ComparisonFunction.LessEqual);
}