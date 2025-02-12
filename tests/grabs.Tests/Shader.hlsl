struct VSInput
{
    uint VertexID : SV_VertexID;
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

    const float2 vertices[] = {
        float2(-0.5, -0.5),
        float2(0.0, 0.5),
        float2(0.5, -0.5)
    };

    const float4 colors[] = {
        float4(1.0, 0.0, 0.0, 1.0),
        float4(0.0, 1.0, 0.0, 1.0),
        float4(0.0, 0.0, 1.0, 1.0)
    };

    output.Position = float4(vertices[input.VertexID], 0.0, 1.0);
    output.Color = colors[input.VertexID];
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = input.Color;
    
    return output;
}