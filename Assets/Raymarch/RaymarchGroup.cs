using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public enum RaymarchGroupOperation
{
    intersection,
    union,
    subtraction,
}

public class RaymarchGroup : MonoBehaviour
{

    [SerializeField]
    private RaymarchGroupOperation operation = RaymarchGroupOperation.union;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
