using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class testCube : MonoBehaviour
{
    void Start()
    {
        GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.position = new Vector3(10, 20, 30);
    }
}
