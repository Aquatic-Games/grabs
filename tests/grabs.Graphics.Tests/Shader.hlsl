struct VSInput
{
    float3 Position: POSITION0;
    float3 Color:    COLOR0;
};

struct VSOutput
{
    float4 Position: SV_Position;
    float4 Color:    COLOR0;
};

struct PSOutput
{
    float4 Color: SV_Target0;
};

cbuffer Matrices : register(b0)
{
    float4x4 Matrix;
}

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = mul(Matrix, float4(input.Position, 1.0));
    output.Color = float4(input.Color, 1.0);
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = input.Color;
    
    return output;
}