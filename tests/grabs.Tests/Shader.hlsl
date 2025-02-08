struct VSInput
{
    float2 Position: POSITION0;
    float4 Color:    COLOR0;
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

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = float4(input.Position, 0.0, 1.0);
    output.Color = input.Color;
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = input.Color;
    
    return output;
}