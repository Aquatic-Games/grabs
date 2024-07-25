using System;
using grabs.Graphics;
using grabs.ShaderCompiler.DXC;
using grabs.ShaderCompiler.Spirv;

const ShaderStage stage = ShaderStage.Pixel;
const string entryPoint = "Pixel";

const string shaderCode = """
                          struct VSInput
                          {
                              float3 Position: POSITION0;
                              float2 TexCoord: TEXCOORD0;
                              float4 Color:    COLOR0;
                          };

                          struct VSOutput
                          {
                              float4 Position: SV_Position;
                              float2 TexCoord: TEXCOORD0;
                              float4 Color:    COLOR0;
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
                          
                          cbuffer DrawInfo : register(b1, space0)
                          {
                              float4x4 World;
                          }
                          
                          Texture2D Texture  : register(t0, space1);
                          SamplerState state : register(s0, space1);

                          VSOutput Vertex(const in VSInput input)
                          {
                              VSOutput output;
                              
                              output.Position = mul(Projection, mul(View, mul(World, float4(input.Position, 1.0))));
                              output.TexCoord = input.TexCoord;
                              output.Color = input.Color;
                              
                              return output;
                          }
                          
                          PSOutput Pixel(const in VSOutput input)
                          {
                              PSOutput output;
                              
                              output.Color = Texture.Sample(state, input.TexCoord);
                              
                              return output;
                          }
                          """;

byte[] result = Compiler.CompileToSpirV(shaderCode, entryPoint, stage, true);
Console.WriteLine(SpirvCompiler.TranspileSpirv(stage, ShaderLanguage.Glsl430, result, entryPoint, out DescriptorRemappings remappings));

foreach ((uint set, Remapping remapping) in remappings.Sets)
{
    foreach ((uint originalBinding, uint newBinding) in remapping.Bindings)
    {
        Console.WriteLine($"Remapping: Set {set}, Binding: {originalBinding} -> Binding {newBinding}");
    }
}