using UnityEngine;

public class CreateRoom : MonoBehaviour
{
    float planeSize = 20f; // Size of the plane

    void Start()
    {
        // Use the GameObject this script is attached to as the parent
        Transform parentTransform = transform;

        CreateFlatPlane(parentTransform); // Create a flat plane using the parentTransform
        CreateWalls(parentTransform); // Create walls using the parentTransform
    }

    // Function to create a flat gray plane
    void CreateFlatPlane(Transform parent)
    {
        float planeHeight = 0.1f; // Height of the plane

        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Floor"; // Set a name for the plane
        plane.transform.position = new Vector3(0f, -0.05f, 0f); // Set the position of the plane slightly below the center
        plane.transform.localScale = new Vector3(planeSize, planeHeight, planeSize); // Set the scale of the plane
        plane.GetComponent<Renderer>().material.color = Color.gray; // Set the color of the plane to gray

        // Add BoxCollider to the plane
        BoxCollider planeCollider = plane.AddComponent<BoxCollider>();
        planeCollider.size = new Vector3(planeSize, planeHeight, planeSize); // Adjust collider size to match plane's scale

        // Set the parent GameObject
        plane.transform.SetParent(parent);
    }

    // Function to create four gray walls around the plane
    void CreateWalls(Transform parent)
    {
        float wallHeight = 4f; // Height of the walls

        // Get the position and scale of the plane
        Vector3 planePosition = Vector3.zero; // Center of the plane

        // Calculate wall positions based on the plane's position and scale
        float halfWidth = planeSize / 2f;
        float halfLength = planeSize / 2f;

        // Wall 1 (left)
        GameObject wall1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall1.name = "Wall1"; // Set a name for the wall
        wall1.transform.position = planePosition + new Vector3(-halfWidth, wallHeight / 2f, 0f);
        wall1.transform.localScale = new Vector3(1f, wallHeight, planeSize);
        wall1.GetComponent<Renderer>().material.color = Color.gray;

        // Add BoxCollider to Wall 1
        BoxCollider wall1Collider = wall1.AddComponent<BoxCollider>();
        wall1Collider.size = new Vector3(1f, wallHeight, planeSize);

        // Set the parent GameObject
        wall1.transform.SetParent(parent);

        // Wall 2 (right)
        GameObject wall2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall2.name = "Wall2"; // Set a name for the wall
        wall2.transform.position = planePosition + new Vector3(halfWidth, wallHeight / 2f, 0f);
        wall2.transform.localScale = new Vector3(1f, wallHeight, planeSize);
        wall2.GetComponent<Renderer>().material.color = Color.gray;

        // Add BoxCollider to Wall 2
        BoxCollider wall2Collider = wall2.AddComponent<BoxCollider>();
        wall2Collider.size = new Vector3(1f, wallHeight, planeSize);

        // Set the parent GameObject
        wall2.transform.SetParent(parent);

        // Wall 3 (top)
        GameObject wall3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall3.name = "Wall3"; // Set a name for the wall
        wall3.transform.position = planePosition + new Vector3(0f, wallHeight / 2f, halfLength);
        wall3.transform.localScale = new Vector3(planeSize, wallHeight, 1f);
        wall3.GetComponent<Renderer>().material.color = Color.gray;

        // Add BoxCollider to Wall 3
        BoxCollider wall3Collider = wall3.AddComponent<BoxCollider>();
        wall3Collider.size = new Vector3(planeSize, wallHeight, 1f);

        // Set the parent GameObject
        wall3.transform.SetParent(parent);

        // Wall 4 (bottom)
        GameObject wall4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall4.name = "Wall4"; // Set a name for the wall
        wall4.transform.position = planePosition + new Vector3(0f, wallHeight / 2f, -halfLength);
        wall4.transform.localScale = new Vector3(planeSize, wallHeight, 1f);
        wall4.GetComponent<Renderer>().material.color = Color.gray;

        // Add BoxCollider to Wall 4
        BoxCollider wall4Collider = wall4.AddComponent<BoxCollider>();
        wall4Collider.size = new Vector3(planeSize, wallHeight, 1f);

        // Set the parent GameObject
        wall4.transform.SetParent(parent);
    }
}
