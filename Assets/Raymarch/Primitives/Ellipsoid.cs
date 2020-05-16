
using UnityEngine;
public class Ellipsoid : Primitive, IPrimitive
{
    [SerializeField]
    Color color = Color.white;

    [Range(0, 100)]
    [SerializeField]
    float reflective = 0.1f;

    [Range(0, 100)]
    [SerializeField]
    float shininess = 0.5f;

    [Range(0, 100)]
    [SerializeField]
    float ellipseA = 1.0f;

    [Range(0, 100)]
    [SerializeField]
    float ellipseB = 1.0f;

    [Range(0, 100)]
    [SerializeField]
    float ellipseC = 1.0f;

    public override Color GetColor()
    {
        return color;
    }

    public override Vector3 GetMaterialOptions()
    {
        return new Vector3(reflective, shininess, 0.0f);
    }

    public override Vector3 GetPrimitiveOptions()
    {
        return new Vector3(ellipseA, ellipseB, ellipseC);
    }

    public override PrimitiveShapeType GetShapeType()
    {
        return PrimitiveShapeType.ellipsoid;
    }

}
