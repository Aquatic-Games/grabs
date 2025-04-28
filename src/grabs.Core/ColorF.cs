namespace grabs.Core;

public struct ColorF
{
    public float R;

    public float G;

    public float B;

    public float A;

    public ColorF(float r, float g, float b, float a = 1.0f)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
}