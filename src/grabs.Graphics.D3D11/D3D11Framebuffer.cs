using System;
using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.DirectX;
using static grabs.Graphics.D3D11.D3DResult;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public unsafe class D3D11Framebuffer : Framebuffer
{
    public readonly ID3D11RenderTargetView*[] RenderTargets;

    public readonly ID3D11DepthStencilView* DepthTarget;

    public D3D11Framebuffer(ID3D11Device* device, in ReadOnlySpan<Texture> colorTextures, Texture depthTexture)
    {
        RenderTargets = new ID3D11RenderTargetView*[colorTextures.Length];

        for (int i = 0; i < colorTextures.Length; i++)
        {
            fixed (ID3D11RenderTargetView** target = &RenderTargets[i])
                CheckResult(device->CreateRenderTargetView(((D3D11Texture) colorTextures[i]).Texture, null, target));
        }

        if (depthTexture != null)
        {
            fixed (ID3D11DepthStencilView** depthTarget = &DepthTarget)
                CheckResult(device->CreateDepthStencilView(((D3D11Texture) depthTexture).Texture, null, depthTarget));
        }
    }
    
    public override void Dispose()
    {
        if (DepthTarget != null)
            DepthTarget->Release();
        
        for (int i = 0; i < RenderTargets.Length; i++)
            RenderTargets[i]->Release();
    }
}