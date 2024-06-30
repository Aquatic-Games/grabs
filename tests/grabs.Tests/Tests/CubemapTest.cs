using System.IO;
using System.Numerics;
using grabs.Graphics;
using grabs.Tests.Utils;
using StbImageSharp;

namespace grabs.Tests.Tests;

public class CubemapTest : TestBase
{
    private Texture _cubemap;

    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;

    private Pipeline _pipeline;
    
    protected override void Initialize()
    {
        base.Initialize();

        ImageResult right = ImageResult.FromMemory(File.ReadAllBytes(@"C:\Users\ollie\Pictures\skybox\right.jpg"), ColorComponents.RedGreenBlueAlpha);
        ImageResult left = ImageResult.FromMemory(File.ReadAllBytes(@"C:\Users\ollie\Pictures\skybox\left.jpg"), ColorComponents.RedGreenBlueAlpha);
        ImageResult top = ImageResult.FromMemory(File.ReadAllBytes(@"C:\Users\ollie\Pictures\skybox\top.jpg"), ColorComponents.RedGreenBlueAlpha);
        ImageResult bottom = ImageResult.FromMemory(File.ReadAllBytes(@"C:\Users\ollie\Pictures\skybox\bottom.jpg"), ColorComponents.RedGreenBlueAlpha);
        ImageResult front = ImageResult.FromMemory(File.ReadAllBytes(@"C:\Users\ollie\Pictures\skybox\front.jpg"), ColorComponents.RedGreenBlueAlpha);
        ImageResult back = ImageResult.FromMemory(File.ReadAllBytes(@"C:\Users\ollie\Pictures\skybox\back.jpg"), ColorComponents.RedGreenBlueAlpha);

        _cubemap = Device.CreateTexture(
            TextureDescription.Cubemap((uint) right.Width, (uint) right.Height, 1, Format.R8G8B8A8_UNorm,
                TextureUsage.ShaderResource), [right.Data, left.Data, top.Data, bottom.Data, front.Data, back.Data]);

        Cube cube = new Cube();

        _vertexBuffer = Device.CreateBuffer(BufferType.Vertex, cube.Vertices);
        _indexBuffer = Device.CreateBuffer(BufferType.Index, cube.Indices);
        
        
    }

    protected override void Draw()
    {
        CommandList.Begin();

        CommandList.BeginRenderPass(new RenderPassDescription(Framebuffer, new Vector4(1.0f, 0.5f, 0.25f, 1.0f)));
        CommandList.EndRenderPass();
        
        CommandList.End();
        Device.ExecuteCommandList(CommandList);
    }

    public override void Dispose()
    {
        _cubemap.Dispose();
        
        base.Dispose();
    }

    public CubemapTest() : base("Skybox Test") { }
}