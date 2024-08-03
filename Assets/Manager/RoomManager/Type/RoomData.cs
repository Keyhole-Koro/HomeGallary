using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RoomData
{
    public GameObject room;
    public GameObject floor;
    public GameObject[] walls;
    public GameObject roof;

    public Vector3 GetFloorCenter()
    {
        if (room != null)
        {
            if (floor != null)
            {
                MeshFilter meshFilter = floor.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    Mesh mesh = meshFilter.mesh;
                    Vector3[] vertices = mesh.vertices;

                    // Calculate the center of the vertices
                    Vector3 centroid = Vector3.zero;
                    foreach (Vector3 vertex in vertices)
                    {
                        centroid += vertex;
                    }
                    centroid /= vertices.Length;

                    // Convert from local space to world space
                    centroid = floor.transform.TransformPoint(centroid);

                    return centroid;
                }
                else
                {
                    Debug.LogError("MeshFilter component not found on the Floor object.");
                    return Vector3.zero;
                }
            }
            else
            {
                Debug.LogError("Floor object not found in the specified room.");
                return Vector3.zero;
            }
        }
        else
        {
            Debug.LogError("Room object is null.");
            return Vector3.zero;
        }
    }

    public GameObject findAttachingToWall(Vector3 position, float radius = 0.1f)
    {
        // Create a sphere around the position to detect collisions
        Collider[] colliders = Physics.OverlapSphere(position, radius);

        foreach (Collider collider in colliders)
        {
            // Check if the collider's game object is tagged as a wall
            if (collider.CompareTag("Wall"))
            {
                return collider.gameObject;
            }
        }

        return null;
    }
}
