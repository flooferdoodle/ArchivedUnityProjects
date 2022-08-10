using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class MarchingCubes : MonoBehaviour
{
    [Range(1f, 100f)]
    public int NumberOfSubdivisions = 2;
    float resolution;

    Vector3Int scaledDimensions;
    MarchingCubeVertex[] points;
    GameObject[] pointSpheres;

    [Range(0,50)]
    public int threshold = 10;

    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;

    /*
    private void OnValidate()
    {
        //GeneratePoints();
        GenerateMesh();
    }*/

    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<GameObject>().SetActive(false);
        //resolution = MinV3Dimension(GetComponent<DrawBorder>().dimensions) / NumberOfSubdivisions;
        resolution = /*transform.localScale.MinDimension() **/ GetComponent<DrawBorder>().dimensions.MinDimension() / NumberOfSubdivisions;

        scaledDimensions = Vector3Int.FloorToInt(GetComponent<DrawBorder>().dimensions / resolution) + Vector3Int.one;
        points = new MarchingCubeVertex[scaledDimensions.x * scaledDimensions.y * scaledDimensions.z];
        //Debug.Log(points.Length);

        pointSpheres = new GameObject[points.Length];


        ClickablePoint.mcScript = this;
        GeneratePoints();

        relativePos = new int[8] {0, 1, 1+scaledDimensions.x*scaledDimensions.y, scaledDimensions.x*scaledDimensions.y,
            scaledDimensions.x, 1+scaledDimensions.x, 1+scaledDimensions.x+scaledDimensions.x*scaledDimensions.y,scaledDimensions.x+scaledDimensions.x*scaledDimensions.y};


        //create mesh
        vertices = new List<Vector3>();
        triangles = new List<int>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh; //sets the mesh of this object to the code's mesh
        

        GenerateMesh();

        

    }

    int currCube = 0;
    // Update is called once per frame
    void Update()
    {
        //refresh mesh
        //resolution = MinV3Dimension(GetComponent<DrawBorder>().dimensions) / NumberOfSubdivisions;
        resolution = transform.localScale.MinDimension() * GetComponent<DrawBorder>().dimensions.MinDimension() / NumberOfSubdivisions;

        scaledDimensions = Vector3Int.FloorToInt(GetComponent<DrawBorder>().dimensions / resolution) + Vector3Int.one;
        //Debug.Log(scaledDimensions.ToString());
        relativePos = new int[8] {0, 1, 1+scaledDimensions.x*scaledDimensions.y, scaledDimensions.x*scaledDimensions.y,
            scaledDimensions.x, 1+scaledDimensions.x, 1+scaledDimensions.x+scaledDimensions.x*scaledDimensions.y,scaledDimensions.x+scaledDimensions.x*scaledDimensions.y};


        GeneratePoints();
        GenerateMesh();
        
        

        /*
         * One by one method to actually see it happening
         * 
        //draw current cube
        if (currCube < points.Length)
        {
            pointSpheres[currCube].GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            foreach (Vector2 currLine in edgeToCornerIndex)
            {
                Debug.DrawLine(pointSpheres[currCube + relativePos[(int)currLine.x]].transform.localPosition,
                               pointSpheres[currCube + relativePos[(int)currLine.y]].transform.localPosition);
            }
        }
        
        if (Input.GetKeyUp(KeyCode.Space) && currCube < points.Length)
        {
            //check each point/mesh one by one


            int currState = 0;
            //loop through 8 corners and find state
            for (int i = 0; i < 8; i++)
            {
                if (points[currCube + relativePos[i]] > threshold) //point is active
                {
                    currState += 1 << i;
                }
            }
            if (currState != 0) Debug.Log(currState);

            //get the vertex positions and add to mesh
            foreach (int i in TriTable[currState])
            {
                //quit if no more triangles
                if (i == -1) break;

                //get two corners from edge
                Vector3 a = pointSpheres[currCube + relativePos[edgeToCornerIndex[i].x]].transform.localPosition;
                Vector3 b = pointSpheres[currCube + relativePos[edgeToCornerIndex[i].y]].transform.localPosition;

                //add midpoint to vertices and update triangles
                vertices.Add((a + b) / 2);
                triangles.Add(triangles.Count);
            }

            mesh.Clear();
            //assign mesh with vertices and triangles
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            //calculate uvs and normals
            CalculateUV();
            mesh.RecalculateNormals();
            //create backface mesh to stop backface culling
            GenerateBackfaceMesh();

            //reset color
            pointSpheres[currCube].GetComponent<Renderer>().material.SetColor("_Color", Color.white);

            //increment and skip outside layers
            currCube++;
            
            //skip an x level
            if ((currCube + 1) % scaledDimensions.x == 0) currCube += 1;
            //skip a y level
            if ((currCube + 1) % (scaledDimensions.x * scaledDimensions.y) > scaledDimensions.x * (scaledDimensions.y - 1)) currCube += scaledDimensions.x;
            //skip a z level
            if ((currCube + 1) / (scaledDimensions.x * scaledDimensions.y) >= scaledDimensions.z - 1) currCube = points.Length;
            
        }*/
    }

    public void ClickedPoint(int i)
    {
        //update value
        if (points[i].value == 0) points[i].value = 50;
        else points[i].value = 0;

        //recolor
        float colorVal = Map(points[i].value, 0, 50, 0, 1);
        pointSpheres[i].GetComponent<Renderer>().material.SetColor("_Color", new Color(colorVal, colorVal, colorVal));

        //update mesh
        GenerateMesh();

    }

    void GeneratePoints()
    {
        points = new MarchingCubeVertex[(int) scaledDimensions.ProductDimensions()];
        //Debug.Log(scaledDimensions.ToString());
        //pointSpheres = new GameObject[points.Length];

        float xPos = 0, yPos = 0, zPos = 0;
        for (int i = 0; i < points.Length; i++)
        {
            //generate random point
            //Debug.Log(xPos + ", " + yPos + ", " + zPos + ": " + GetComponent<Perlin3D>().GenerateNum(xPos, yPos, zPos));
            points[i] = new MarchingCubeVertex(i, (int) Map(GetComponent<Perlin3D>().GenerateNum(xPos,yPos,zPos), 0, 1, 0, 50) , new Vector3(xPos, yPos, zPos) * resolution);
            //points[i] = 0;

            //create spheres and place inside of cube
            //pointSpheres[i] = CreatePointSphere(xPos, yPos, zPos, points[i].value, i);

            //increment position
            xPos++;
            if (xPos >= scaledDimensions.x)
            {
                xPos %= scaledDimensions.x;
                yPos++;
                if (yPos >= scaledDimensions.y)
                {
                    yPos %= scaledDimensions.y;
                    zPos++;
                }
            }
        }
    }

    

    GameObject CreatePointSphere(float x, float y, float z, int value, int index)
    {
        //create sphere, set location and scale
        GameObject output = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        output.transform.SetParent(transform);
        output.transform.localPosition = new Vector3(x, y, z) * resolution;
        output.transform.localScale = Vector3.one * 0.05f;

        //add clickable script
        output.AddComponent<ClickablePoint>();
        output.GetComponent<ClickablePoint>().pos = index;

        //change shader
        output.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Color");

        //color sphere based on value
        float colorVal = Map(value, 0, 50, 0, 1);
        output.GetComponent<Renderer>().material.SetColor("_Color", new Color(colorVal, colorVal, colorVal));

        return output;
    }

    int[] relativePos;
    Vector3[] relativeVertices = new Vector3[]
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

    void GenerateMesh()
    {
        if (mesh == null) mesh = new Mesh();

        //reset all values
        mesh.Clear();
        vertices = new List<Vector3>();
        triangles = new List<int>();

        int currState;

        //Debug.Log(points.Length);
        for(int pos = 0; pos < points.Length; pos++)
        {
            //skip an x level
            if ((pos + 1) % scaledDimensions.x == 0) pos += 1;
            //skip a y level
            if ((pos + 1) % (scaledDimensions.x * scaledDimensions.y) > scaledDimensions.x * (scaledDimensions.y - 1)) pos += scaledDimensions.x;
            //skip a z level
            if ((pos + 1) / (scaledDimensions.x * scaledDimensions.y) >= scaledDimensions.z - 1) break;

            currState = 0;
            //loop through 8 corners and find state
            for(int i = 0; i < 8; i++)
            {
                if(points[pos + relativePos[i]].value > threshold) //point is active
                {
                    currState += 1 << i;
                }
            }
            //if(currState != 0) Debug.Log(currState);

            //get the vertex positions and add to mesh
            foreach(int i in TriTable[currState])
            {
                //quit if no more triangles
                if (i == -1) break;

                //get two corners from edge
                Vector3 a = points[pos + relativePos[edgeToCornerIndex[i].x]].position;
                Vector3 b = points[pos + relativePos[edgeToCornerIndex[i].y]].position;

                //add midpoint to vertices and update triangles
                //vertices.Add((a + b) / 2);

                //Linearly interpolate the vertex
                float fraction = Map(threshold, points[pos + relativePos[edgeToCornerIndex[i].x]].value, points[pos + relativePos[edgeToCornerIndex[i].y]].value,
                                     0, 1);
                vertices.Add(Vector3.Lerp(a, b, fraction));


                triangles.Add(triangles.Count);
            }
            

        }

        //assign mesh with vertices and triangles
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        //calculate uvs and normals
        CalculateUV();
        mesh.RecalculateNormals();

        //add backface mesh
        GenerateBackfaceMesh();

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void CalculateUV()
    {
        Vector2[] uv = new Vector2[mesh.vertices.Length];
        for(int i = 0; i < uv.Length; i++)
        {
            uv[i] = new Vector2(vertices[i].x/(float)(transform.localScale.x * scaledDimensions.x), vertices[i].z / (float)(transform.localScale.z * scaledDimensions.z));
        }
        mesh.uv = uv;
    }

    void GenerateBackfaceMesh()
    {
        
        var vertices = mesh.vertices;
        var uv = mesh.uv;
        var normals = mesh.normals;
        var szV = vertices.Length;
        var newVerts = new Vector3[szV * 2];
        var newUv = new Vector2[szV * 2];
        var newNorms = new Vector3[szV * 2];
        for (var j = 0; j < szV; j++)
        {
            // duplicate vertices and uvs:
            newVerts[j] = newVerts[j + szV] = vertices[j];
            //Debug.Log("error: " + newUv.Length + " " + uv.Length + " " + j + " " + szV);
            newUv[j] = newUv[j + szV] = uv[j];
            // copy the original normals...
            newNorms[j] = normals[j];
            // and revert the new ones
            newNorms[j + szV] = -normals[j];
        }
        var triangles = mesh.triangles;
        var szT = triangles.Length;
        var newTris = new int[szT * 2]; // double the triangles
        for (var i = 0; i < szT; i += 3)
        {
            // copy the original triangle
            newTris[i] = triangles[i];
            newTris[i + 1] = triangles[i + 1];
            newTris[i + 2] = triangles[i + 2];
            // save the new reversed triangle
            var j = i + szT;
            newTris[j] = triangles[i] + szV;
            newTris[j + 2] = triangles[i + 1] + szV;
            newTris[j + 1] = triangles[i + 2] + szV;
        }
        mesh.vertices = newVerts;
        mesh.uv = newUv;
        mesh.normals = newNorms;
        mesh.triangles = newTris; // assign triangles last!
    }


    //map a number from one range to a new range
    public static float Map(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    int V3toPointIndex(Vector3 v)
    {
        return (int) (v.x + (v.y * scaledDimensions.x) + (v.z * scaledDimensions.x * scaledDimensions.y));
    }

    static readonly Vector2Int[] edgeToCornerIndex = new Vector2Int[] {
        new Vector2Int(0,1), //0 -> 0,1
        new Vector2Int(1,2),//1 -> 1,2
        new Vector2Int(2,3),//2 -> 2,3
        new Vector2Int(3,0),//3 -> 3,0
        new Vector2Int(4,5),//4 -> 4,5
        new Vector2Int(5,6),//5 -> 5,6
        new Vector2Int(6,7),//6 -> 6,7
        new Vector2Int(7,4),//7 -> 7,4
        new Vector2Int(0,4),//8 -> 0,4
        new Vector2Int(1,5),//9 -> 1,5
        new Vector2Int(2,6),//10 -> 2,6
        new Vector2Int(3,7) //11 -> 3,7
    };

    static readonly int[][] TriTable =
{new int[]{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1}, new int[]
{3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1}, new int[]
{3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1}, new int[]
{3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1}, new int[]
{2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1}, new int[]
{8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1}, new int[]
{4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1}, new int[]
{3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1}, new int[]
{4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1}, new int[]
{4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1}, new int[]
{5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1}, new int[]
{2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1}, new int[]
{9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1}, new int[]
{2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1}, new int[]
{10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1}, new int[]
{4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1}, new int[]
{5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1}, new int[]
{5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1}, new int[]
{10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1}, new int[]
{8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1}, new int[]
{2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1}, new int[]
{7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1}, new int[]
{2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1}, new int[]
{11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1}, new int[]
{5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1}, new int[]
{11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1}, new int[]
{11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1}, new int[]
{5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1}, new int[]
{2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1}, new int[]
{5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1}, new int[]
{6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1}, new int[]
{3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1}, new int[]
{6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1}, new int[]
{5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1}, new int[]
{10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1}, new int[]
{6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1}, new int[]
{8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1}, new int[]
{7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1}, new int[]
{3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1}, new int[]
{5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1}, new int[]
{0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1}, new int[]
{9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1}, new int[]
{8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1}, new int[]
{5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1}, new int[]
{0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1}, new int[]
{6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1}, new int[]
{10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1}, new int[]
{10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1}, new int[]
{8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1}, new int[]
{1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1}, new int[]
{3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1}, new int[]
{0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1}, new int[]
{10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1}, new int[]
{3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1}, new int[]
{6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1}, new int[]
{9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1}, new int[]
{8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1}, new int[]
{3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1}, new int[]
{6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1}, new int[]
{10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1}, new int[]
{10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1}, new int[]
{2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1}, new int[]
{7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1}, new int[]
{7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1}, new int[]
{2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1}, new int[]
{1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1}, new int[]
{11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1}, new int[]
{8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1}, new int[]
{0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1}, new int[]
{7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1}, new int[]
{10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1}, new int[]
{2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1}, new int[]
{6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1}, new int[]
{7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1}, new int[]
{2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1}, new int[]
{10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1}, new int[]
{10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1}, new int[]
{0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1}, new int[]
{7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1}, new int[]
{6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1}, new int[]
{8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1}, new int[]
{6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1}, new int[]
{4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1}, new int[]
{10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1}, new int[]
{8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1}, new int[]
{1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1}, new int[]
{8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1}, new int[]
{10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1}, new int[]
{4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1}, new int[]
{10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1}, new int[]
{5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1}, new int[]
{11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1}, new int[]
{9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1}, new int[]
{6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1}, new int[]
{7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1}, new int[]
{3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1}, new int[]
{7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1}, new int[]
{3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1}, new int[]
{6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1}, new int[]
{9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1}, new int[]
{1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1}, new int[]
{4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1}, new int[]
{7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1}, new int[]
{6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1}, new int[]
{3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1}, new int[]
{0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1}, new int[]
{6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1}, new int[]
{0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1}, new int[]
{11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1}, new int[]
{6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1}, new int[]
{5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1}, new int[]
{9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1}, new int[]
{1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1}, new int[]
{10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1}, new int[]
{0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1}, new int[]
{5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1}, new int[]
{10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1}, new int[]
{11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1}, new int[]
{9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1}, new int[]
{7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1}, new int[]
{2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1}, new int[]
{8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1}, new int[]
{9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1}, new int[]
{9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1}, new int[]
{1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1}, new int[]
{5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1}, new int[]
{0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1}, new int[]
{10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1}, new int[]
{2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1}, new int[]
{0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1}, new int[]
{0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1}, new int[]
{9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1}, new int[]
{5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1}, new int[]
{3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1}, new int[]
{5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1}, new int[]
{8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1}, new int[]
{9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1}, new int[]
{1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1}, new int[]
{3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1}, new int[]
{4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1}, new int[]
{9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1}, new int[]
{11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1}, new int[]
{11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1}, new int[]
{2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1}, new int[]
{9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1}, new int[]
{3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1}, new int[]
{1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1}, new int[]
{4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1}, new int[]
{4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1}, new int[]
{3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1}, new int[]
{3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1}, new int[]
{0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1}, new int[]
{9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1}, new int[]
{1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, new int[]
{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

}