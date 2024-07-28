using System.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : Singleton<SpawnItem>
{
    float maxWidth = 1.0f; // Maximum width of the cuboid
    float maxHeight = 1.0f; // Maximum height of the cuboid
    float cuboidDepth = 0.1f; // Fixed depth of the cuboid
    string filePath = "Assets/Object/Images/MainAfter.jpg"; // Path to the image

    public GameObject SpawnItemObject(ItemDataManager.ItemData itemData)
    {
        if (itemData.dataType == "image")
        {
            return CreateImageItem(itemData);
        }
        return null;
    }

    // Creates a cuboid and applies an image texture to one face based on ItemData
    private GameObject CreateImageItem(ItemDataManager.ItemData itemData)
    {
        // Load the texture from the file path
        Texture2D imageTexture = LoadTexture(itemData.filePath);

        if (imageTexture == null)
        {
            Debug.LogError("Failed to load texture.");
            return null;
        }

        // Calculate cuboid size based on image dimensions
        float textureWidth = imageTexture.width;
        float textureHeight = imageTexture.height;

        // Calculate aspect ratio
        float aspectRatio = textureWidth / textureHeight;

        // Calculate the cuboid dimensions while ensuring they do not exceed the max allowed size
        float cuboidWidth = maxWidth;
        float cuboidHeight = maxHeight;

        // Adjust dimensions to maintain aspect ratio
        if (aspectRatio > 1) // Image is wider than tall
        {
            cuboidHeight = maxWidth / aspectRatio;
            if (cuboidHeight > maxHeight)
            {
                cuboidHeight = maxHeight;
                cuboidWidth = maxHeight * aspectRatio;
            }
        }
        else // Image is taller than wide or square
        {
            cuboidWidth = maxHeight * aspectRatio;
            if (cuboidWidth > maxWidth)
            {
                cuboidWidth = maxWidth;
                cuboidHeight = maxWidth / aspectRatio;
            }
        }

        Vector3 cuboidSize = new Vector3(cuboidWidth, cuboidHeight, cuboidDepth);

        // Create the cuboid
        GameObject cuboid = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cuboid.name = "image_item_" + itemData.id;
        cuboid.transform.localScale = cuboidSize;

        // Create a new material with transparency
        Material material = new Material(Shader.Find("Standard"));
        material.mainTexture = imageTexture;
        material.color = new Color(1, 1, 1, 0.5f); // Set the color with alpha for transparency

        // Apply the material to the cuboid
        Renderer renderer = cuboid.GetComponent<Renderer>();
        renderer.material = material;

        MeshUtils.Instance.ApplyFrontFaceTexture(cuboid, imageTexture);

        // Position the cuboid at the center of the scene
        cuboid.transform.position = new Vector3(1, 1, 1);

        InitializeTransform(cuboid);

        return cuboid;
    }

    // Loads a texture from a file path
    private Texture2D LoadTexture(string filePath)
    {
        if (System.IO.File.Exists(filePath))
        {
            Texture2D texture = new Texture2D(2, 2);
            byte[] imageData = System.IO.File.ReadAllBytes(filePath);
            texture.LoadImage(imageData);
            return texture;
        }
        else
        {
            Debug.LogError($"Image file not found: {filePath}");
            return null;
        }
    }

    private void InitializeTransform(GameObject gObject)
    {
        // Get the player's forward direction
        Vector3 playerCameraForward = CameraController.Instance.transform.forward;

        // Define the distance to place the object in front of the player
        float distanceInFront = 1.5f;

        // Calculate the new position
        Vector3 newPosition =
            PlayerController.Instance.GetPlayerPosition() + playerCameraForward * distanceInFront;

        Vector3 playerCameraPosition = CameraController.Instance.GetCameraPosition();
        newPosition.y = playerCameraPosition.y;

        // Set the object's position
        gObject.transform.position = newPosition;

        // Get the camera's position
        Vector3 cameraPosition = CameraController.Instance.GetCameraPosition();

        // Calculate the direction from the object to the camera
        Vector3 directionToCamera = cameraPosition - newPosition;

        // Calculate the rotation needed for the object's front face to face the camera
        Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera);

        // Set the object's rotation
        gObject.transform.rotation = targetRotation;
    }
}
