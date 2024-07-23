using UnityEngine;

public class Pedestal : MonoBehaviour
{
    void Start()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube); // Create a cube GameObject
        cube.transform.position = transform.position; // Set the cube's position to the same as this GameObject
    }
}
