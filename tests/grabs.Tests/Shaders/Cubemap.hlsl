struct VSInput
{
    float3 Position: POSITION0;
};

struct VSOutput
{
    float4 Position: SV_Position;
    float3 TexCoord: TEXCOORD0;
};

struct PSOutput
{
    float4 Color: SV_Target0;
};

cbuffer CameraMatrices : register(b0)
{
    float4x4 Projection;
    float4x4 View;
}

cbuffer WorldMatrices : register(b1)
{
    float4x4 World;
}

TextureCube Texture : register(t2);
SamplerState Sampler : register(s2);

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = mul(Projection, mul(View, mul(World, float4(input.Position, 1.0))));
    output.TexCoord = input.Position;
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = Texture.Sample(Sampler, input.TexCoord);
    
    return output;
}