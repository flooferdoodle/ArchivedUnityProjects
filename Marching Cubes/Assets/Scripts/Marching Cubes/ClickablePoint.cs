using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickablePoint : MonoBehaviour
{

    public int pos;

    public static MarchingCubes mcScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        mcScript.ClickedPoint(pos);
    }
}
