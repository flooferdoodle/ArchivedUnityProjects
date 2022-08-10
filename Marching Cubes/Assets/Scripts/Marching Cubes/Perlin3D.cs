using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perlin3D : MonoBehaviour
{
    public float xoffset = 0;
    public float yoffset = 0;
    public float zoffset = 0;
    [Range(0.0001f, 0.5f)]
    public float scale = 0.1f;

    public float GenerateNum(float x, float y, float z)
    {
        x += xoffset;
        y += yoffset;
        z += zoffset;

        x *= scale;
        y *= scale;
        z *= scale;

        float AB = Mathf.PerlinNoise(x, y);
        float BC = Mathf.PerlinNoise(y, z);
        float AC = Mathf.PerlinNoise(x, z);

        float BA = Mathf.PerlinNoise(y, x);
        float CB = Mathf.PerlinNoise(z, y);
        float CA = Mathf.PerlinNoise(z, x);

        //Debug.Log(x + ", " + y + ", " + z + ": " + AB + " " + BC);

        float ABC = AB + BC + AC + BA + CB + CA;
        return ABC / 6f;
    }
}
