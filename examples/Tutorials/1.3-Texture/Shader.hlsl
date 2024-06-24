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

SamplerState State : register(s0);
Texture2D Texture1  : register(t0);
Texture2D Texture2  : register(t1);

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = float4(input.Position, 1.0);
    output.TexCoord = input.TexCoord;
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    const float4 texture1 = Texture1.Sample(State, input.TexCoord);
    const float4 texture2 = Texture2.Sample(State, input.TexCoord);

    output.Color = lerp(texture1, texture2, 0.2);
    
    return output;
}