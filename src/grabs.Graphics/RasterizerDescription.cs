namespace grabs.Graphics;

public struct RasterizerDescription
{
    public FillMode FillMode;

    public CullFace CullFace;

    public CullDirection FrontFace;
    
    // TODO: Other bits

    public RasterizerDescription(FillMode fillMode, CullFace cullFace, CullDirection frontFace)
    {
        FillMode = fillMode;
        CullFace = cullFace;
        FrontFace = frontFace;
    }

    public static RasterizerDescription CullNone =>
        new RasterizerDescription(FillMode.Solid, CullFace.None, CullDirection.Clockwise);

    public static RasterizerDescription CullClockwise =>
        new RasterizerDescription(FillMode.Solid, CullFace.Back, CullDirection.Clockwise);

    public static RasterizerDescription CullCounterClockwise =>
        new RasterizerDescription(FillMode.Solid, CullFace.Back, CullDirection.CounterClockwise);
}