using UnityEngine;

public class RoomManager : Singleton<RoomManager>
{
    public float radius = 10f; // Radius of the polygon
    public float wallThickness = 0.1f; // Thickness of the walls
    public float wallHeight = 4f; // Height of the walls

    // Debug log control macro
    private const bool DEBUG_LOG = false;

    public void CreateRoom(string roomName, int numberOfSides)
    {
        // Create a parent GameObject for the room
        GameObject room = new GameObject(roomName);
        room.tag = "Room"; // Set tag for the room

        // Use the Room GameObject as the parent
        Transform parentTransform = room.transform;

        CreateFlatPolygon(parentTransform, numberOfSides); // Create a flat polygon floor using the parentTransform
        CreateWalls(parentTransform, numberOfSides); // Create walls using the parentTransform
    }

    // Function to create a flat polygon floor
    void CreateFlatPolygon(Transform parent, int numberOfSides)
    {
        float planeHeight = 0.1f; // Height of the plane

        // Check if the layer exists
        int roomLayer = LayerMask.NameToLayer("RoomLayer");
        if (roomLayer == -1)
        {
            LogError(
                "Layer 'RoomLayer' does not exist. Please add it in the Tags and Layers settings."
            );
            return;
        }

        // Create a new GameObject for the polygon
        GameObject polygon = new GameObject("Floor");
        polygon.tag = "Floor"; // Set tag for the plane
        polygon.layer = roomLayer; // Set the layer to RoomLayer

        // Add a MeshFilter and MeshRenderer to the polygon
        MeshFilter meshFilter = polygon.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = polygon.AddComponent<MeshRenderer>();

        // Add a MeshCollider to the polygon
        MeshCollider meshCollider = polygon.AddComponent<MeshCollider>();

        // Generate the mesh for the polygon
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        polygon.transform.position = new Vector3(0f, -planeHeight / 2f, 0f);

        Vector3[] vertices = new Vector3[numberOfSides + 1];
        int[] triangles = new int[numberOfSides * 3];

        // Center vertex
        vertices[0] = Vector3.zero;

        // Calculate the vertices for the perimeter
        for (int i = 0; i < numberOfSides; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfSides;
            vertices[i + 1] = new Vector3(radius * Mathf.Cos(angle), 0f, radius * Mathf.Sin(angle));
        }

        // Create triangles (right-handed winding order)
        for (int i = 0; i < numberOfSides; i++)
        {
            int next = (i + 1) % numberOfSides;
            triangles[i * 3] = 0; // Center vertex
            triangles[i * 3 + 1] = next + 1; // Next vertex
            triangles[i * 3 + 2] = i + 1; // Current vertex
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // Recalculate normals for proper lighting
        mesh.RecalculateBounds(); // Recalculate the bounding box of the mesh

        // Log vertex and triangle information
        Log("Vertices:");
        for (int i = 0; i < vertices.Length; i++)
        {
            Log($"Vertex {i}: {vertices[i]}");
        }

        Log("Triangles:");
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Log($"Triangle: {triangles[i]}, {triangles[i + 1]}, {triangles[i + 2]}");
        }

        // Set the material color of the floor to gray
        meshRenderer.material.color = Color.gray;

        // Set the parent GameObject
        polygon.transform.SetParent(parent);

        // Set the mesh to the MeshCollider
        meshCollider.sharedMesh = mesh;
    }

    // Function to create walls around the polygon
    void CreateWalls(Transform parent, int numberOfSides)
    {
        // Check if the layer exists
        int roomLayer = LayerMask.NameToLayer("RoomLayer");
        if (roomLayer == -1)
        {
            LogError(
                "Layer 'RoomLayer' does not exist. Please add it in the Tags and Layers settings."
            );
            return;
        }

        float angleStep = 360f / numberOfSides;
        float halfWidth = wallThickness / 2f;

        // Create walls
        for (int i = 0; i < numberOfSides; i++)
        {
            float angle1 = i * Mathf.PI * 2f / numberOfSides;
            float angle2 = (i + 1) * Mathf.PI * 2f / numberOfSides;

            Vector3 start = new Vector3(radius * Mathf.Cos(angle1), 0f, radius * Mathf.Sin(angle1));
            Vector3 end = new Vector3(radius * Mathf.Cos(angle2), 0f, radius * Mathf.Sin(angle2));

            CreateWall(parent, $"Wall{i + 1}", start, end, wallHeight, wallThickness, roomLayer);
        }
    }

    // Function to create a wall between two points
    void CreateWall(
        Transform parent,
        string name,
        Vector3 start,
        Vector3 end,
        float height,
        float thickness,
        int layer
    )
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name; // Set a name for the wall
        wall.tag = "Wall"; // Set tag for the wall
        wall.layer = layer; // Set the layer to RoomLayer

        Vector3 wallCenter = (start + end) / 2f;
        wall.transform.position = wallCenter;
        wall.transform.localScale = new Vector3(thickness, height, Vector3.Distance(start, end));

        // Rotate wall to align with the edge
        wall.transform.rotation = Quaternion.LookRotation(end - start);

        // Set the material color of the wall to gray
        Renderer wallRenderer = wall.GetComponent<Renderer>();
        if (wallRenderer != null)
        {
            wallRenderer.material.color = Color.gray; // Set the color of the wall to gray
        }

        // Set the parent GameObject
        wall.transform.SetParent(parent);
    }

    // Log message function based on the DEBUG_LOG flag
    private void Log(string message)
    {
        if (DEBUG_LOG)
        {
            Debug.Log(message);
        }
    }

    // Log error message function based on the DEBUG_LOG flag
    private void LogError(string message)
    {
        if (DEBUG_LOG)
        {
            Debug.LogError(message);
        }
    }
}
