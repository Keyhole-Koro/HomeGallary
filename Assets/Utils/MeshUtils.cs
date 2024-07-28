using UnityEngine;

public class MeshUtils : Singleton<MeshUtils>
{
    public void ApplyFrontFaceTexture(GameObject cubeObject, Texture2D frontTexture)
    {
        if (cubeObject == null)
        {
            Debug.LogError("Cube object is null.");
            return;
        }

        if (frontTexture == null)
        {
            Debug.LogError("Front texture is null.");
            return;
        }

        // Define vertices for the cube
        Vector3[] vertices =
        {
            // Back face
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            // Front face
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            // Top face
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            // Bottom face
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            // Left face
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            // Right face
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f)
        };

        // Define triangles for each face
        int[] backTriangles = { 0, 2, 1, 0, 3, 2 };
        int[] frontTriangles = { 4, 5, 6, 4, 6, 7 };
        int[] topTriangles = { 8, 9, 10, 8, 10, 11 };
        int[] bottomTriangles = { 12, 13, 14, 12, 14, 15 };
        int[] leftTriangles = { 16, 17, 18, 16, 18, 19 };
        int[] rightTriangles = { 20, 21, 22, 20, 22, 23 };

        // Define UV coordinates for each face
        Vector2[] uv =
        {
            // Back face
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            // Front face
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            // Top face
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            // Bottom face
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            // Left face
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            // Right face
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };

        // Create the mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.subMeshCount = 6;
        mesh.SetTriangles(frontTriangles, 0);
        mesh.SetTriangles(backTriangles, 1);
        mesh.SetTriangles(topTriangles, 2);
        mesh.SetTriangles(bottomTriangles, 3);
        mesh.SetTriangles(leftTriangles, 4);
        mesh.SetTriangles(rightTriangles, 5);

        // Create materials for each face
        Material frontMaterial = new Material(Shader.Find("Custom/DoubleSided"));
        frontMaterial.mainTexture = frontTexture;

        Material blackMaterial = new Material(Shader.Find("Custom/DoubleSided"));
        blackMaterial.color = Color.black;

        // Apply the mesh and materials to the cube object
        MeshFilter meshFilter = cubeObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = cubeObject.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = cubeObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = cubeObject.AddComponent<MeshRenderer>();
        }
        meshRenderer.materials = new Material[]
        {
            blackMaterial, // Back face
            frontMaterial, // Front face
            blackMaterial, // Top face
            blackMaterial, // Bottom face
            blackMaterial, // Left face
            blackMaterial // Right face
        };

        // Optimize the mesh


        // Optimize the mesh data
        mesh.Optimize();
        mesh.RecalculateNormals();
    }
}
