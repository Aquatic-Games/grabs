namespace grabs.Graphics;

public struct SpecializationConstant
{
    public uint Id;
    public ConstantType Type;
    public long Value;

    public SpecializationConstant(uint id, ConstantType type, long value)
    {
        Id = id;
        Type = type;
        Value = value;
    }

    public SpecializationConstant(uint id, bool value)
    {
        Id = id;
        Type = ConstantType.U8;
        Value = value ? 1 : 0;
    }
    
    public SpecializationConstant(uint id, sbyte value)
    {
        Id = id;
        Type = ConstantType.I8;
        Value = value;
    }
    
    public SpecializationConstant(uint id, short value)
    {
        Id = id;
        Type = ConstantType.I16;
        Value = value;
    }
    
    public SpecializationConstant(uint id, int value)
    {
        Id = id;
        Type = ConstantType.I32;
        Value = value;
    }
    
    public SpecializationConstant(uint id, long value)
    {
        Id = id;
        Type = ConstantType.I64;
        Value = value;
    }

    public SpecializationConstant(uint id, byte value)
    {
        Id = id;
        Type = ConstantType.U8;
        Value = value;
    }
    
    public SpecializationConstant(uint id, ushort value)
    {
        Id = id;
        Type = ConstantType.U16;
        Value = value;
    }
    
    public SpecializationConstant(uint id, uint value)
    {
        Id = id;
        Type = ConstantType.U32;
        Value = value;
    }
    
    public SpecializationConstant(uint id, ulong value)
    {
        Id = id;
        Type = ConstantType.U64;
        Value = (long) value;
    }
    
    public unsafe SpecializationConstant(uint id, float value)
    {
        Id = id;
        Type = ConstantType.F32;
        Value = *(int*) &value;
    }
    
    public unsafe SpecializationConstant(uint id, double value)
    {
        Id = id;
        Type = ConstantType.F64;
        Value = *(long*) &value;
    }
}