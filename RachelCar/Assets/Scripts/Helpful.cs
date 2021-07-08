using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpful
{
    public static float FastInverseSquareRoot(float f)
    {
        float x2 = f / 2;
        
        int i = System.BitConverter.ToInt32(System.BitConverter.GetBytes(f), 0);

        i = 0x5f3759df - (i >> 1);
        //System.BitConverter.
        f = System.BitConverter.ToSingle(System.BitConverter.GetBytes(i), 0);
        //f = System.BitConverter.Int32BitsToSingle(i);//Why doesn't this work?: Also: System.BitConverter.SingleToInt32Bits(f)
        f = (f * (1.5f - (f*f*x2)));

        return f;
    }

    public static void SetRandPosInCircle(GameObject go, Vector3 center, float radius)
    {
        Vector2 randChange = Random.insideUnitCircle * radius;
        go.transform.localPosition = center + new Vector3(randChange.x, 0f, randChange.y);
    }
    public static float Hash128ToFloat(Hash128 h)
    {
        string hexString = h.ToString();
        uint[] hashParts = {
            uint.Parse(hexString.Substring(0, 8), System.Globalization.NumberStyles.AllowHexSpecifier),
            uint.Parse(hexString.Substring(8, 8), System.Globalization.NumberStyles.AllowHexSpecifier),
            uint.Parse(hexString.Substring(16, 8), System.Globalization.NumberStyles.AllowHexSpecifier),
            uint.Parse(hexString.Substring(24, 8), System.Globalization.NumberStyles.AllowHexSpecifier) };
        byte[] floatVals = System.BitConverter.GetBytes(hashParts[0] ^ hashParts[1] ^ hashParts[2] ^ hashParts[3]);
        return System.BitConverter.ToSingle(floatVals, 0);
    }
    public static float Sum(float[] far)
    {
        float sum = 0f;
        foreach (float f in far)
        {
            sum += f;
        }
        return sum;
    }
    
}
