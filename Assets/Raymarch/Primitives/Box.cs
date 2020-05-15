using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Box : Primitive, IPrimitive
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
    float size = 1.0f;

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
        return new Vector3(size, 0.0f, 0.0f);
    }

    public override PrimitiveShapeType GetShapeType()
    {
        return PrimitiveShapeType.box;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
