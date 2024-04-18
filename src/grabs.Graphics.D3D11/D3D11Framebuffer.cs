using System;
using Vortice.Direct3D11;

namespace grabs.Graphics.D3D11;

public class D3D11Framebuffer : Framebuffer
{
    public readonly ID3D11RenderTargetView[] RenderTargets;

    public readonly ID3D11DepthStencilView DepthTarget;

    public D3D11Framebuffer(ID3D11Device device, in ReadOnlySpan<Texture> colorTextures, Texture depthTexture)
    {
        RenderTargets = new ID3D11RenderTargetView[colorTextures.Length];
        
        for (int i = 0; i < colorTextures.Length; i++)
            RenderTargets[i] = device.CreateRenderTargetView(((D3D11Texture) colorTextures[i]).Texture);

        if (depthTexture != null)
            DepthTarget = device.CreateDepthStencilView(((D3D11Texture) depthTexture).Texture);
    }
    
    public override void Dispose()
    {
        for (int i = 0; i < RenderTargets.Length; i++)
            RenderTargets[i].Dispose();
        
        DepthTarget?.Dispose();
    }
}