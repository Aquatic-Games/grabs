namespace grabs.Graphics;

public struct InputElementDescription(SemanticType semantic, uint semanticIndex, Format format, uint offset)
{
    public SemanticType Semantic = semantic;

    public uint SemanticIndex = semanticIndex;

    public Format Format = format;

    public uint Offset = offset;
}