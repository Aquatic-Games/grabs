using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace grabs.Graphics;

public abstract class Device : IDisposable
{
    public abstract Swapchain CreateSwapchain(in SwapchainDescription description);

    public abstract CommandList CreateCommandList();

    public abstract Pipeline CreatePipeline(in PipelineDescription description);

    public unsafe Buffer CreateBuffer(in BufferDescription description)
        => CreateBuffer(description, null);

    public unsafe Buffer CreateBuffer<T>(BufferType type, T data, bool dynamic = false) where T : unmanaged
        => CreateBuffer(new BufferDescription(type, (uint) sizeof(T), dynamic), data);
    
    public Buffer CreateBuffer<T>(in BufferDescription description, T data) where T : unmanaged
        => CreateBuffer(description, new ReadOnlySpan<T>(ref data));

    public unsafe Buffer CreateBuffer<T>(BufferType type, in ReadOnlySpan<T> data, bool dynamic = false) where T : unmanaged
        => CreateBuffer(new BufferDescription(type, (uint) (data.Length * sizeof(T)), dynamic), data);

    public unsafe Buffer CreateBuffer<T>(BufferType type, T[] data, bool dynamic = false) where T : unmanaged
        => CreateBuffer(new BufferDescription(type, (uint) (data.Length * sizeof(T)), dynamic), data);
    
    public unsafe Buffer CreateBuffer<T>(in BufferDescription description, in ReadOnlySpan<T> data) where T : unmanaged
    {
        fixed (void* pData = data)
            return CreateBuffer(description, pData);
    }

    public unsafe Buffer CreateBuffer<T>(in BufferDescription description, T[] data) where T : unmanaged
    {
        fixed (void* pData = data)
            return CreateBuffer(description, pData);
    }

    public abstract unsafe Buffer CreateBuffer(in BufferDescription description, void* pData);

    public unsafe Texture CreateTexture(in TextureDescription description)
        => CreateTexture(description, null);

    public unsafe Texture CreateTexture<T>(in TextureDescription description, T[] data) where T : unmanaged
    {
        fixed (void* pData = data)
            return CreateTexture(description, &pData);
    }

    public unsafe Texture CreateTexture<T>(in TextureDescription description, T[][] datas) where T : unmanaged
    {
        GCHandle* handles = stackalloc GCHandle[datas.Length];
        void** pointers = stackalloc void*[datas.Length];

        for (int i = 0; i < datas.Length; i++)
        {
            handles[i] = GCHandle.Alloc(datas[i], GCHandleType.Pinned);
            pointers[i] = (void*) handles[i].AddrOfPinnedObject();
        }

        Texture texture = CreateTexture(description, pointers);

        for (int i = 0; i < datas.Length; i++)
            handles[i].Free();
        
        return texture;
    }
    
    public abstract unsafe Texture CreateTexture(in TextureDescription description, void** ppData);

    public abstract ShaderModule CreateShaderModule(ShaderStage stage, byte[] spirv, string entryPoint,
        SpecializationConstant[] constants = null);

    public Framebuffer CreateFramebuffer(Texture colorTexture, Texture depthTexture = null)
        => CreateFramebuffer(new ReadOnlySpan<Texture>(in colorTexture), depthTexture);
    
    public Framebuffer CreateFramebuffer(Texture[] colorTextures, Texture depthTexture = null)
        => CreateFramebuffer(colorTextures.AsSpan(), depthTexture);

    public abstract Framebuffer CreateFramebuffer(in ReadOnlySpan<Texture> colorTextures, Texture depthTexture = null);

    public abstract DescriptorLayout CreateDescriptorLayout(in DescriptorLayoutDescription description);

    public DescriptorSet CreateDescriptorSet(DescriptorLayout layout, params DescriptorSetDescription[] descriptions)
        => CreateDescriptorSet(layout, descriptions.AsSpan());

    public abstract DescriptorSet CreateDescriptorSet(DescriptorLayout layout, in ReadOnlySpan<DescriptorSetDescription> descriptions);

    public abstract Sampler CreateSampler(in SamplerDescription description);

    public unsafe void UpdateBuffer<T>(Buffer buffer, uint offsetInBytes, T data) where T : unmanaged
        => UpdateBuffer(buffer, offsetInBytes, (uint) sizeof(T), data);
    
    public void UpdateBuffer<T>(Buffer buffer, uint offsetInBytes, uint sizeInBytes, T data) where T : unmanaged
        => UpdateBuffer(buffer, offsetInBytes, sizeInBytes, new ReadOnlySpan<T>(ref data));

    public unsafe void UpdateBuffer<T>(Buffer buffer, uint offsetInBytes, in ReadOnlySpan<T> data) where T : unmanaged
        => UpdateBuffer(buffer, offsetInBytes, (uint) (data.Length * sizeof(T)), data);
    
    public unsafe void UpdateBuffer<T>(Buffer buffer, uint offsetInBytes, uint sizeInBytes, in ReadOnlySpan<T> data)
        where T : unmanaged
    {
        fixed (void* pData = data)
            UpdateBuffer(buffer, offsetInBytes, sizeInBytes, pData);
    }
    
    public abstract unsafe void UpdateBuffer(Buffer buffer, uint offsetInBytes, uint sizeInBytes, void* pData);

    public unsafe void UpdateTexture<T>(Texture texture, int x, int y, uint width, uint height, uint mipLevel, T[] data)
        where T : unmanaged
    {
        fixed (void* pData = data)
            UpdateTexture(texture, x, y, width, height, mipLevel, pData);
    }

    public unsafe void UpdateTexture<T>(Texture texture, int x, int y, uint width, uint height, uint mipLevel,
        in ReadOnlySpan<T> data) where T : unmanaged
    {
        fixed (void* pData = data)
            UpdateTexture(texture, x, y, width, height, mipLevel, pData);
    }
    
    public abstract unsafe void UpdateTexture(Texture texture, int x, int y, uint width, uint height, uint mipLevel, void* pData);
    
    public void UpdateDescriptorSet(DescriptorSet set, params DescriptorSetDescription[] descriptions)
        => UpdateDescriptorSet(set, descriptions.AsSpan());
    
    public abstract void UpdateDescriptorSet(DescriptorSet set, in ReadOnlySpan<DescriptorSetDescription> descriptions);
    
    public abstract nint MapBuffer(Buffer buffer, MapMode mapMode);

    public abstract void UnmapBuffer(Buffer buffer);

    public abstract void ExecuteCommandList(CommandList list);
    
    public abstract void Dispose();
}