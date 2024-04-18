using System.Text;
using grabs.Graphics;
using grabs.ShaderCompiler.DXC;

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
                          };

                          VSOutput Vertex(const in VSInput input)
                          {
                              VSOutput output;
                              
                              output.Position = float4(input.Position, 1.0);
                              
                              return output;
                          }
                          """;

byte[] result = Compiler.CompileToSpirV(shaderCode, "Vertex", ShaderStage.Vertex);
Console.WriteLine(Encoding.UTF8.GetString(result));