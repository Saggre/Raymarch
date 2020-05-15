using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PrimitiveShapeType
{
    sphere,
}

public interface IPrimitive
{
    Vector3 GetPrimitiveOptions();
    Color GetColor();
    Vector3 GetMaterialOptions();
    PrimitiveShapeType GetShapeType();
    Vector3 GetPosition();

    Vector3 GetEulerAngles();
    Vector3 GetScale();

}
