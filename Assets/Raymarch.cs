using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class Raymarch : MonoBehaviour
{

    void Start()
    {
        if (Application.isEditor)
        {

        }
        else
        {

        }
    }

    void Update()
    {
      
    }

    void OnPostRender()
    {

    }
}