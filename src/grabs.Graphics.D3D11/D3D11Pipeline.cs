using Vortice.Direct3D11;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11Pipeline : Pipeline
{
    public const uint MaxPushConstantSize = 256;
    
    public readonly ID3D11VertexShader VertexShader;
    public readonly Dictionary<uint, Dictionary<uint, uint>> VertexRemappings;

    public readonly ID3D11PixelShader PixelShader;
    public readonly Dictionary<uint, Dictionary<uint, uint>> PixelRemappings;

    public readonly ID3D11InputLayout? Layout;

    public readonly Dictionary<ShaderStage, ID3D11Buffer> PushConstantBuffers;
    
    public D3D11Pipeline(ID3D11Device device, ref readonly PipelineInfo info)
    {
        D3D11ShaderModule vertexModule = (D3D11ShaderModule) info.VertexShader;
        D3D11ShaderModule pixelModule = (D3D11ShaderModule) info.PixelShader;

        VertexShader = device.CreateVertexShader(vertexModule.Blob);
        VertexRemappings = vertexModule.Remappings;
        
        PixelShader = device.CreatePixelShader(pixelModule.Blob);
        PixelRemappings = pixelModule.Remappings;

        if (info.InputLayout.Length > 0)
        {
            InputElementDescription[] elementDescs = new InputElementDescription[info.InputLayout.Length];

            for (int i = 0; i < info.InputLayout.Length; i++)
            {
                ref readonly InputElement element = ref info.InputLayout[i];

                elementDescs[i] = new InputElementDescription()
                {
                    SemanticName = "TEXCOORD",
                    SemanticIndex = (uint) i,
                    Format = element.Format.ToD3D(),
                    Slot = element.Slot,
                    AlignedByteOffset = element.Offset,
                    Classification = InputClassification.PerVertexData
                };
            }

            Layout = device.CreateInputLayout(elementDescs, vertexModule.Blob);
        }

        /*PushConstantBuffers = [];
        uint vertexConstSize = 0;
        uint pixelConstSize = 0;
        
        for (int i = 0; i < info.Constants.Length; i++)
        {
            ref readonly ConstantLayout constant = ref info.Constants[i];

            switch (constant.Stages)
            {
                
            }
        }*/
    }
    
    public override void Dispose()
    {
        Layout?.Dispose();
        PixelShader.Dispose();
        VertexShader.Dispose();
    }
}