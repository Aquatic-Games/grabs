using System;

namespace grabs.Audio;

public enum DataType
{
    U8,
    I16,
    I32,
    F32
}

public static class DataTypeExtensions
{
    public static int Bits(this DataType dataType)
    {
        return dataType switch
        {
            DataType.U8 => 8,
            DataType.I16 => 16,
            DataType.I32 => 32,
            DataType.F32 => 32,
            _ => throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null)
        };
    }

    public static int Bytes(this DataType dataType)
        => dataType.Bits() / 8;
}