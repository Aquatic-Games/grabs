struct VSInput
{
    float3 Position: POSITION0;
    float2 TexCoord: TEXCOORD0;
};

struct VSOutput
{
    float4 Position: SV_Position;
    float2 TexCoord: TEXCOORD0;
};

struct PSOutput
{
    float4 Color: SV_Target0;
};

cbuffer CameraMatrices : register(b0, space0)
{
    float4x4 Projection;
    float4x4 View;
}

cbuffer WorldMatrix : register(b2, space0)
{
    float4x4 World;
}

SamplerState Sampler : register(s10, space1);
Texture2D Texture : register(t1, space1);

VSOutput Vertex(const in VSInput input)
{
    VSOutput output;

    output.Position = mul(Projection, mul(View, mul(World, float4(input.Position, 1.0))));
    output.TexCoord = input.TexCoord;
    
    return output;
}

PSOutput Pixel(const in VSOutput input)
{
    PSOutput output;

    output.Color = Texture.Sample(Sampler, input.TexCoord);
    
    return output;
}