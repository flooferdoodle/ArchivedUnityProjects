using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBorder : MonoBehaviour
{
    Vector3[] vertices;
    static int[] pairs;

    public Vector3 dimensions = Vector3.one;

    // Start is called before the first frame update
    void Start()
    {

        vertices = new Vector3[]
        {
            new Vector3(0,0,0),//0
            new Vector3(0,0,1),//1
            new Vector3(0,1,0),//2
            new Vector3(1,0,0),//3
            new Vector3(0,1,1),//4
            new Vector3(1,1,1),//5
            new Vector3(1,1,0),//6
            new Vector3(1,0,1) //7

        };

        pairs = new int[]
        {
            0,1,
            0,2,
            0,3,
            1,4,
            1,7,
            2,4,
            2,6,
            3,6,
            3,7,
            4,5,
            5,6,
            5,7
        };
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < pairs.Length; i += 2)
        {
            Debug.DrawLine(Vector3.Scale(vertices[pairs[i]], Vector3.Scale(dimensions, transform.localScale)), Vector3.Scale(vertices[pairs[i + 1]], Vector3.Scale(dimensions, transform.localScale)));
        }


    }
}
