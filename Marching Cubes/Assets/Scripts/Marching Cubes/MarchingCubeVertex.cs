using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MarchingCubeVertex
{
    public int index;
    public int value;
    public Vector3 position;

    public MarchingCubeVertex(int i, int v, Vector3 p)
    {
        index = i;
        value = v;
        position = p;
    }

}
