using System.Runtime.CompilerServices;
using grabs.Core;

namespace grabs.Graphics.Debugging;

internal sealed class DebugCommandList(CommandList cl) : CommandList
{
    public readonly CommandList CommandList = cl;
    
    private bool _isBegunRenderPass;

    private Format[] _renderPassColorFormats = new Format[8];
    private DebugPipeline? _currentlyBoundPipeline;
    
    public bool IsBegun;
    public bool HasIssuedCommands;

    public override bool IsDisposed
    {
        get => CommandList.IsDisposed;
        protected set => throw new NotImplementedException();
    }
    
    public override void Begin()
    {
        if (IsBegun)
            throw new ValidationException("Cannot begin command list! You must call End() first.");

        IsBegun = true;
        HasIssuedCommands = true;
        CommandList.Begin();
    }
    
    public override void End()
    {
        if (!IsBegun)
            throw new ValidationException("Cannot end command list! You must call Begin() first.");
        IsBegun = false;

        if (_isBegunRenderPass)
            throw new ValidationException("Cannot end command list! A render pass is currently active.");
        
        CommandList.End();
    }
    public override void BeginRenderPass(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments)
    {
        CheckIfBegun();
        
        if (_isBegunRenderPass)
            throw new ValidationException("Cannot begin render pass! Another render pass is currently active.");
        _isBegunRenderPass = true;
        
        if (colorAttachments.Length < 1)
            throw new ValidationException("There must be at least 1 color attachment in a render pass.");

        ColorAttachmentInfo[] convertedColorAttachments = new ColorAttachmentInfo[colorAttachments.Length];
        _renderPassColorFormats = new Format[colorAttachments.Length];
        
        Size2D size = colorAttachments[0].Texture.Size;
        for (int i = 0; i < colorAttachments.Length; i++)
        {
            DebugTexture texture = (DebugTexture) colorAttachments[i].Texture;
            
            if (texture.Size != size)
            {
                throw new ValidationException(
                    $"All color attachments must be the same size. Expected: {size}, Actual: {texture.Size}");
            }

            _renderPassColorFormats[i] = texture.Format;
            convertedColorAttachments[i] = new ColorAttachmentInfo()
            {
                Texture = texture.Texture,
                ClearColor = colorAttachments[i].ClearColor,
                LoadOp = colorAttachments[i].LoadOp,
                StoreOp = colorAttachments[i].StoreOp
            };
        }
        
        CommandList.BeginRenderPass(convertedColorAttachments);
    }
    
    public override void EndRenderPass()
    {
        CheckIfBegun();

        if (!_isBegunRenderPass)
            throw new ValidationException("Cannot end render pass! No render pass is currently active.");
        _isBegunRenderPass = false;
        
        CommandList.EndRenderPass();
    }

    public override void SetGraphicsPipeline(Pipeline pipeline)
    {
        CheckIfBegun();
        CheckIsInRenderPass();

        _currentlyBoundPipeline = (DebugPipeline) pipeline;
        CommandList.SetGraphicsPipeline(_currentlyBoundPipeline.Pipeline);
    }

    public override void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset = 0)
    {
        DebugBuffer debugBuffer = (DebugBuffer) buffer;
        CommandList.SetVertexBuffer(slot, debugBuffer.Buffer, stride, offset);
    }
    
    public override void SetIndexBuffer(Buffer buffer, Format format, uint offset = 0)
    {
        DebugBuffer debugBuffer = (DebugBuffer) buffer;
        CommandList.SetIndexBuffer(debugBuffer.Buffer, format, offset);
    }

    public override void Draw(uint numVertices)
    {
        CheckPipelineValidity();
        CommandList.Draw(numVertices);
    }

    public override void DrawIndexed(uint numIndices)
    {
        CommandList.DrawIndexed(numIndices);
    }

    public override void Dispose()
    {
        CommandList.Dispose();
    }

    private void CheckIfBegun()
    {
        if (!IsBegun)
            throw new ValidationException("You must call Begin() before you can issue any commands to the command list.");
    }

    private void CheckIsInRenderPass([CallerMemberName] string funcName = "")
    {
        if (!_isBegunRenderPass)
            throw new ValidationException($"{funcName} must only be called inside an active render pass.");
    }

    private void CheckPipelineValidity([CallerMemberName] string funcName = "")
    {
        CheckIfBegun();
        CheckIsInRenderPass();
        
        if (_currentlyBoundPipeline == null)
            throw new ValidationException("A valid graphics pipeline must be bound before draw calls can be issued.");

        if (_currentlyBoundPipeline.ColorAttachmentFormats.Length != _renderPassColorFormats.Length)
        {
            throw new ValidationException(
                $"Currently bound pipeline requires {_currentlyBoundPipeline.ColorAttachmentFormats.Length} color attachments, however the current render pass has {_renderPassColorFormats.Length}.");
        }

        for (int i = 0; i < _renderPassColorFormats.Length; i++)
        {
            if (_renderPassColorFormats[i] != _currentlyBoundPipeline.ColorAttachmentFormats[i])
            {
                throw new ValidationException(
                    $"Currently bound pipeline expected a color attachment with format {_currentlyBoundPipeline.ColorAttachmentFormats[i]} at index {i}, however the current render pass has format {_renderPassColorFormats[i]}.");
            }
        }
    }
}