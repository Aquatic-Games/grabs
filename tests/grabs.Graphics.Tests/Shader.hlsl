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

/*cbuffer Matrices : register(b0, space0)
{
    float4x4 Matrix;
}*/

struct Matrices
{
    float4x4 Matrix;
};
[[vk::push_constant]] Matrices Transform;

Texture2D Texture : register(t1, space0);
SamplerState State : register(s1, space0);

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = mul(Transform.Matrix, float4(input.Position, 1.0));
    output.TexCoord = input.TexCoord;
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = Texture.Sample(State, input.TexCoord);
    
    return output;
}