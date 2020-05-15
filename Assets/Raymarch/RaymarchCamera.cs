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
        int shapeType;

        Vector3 materialOptions;
        Color color;

        Vector3 position;
        Vector3 eulerAngles;
        Vector3 scale;

        Vector3 primitiveOptions;

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
            return 18 * 4;
        }
    }

    [SerializeField]
    Material raymarchMaterial;
    Mesh raymarchPlane;
    float raymarchPlaneDistance;

    List<IPrimitive> scenePrimitives;
    ComputeBuffer objectsBuffer;
    RaymarchObjectData[] objectsDataBuffer;

    /**
     * Get raymarching primitives from scene
     */
    List<IPrimitive> GetRaymarchPrimitives()
    {
        return FindObjectsOfType(typeof(MonoBehaviour)).Select(a =>
        {
            if (a is IPrimitive)
            {
                return a as IPrimitive;
            }

            return null;
        }).ToList();

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
        Debug.Log(scenePrimitives);

        // Create data buffer from raymarch objects properties
        objectsDataBuffer = scenePrimitives.Select<IPrimitive, RaymarchObjectData>(a =>
        {
            return new RaymarchObjectData(a);
        }).ToArray();
        objectsBuffer = new ComputeBuffer(objectsDataBuffer.Length, RaymarchObjectData.byteSize(), ComputeBufferType.Default);
    }

    void OnPostRender()
    {
        if (!raymarchPlane)
        {
            return;
        }

        raymarchMaterial.SetPass(0);
        Graphics.DrawMeshNow(raymarchPlane, transform.position + transform.forward * raymarchPlaneDistance, transform.rotation);
    }

}