using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System;
using Unity.VisualScripting;

public class MMTest : MonoBehaviour
{
    public Mesh mesh;
    public Material material;

    [Header("Draw Mesh Active State")]
    public bool isMeshMoving = false;
    public bool isDrawMeshActive = false;
    public bool isDrawMeshInstActive = false;
    public bool isDrawMeshInstIndActive = false;

    [Header("Draw Mesh Position")]
    public Vector3 drawMeshPos;
    public Vector3 drawMeshInstPos;
    public Vector3 drawMeshInstIndPos;

    [Header("Pool Info")]
    public int poolSize = 100;
    private int prevPoolSize;
    //public NativeArray<Vector3> Positions;
    //public NativeArray<float> Rotations;
    //public NativeArray<float> Speeds;
    //public NativeArray<float> AngularSpeeds;
    //public NativeArray<int> RotationDirections;
    MaterialPropertyBlock blockProp;    //used to change the color material used in DrawMesh per instance
    Matrix4x4[] instanceMatrices;       //used for DrawMeshInstance
    ComputeBuffer argsBuffer;           //used for DrawMeshInstanceIndirect
    ComputeBuffer matricesBuffer;
    Bounds bound;
    Matrix4x4[] indirectMatrices;


    [Header("Sine Wave")]
    public float meshSpacing = 0.5f;
    public float amplitude = 2.5f;
    public float frequency = 0.5f;
    public float speed = 1;

    private void OnEnable()
    {
        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);//CommputeBuffer values are non-specific at the moment
        argsBuffer.SetData(new uint[]
        {
            (uint)mesh.GetIndexCount(0), // triangle indices count per instance
            (uint)poolSize, // instance count
            (uint)mesh.GetIndexStart(0), // start index location
            (uint)mesh.GetBaseVertex(0), // base vertex location
            0, // start instance location
        });
    }

    void Start()
    {
        instanceMatrices = new Matrix4x4[poolSize];
        indirectMatrices = new Matrix4x4[poolSize];
        blockProp = new MaterialPropertyBlock();
        bound = new Bounds(Vector3.zero, new Vector3(25f, 25f, 25f));
    }

    void Update()
    {
        if(prevPoolSize != poolSize)
        {
            instanceMatrices = new Matrix4x4[poolSize];
            indirectMatrices = new Matrix4x4[poolSize];
        }
        CheckInput();
        CheckDrawMeshState();
        prevPoolSize = poolSize;
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isMeshMoving = !isMeshMoving;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            isDrawMeshActive = !isDrawMeshActive;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            isDrawMeshInstActive = !isDrawMeshInstActive;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            isDrawMeshInstIndActive = !isDrawMeshInstIndActive;
        }
    }

    void CheckDrawMeshState()
    {
        if (isDrawMeshActive)
        {
            ExampleDrawMesh();
        }
        if (isDrawMeshInstActive)
        {
            ExampleDrawMeshInstanced();
        }
        if (isDrawMeshInstIndActive)
        {
            ExampleDrawMeshInstancedIndirect();
        }
    }

    Vector3 CalculateSinePosition(int i, Vector3 wavePos)
    {
        float startPosX = wavePos.x - (meshSpacing * poolSize / 2f);
        float meshPosX = startPosX + (i * meshSpacing);
        float sineAngle = isMeshMoving ? Time.time + (i * frequency) : i * frequency;
        float meshPosY = wavePos.y + (amplitude * Mathf.Sin(sineAngle));
        return new Vector3(meshPosX, meshPosY, wavePos.z);
    }

    void ExampleDrawMesh()
    {
        blockProp.SetColor("_Color", Color.red);
        //material.SetColor("_Color", Color.red);
        for (int i = 0; i < poolSize; i++)
        {
            Vector3 meshPos = CalculateSinePosition(i, drawMeshPos);
            Graphics.DrawMesh(mesh, meshPos, Quaternion.identity, material, 0, null, 0, blockProp);
        }
    }

    void ExampleDrawMeshInstanced()
    {
        blockProp.SetColor("_Color", Color.cyan);
        //material.SetColor("_Color", Color.cyan);
        for (int i = 0; i < poolSize; i++)
        {
            Vector3 meshPos = CalculateSinePosition(i, drawMeshInstPos);
            instanceMatrices[i] = Matrix4x4.TRS(meshPos, Quaternion.identity, new Vector3(1f, 1f, 1f));
        }
        Graphics.DrawMeshInstanced(mesh, 0, material, instanceMatrices, poolSize, blockProp);
        //Debug.Log(string.Join(" || ", instanceMatrices));
    }

    void ExampleDrawMeshInstancedIndirect()
    {
        blockProp.SetColor("_Color", Color.green);
        //material.SetColor("_Color", Color.green);
        matricesBuffer?.Release();
        for (int i = 0; i < poolSize; i++)
        {
            Vector3 meshPos = CalculateSinePosition(i, drawMeshInstIndPos);
            indirectMatrices[i] = Matrix4x4.TRS(meshPos, Quaternion.identity, new Vector3(1f, 1f, 1f));
        }
        matricesBuffer = new ComputeBuffer(poolSize, sizeof(float) * 4 * 4);
        matricesBuffer.SetData(indirectMatrices);
        material.SetBuffer("matricesBuffer", matricesBuffer);
        Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bound, argsBuffer, 0, blockProp);
        Debug.Log(string.Join(" || ", indirectMatrices));
    }

    private void OnDisable()
    {
        matricesBuffer?.Release();
        matricesBuffer = null;
        argsBuffer?.Release();
        argsBuffer = null;
    }

    private void OnDrawGizmos()
    {
        float halfLength = meshSpacing * poolSize / 2f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(drawMeshPos, 1);
        Gizmos.DrawLine(drawMeshPos - new Vector3(halfLength, 0f, 0f), drawMeshPos + new Vector3(halfLength, 0f, 0f));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(drawMeshInstPos, 1);
        Gizmos.DrawLine(drawMeshInstPos - new Vector3(halfLength, 0f, 0f), drawMeshInstPos + new Vector3(halfLength, 0f, 0f));
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(drawMeshInstIndPos, 1);
        Gizmos.DrawLine(drawMeshInstIndPos - new Vector3(halfLength, 0f, 0f), drawMeshInstIndPos + new Vector3(halfLength, 0f, 0f));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(25f, 25f, 25f));

    }

}
