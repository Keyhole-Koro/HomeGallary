using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : Singleton<SpawnItem>
{
    float maxWidth = 1.0f; // Maximum width of the cuboid
    float maxHeight = 1.0f; // Maximum height of the cuboid
    float cuboidDepth = 0.1f; // Fixed depth of the cuboid

    public GameObject SpawnItemObject(ItemData itemData)
    {
        if (itemData.dataType == "image")
        {
            return CreateImageItem(itemData);
        }
        return null;
    }

    private GameObject CreateImageItem(ItemData itemData)
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

        // Set the initial position and rotation
        InitializeTransform(cuboid);

        cuboid.layer = LayerMask.NameToLayer("SpawnedItemLayer");

        ItemDataComponent itemDataComponent = cuboid.AddComponent<ItemDataComponent>();
        itemDataComponent.itemData = itemData;

        return cuboid;
    }

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
        Vector3 playerCameraForward = PlayerCameraController.Instance.transform.forward;

        float distanceInFront = 1.5f;

        Vector3 newPosition =
            PlayerController.Instance.GetPlayerPosition() + playerCameraForward * distanceInFront;

        Vector3 playerCameraPosition = PlayerCameraController.Instance.GetCameraPosition();
        newPosition.y = playerCameraPosition.y;

        gObject.transform.position = newPosition;

        Vector3 cameraPosition = PlayerCameraController.Instance.GetCameraPosition();

        Vector3 directionToCamera = cameraPosition - newPosition;

        Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera);

        gObject.transform.rotation = targetRotation;
    }
}
