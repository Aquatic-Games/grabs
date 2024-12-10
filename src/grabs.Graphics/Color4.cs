using System.Drawing;

namespace grabs.Graphics;

public struct Color4
{
    public float R;

    public float G;

    public float B;

    public float A;

    public Color4(float r, float g, float b, float a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public static explicit operator Color4(in Color color)
        => new Color4(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
}