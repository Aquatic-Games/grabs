using Vortice.Direct3D11;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11Pipeline : Pipeline
{
    public readonly ID3D11VertexShader VertexShader;

    public readonly ID3D11PixelShader PixelShader;

    public readonly ID3D11InputLayout? Layout;

    public readonly uint Stride;
    
    public D3D11Pipeline(ID3D11Device device, ref readonly PipelineInfo info)
    {
        D3D11ShaderModule vertexModule = (D3D11ShaderModule) info.VertexShader;
        D3D11ShaderModule pixelModule = (D3D11ShaderModule) info.PixelShader;

        VertexShader = device.CreateVertexShader(vertexModule.Blob);
        PixelShader = device.CreatePixelShader(pixelModule.Blob);

        if (info.InputLayout.Length > 0)
        {
            InputElementDescription[] elementDescs = new InputElementDescription[info.InputLayout.Length];

            for (int i = 0; i < info.InputLayout.Length; i++)
            {
                ref readonly InputLayoutInfo layout = ref info.InputLayout[i];

                elementDescs[i] = new InputElementDescription()
                {
                    SemanticName = "TEXCOORD",
                    SemanticIndex = (uint) i,
                    Format = layout.Format.ToD3D(),
                    Slot = layout.Slot,
                    AlignedByteOffset = layout.Offset,
                    Classification = InputClassification.PerVertexData
                };
            }

            Layout = device.CreateInputLayout(elementDescs, vertexModule.Blob);
        }

        Stride = info.Stride;
    }
    
    public override void Dispose()
    {
        Layout?.Dispose();
        PixelShader.Dispose();
        VertexShader.Dispose();
    }
}