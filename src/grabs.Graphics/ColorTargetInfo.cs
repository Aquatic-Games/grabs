using grabs.Core;

namespace grabs.Graphics;

public struct ColorTargetInfo(
    Texture texture,
    ColorF clearColor = default,
    LoadOp loadOp = LoadOp.Clear,
    StoreOp storeOp = StoreOp.Store)
{
    public Texture Texture = texture;

    public ColorF ClearColor = clearColor;

    public LoadOp LoadOp = loadOp;

    public StoreOp StoreOp = storeOp;
}