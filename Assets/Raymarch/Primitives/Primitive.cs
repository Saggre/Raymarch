
using UnityEngine;

public abstract class Primitive : MonoBehaviour, IPrimitive
{
    public Vector3 GetEulerAngles()
    {
        return transform.eulerAngles;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Vector3 GetScale()
    {
        return transform.localScale;
    }

    public abstract Color GetColor();

    public abstract Vector3 GetMaterialOptions();

    public abstract Vector3 GetPrimitiveOptions();

    public abstract PrimitiveShapeType GetShapeType();
}