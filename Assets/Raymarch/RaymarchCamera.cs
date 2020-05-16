using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class RaymarchCamera : MonoBehaviour
{

    /**
     * The type of objects sent to shader
     */
    struct RaymarchObjectData
    {
        public int shapeType;

        public Vector3 materialOptions;
        public Color color;

        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 scale;

        public Vector3 primitiveOptions;

        public RaymarchObjectData(IPrimitive primitive)
        {
            shapeType = (int)primitive.GetShapeType();
            materialOptions = primitive.GetMaterialOptions();
            color = primitive.GetColor();
            position = primitive.GetPosition();
            eulerAngles = primitive.GetEulerAngles();
            scale = primitive.GetEulerAngles();
            primitiveOptions = primitive.GetPrimitiveOptions();
        }

        public static int byteSize()
        {
            return 20 * 4;
        }
    }

    [SerializeField]
    Material raymarchMaterial;
    Mesh raymarchPlane;
    float raymarchPlaneDistance;

    Primitive[] scenePrimitives;
    ComputeBuffer objectsBuffer;
    RaymarchObjectData[] objectsDataBuffer;

    /**
     * Get raymarching primitives from scene
     */
    Primitive[] GetRaymarchPrimitives()
    {
        return FindObjectsOfType(typeof(Primitive)) as Primitive[];
    }

    /**
     * Create plane mesh
     */
    Mesh CreatePlane(float width, float height)
    {
        Mesh m = new Mesh();
        m.name = "RaymarchRenderPlane";
        m.vertices = new Vector3[] {
            new Vector3(-width * 0.5f, -height * 0.5f, 0.0f),
            new Vector3(width * 0.5f, -height * 0.5f, 0.0f),
            new Vector3(width * 0.5f, height * 0.5f, 0.0f),
            new Vector3(-width * 0.5f, height * 0.5f, 0.0f)
        };

        m.uv = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0)
        };

        m.triangles = new int[] { 1, 0, 2, 2, 0, 3 };
        return m;
    }

    void Start()
    {
        Camera camera = GetComponent<Camera>();

        // Create plane to render raymarch world on
        raymarchPlaneDistance = camera.nearClipPlane + 0.01f;
        Vector3 cameraExtremumA = camera.ViewportToWorldPoint(new Vector3(0, 0, raymarchPlaneDistance));
        Vector3 cameraExtremumB = camera.ViewportToWorldPoint(new Vector3(1, 1, raymarchPlaneDistance));

        raymarchPlane = CreatePlane(Mathf.Abs(cameraExtremumA.x - cameraExtremumB.x), Mathf.Abs(cameraExtremumA.y - cameraExtremumB.y));

        // Get raymarched objects in scene
        scenePrimitives = GetRaymarchPrimitives();
        objectsBuffer = new ComputeBuffer(scenePrimitives.Length, RaymarchObjectData.byteSize());

    }

    void Update()
    {

    }

    void OnPostRender()
    {
        if (!raymarchPlane || objectsBuffer == null)
        {
            return;
        }

        objectsDataBuffer = scenePrimitives.Select<IPrimitive, RaymarchObjectData>(a =>
        {
            return new RaymarchObjectData(a);
        }).ToArray();
        objectsBuffer.SetData(objectsDataBuffer);

        Graphics.ClearRandomWriteTargets();
        raymarchMaterial.SetPass(0);
        raymarchMaterial.SetBuffer("raymarchObjectData", objectsBuffer);
        Graphics.SetRandomWriteTarget(1, objectsBuffer);
        Graphics.DrawMeshNow(raymarchPlane, transform.position + transform.forward * raymarchPlaneDistance, transform.rotation);
    }

    /**
     * Cleanup
     */
    void OnDestroy()
    {
        if (objectsBuffer != null)
        {
            objectsBuffer.Dispose();
        }
    }

}