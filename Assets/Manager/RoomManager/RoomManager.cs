using UnityEngine;
using System.Collections.Generic;

public struct RoomObject
{
    public string Name;
    public string Tag;
    public int Layer;

    public RoomObject(string name, string tag, int layer)
    {
        Name = name;
        Tag = tag;
        Layer = layer;
    }

    // Check if the given GameObject matches the criteria
    public bool Matches(GameObject obj)
    {
        if (!string.IsNullOrEmpty(Name) && obj.name != Name)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(Tag) && obj.tag != Tag)
        {
            return false;
        }

        if (Layer >= 0 && obj.layer != Layer)
        {
            return false;
        }

        return true;
    }
}

public class RoomManager : Singleton<RoomManager>
{
    private float radius = 10f;
    private float wallThickness = 0.1f;
    private float wallHeight = 4f;
    private Vector3 position = new Vector3(0, 0, 0);

    private const bool DEBUG_LOG = false;

    private List<RoomData> rooms = new List<RoomData>(); // List to hold RoomData

    public GameObject CreateRoom(
        string roomName,
        int numberOfSides,
        Vector3 position,
        float radius,
        float wallThickness,
        float wallHeight
    )
    {
        this.radius = radius;
        this.wallThickness = wallThickness;
        this.wallHeight = wallHeight;

        GameObject room = new GameObject(roomName);
        room.tag = "Room";
        room.AddComponent<MeshCollider>();

        Transform parentTransform = room.transform;
        parentTransform.position = position;

        RoomData roomData = new RoomData
        {
            room = room,
            floor = CreateFlatPolygon(parentTransform, numberOfSides),
            walls = CreateWalls(parentTransform, numberOfSides)
        };

        // Add new RoomData to the list
        rooms.Add(roomData);

        return room;
    }

    GameObject CreateFlatPolygon(Transform parent, int numberOfSides)
    {
        float planeHeight = 0.1f;

        int roomLayer = LayerMask.NameToLayer("RoomLayer");
        if (roomLayer == -1)
        {
            LogError(
                "Layer 'RoomLayer' does not exist. Please add it in the Tags and Layers settings."
            );
            return null;
        }

        GameObject polygon = new GameObject("Floor");
        polygon.tag = "Floor";
        polygon.layer = roomLayer;
        polygon.transform.SetParent(parent);

        MeshFilter meshFilter = polygon.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = polygon.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = polygon.AddComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        polygon.transform.localPosition = new Vector3(0f, -planeHeight / 2f, 0f);

        Vector3[] vertices = new Vector3[numberOfSides + 1];
        int[] triangles = new int[numberOfSides * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < numberOfSides; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfSides;
            vertices[i + 1] = new Vector3(radius * Mathf.Cos(angle), 0f, radius * Mathf.Sin(angle));
        }

        for (int i = 0; i < numberOfSides; i++)
        {
            int next = (i + 1) % numberOfSides;
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = next + 1;
            triangles[i * 3 + 2] = i + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

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

        meshRenderer.material.color = Color.gray;

        meshCollider.sharedMesh = mesh;

        return polygon;
    }

    GameObject[] CreateWalls(Transform parent, int numberOfSides)
    {
        int roomLayer = LayerMask.NameToLayer("RoomLayer");
        if (roomLayer == -1)
        {
            Debug.LogError(
                "Layer 'RoomLayer' does not exist. Please add it in the Tags and Layers settings."
            );
            return null; // Return null if the layer does not exist
        }

        if (numberOfSides < 3)
        {
            Debug.LogError("Number of sides must be at least 3 to form a closed room.");
            return null; // Return null if the number of sides is invalid
        }

        float angleStep = 360f / numberOfSides;
        GameObject[] walls = new GameObject[numberOfSides];

        for (int i = 0; i < numberOfSides; i++)
        {
            float angle1 = i * Mathf.PI * 2f / numberOfSides;
            float angle2 = (i + 1) * Mathf.PI * 2f / numberOfSides;

            Vector3 start = new Vector3(radius * Mathf.Cos(angle1), 0f, radius * Mathf.Sin(angle1));
            Vector3 end = new Vector3(radius * Mathf.Cos(angle2), 0f, radius * Mathf.Sin(angle2));

            walls[i] = CreateWall(
                parent,
                $"Wall{i + 1}",
                start,
                end,
                wallHeight,
                wallThickness,
                roomLayer
            );
        }

        return walls;
    }

    GameObject CreateWall(
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
        wall.name = name;
        wall.tag = "Wall";
        wall.layer = layer;
        wall.transform.SetParent(parent);

        Vector3 wallCenter = (start + end) / 2f;
        wall.transform.localPosition = wallCenter;
        wall.transform.localScale = new Vector3(thickness, height, Vector3.Distance(start, end));

        wall.transform.rotation = Quaternion.LookRotation(end - start);

        Renderer wallRenderer = wall.GetComponent<Renderer>();
        if (wallRenderer != null)
        {
            wallRenderer.material.color = Color.gray;
        }
        return wall;
    }

    private bool IsPointInPolygon(Vector3 point, Vector3[] vertices)
    {
        int n = vertices.Length;
        bool inside = false;

        // Project 3D points to 2D plane (XY plane in this case)
        Vector2 pt = new Vector2(point.x, point.z);

        Vector2[] poly = new Vector2[n];
        for (int i = 0; i < n; i++)
        {
            poly[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            Vector2 vi = poly[i];
            Vector2 vj = poly[j];
            if (
                (vi.y > pt.y) != (vj.y > pt.y)
                && (pt.x < (vj.x - vi.x) * (pt.y - vi.y) / (vj.y - vi.y) + vi.x)
            )
            {
                inside = !inside;
            }
        }
        return inside;
    }

    // Function to find the room containing a specific point, considering wall height
    public RoomData FindRoomContainingPoint(Vector3 point)
    {
        foreach (RoomData roomData in rooms)
        {
            GameObject floor = roomData.floor;
            if (floor != null)
            {
                MeshFilter meshFilter = floor.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    Mesh mesh = meshFilter.mesh;
                    Vector3[] vertices = mesh.vertices;
                    Vector3[] worldVertices = new Vector3[vertices.Length];
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        worldVertices[i] = floor.transform.TransformPoint(vertices[i]);
                    }

                    // Check if the point is within the polygon of the floor
                    if (IsPointInPolygon(point, worldVertices))
                    {
                        // Check if the point's y-coordinate is within the room's height range
                        float floorY = floor.transform.position.y;
                        if (point.y >= floorY && point.y <= floorY + wallHeight)
                        {
                            return roomData;
                        }
                    }
                }
            }
        }

        Debug.Log("No room found containing the point.");
        return null;
    }

    // Function to find a GameObject by name within the entire scene
    public GameObject FindGameObjectByName(string name)
    {
        return GameObject.Find(name);
    }

    private void Log(string message)
    {
        if (DEBUG_LOG)
        {
            Debug.Log(message);
        }
    }

    private void LogError(string message)
    {
        if (DEBUG_LOG)
        {
            Debug.LogError(message);
        }
    }
}
