using grabs.Core;

namespace grabs.Graphics.Debugging;

internal sealed class DebugCommandList(CommandList cl) : CommandList
{
    public readonly CommandList CommandList = cl;
    
    private bool _isBegunRenderPass;
    
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

        Size2D size = colorAttachments[0].Texture.Size;
        for (int i = 1; i < colorAttachments.Length; i++)
        {
            if (colorAttachments[i].Texture.Size != size)
            {
                throw new ValidationException(
                    $"All color attachments must be the same size. Expected: {size}, Actual: {colorAttachments[i].Texture.Size}");
            }
        }
        
        CommandList.BeginRenderPass(in colorAttachments);
    }
    
    public override void EndRenderPass()
    {
        CheckIfBegun();

        if (!_isBegunRenderPass)
            throw new ValidationException("Cannot end render pass! No render pass is currently active.");
        _isBegunRenderPass = false;
        
        CommandList.EndRenderPass();
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
}