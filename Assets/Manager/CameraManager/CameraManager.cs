using System.Collections.Generic;
using UnityEngine;
using System.Linq; // For FirstOrDefault()

public class CameraManager : Singleton<CameraManager>
{
    private Dictionary<string, Camera> cameraDict;
    private Camera currentCamera;
    private Camera previousCamera;

    void Start()
    {
        // Automatically find and store all cameras in the Dictionary
        cameraDict = new Dictionary<string, Camera>();

        // Get all cameras in the scene
        Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            string cameraName = cam.gameObject.name;
            if (!cameraDict.ContainsKey(cameraName))
            {
                cameraDict[cameraName] = cam;
            }
        }
    }

    public void SwitchToPlayerCamera()
    {
        SwitchCamera("PlayerCamera");
    }

    public void SwitchCamera(string cameraName)
    {
        if (cameraDict.TryGetValue(cameraName, out Camera newCamera))
        {
            SwitchCamera(newCamera);
        }
        else
        {
            Debug.LogWarning("Camera not found: " + cameraName);
        }
    }

    public void SwitchCamera(Camera camera)
    {
        previousCamera = currentCamera;
        SetActiveCamera(camera);
    }

    void SetActiveCamera(Camera newCamera)
    {
        // Disable the current camera
        if (currentCamera != null)
        {
            currentCamera.gameObject.SetActive(false);
        }

        // Enable the new camera
        if (newCamera != null)
        {
            newCamera.gameObject.SetActive(true);
            currentCamera = newCamera;
        }
    }

    public GameObject CreateAndRegisterCamera(string name)
    {
        // Create a new GameObject for the camera
        GameObject cameraObject = new GameObject(name);

        // Add a Camera component to the GameObject
        Camera camera = cameraObject.AddComponent<Camera>();

        // Optionally, configure additional camera settings here
        camera.clearFlags = CameraClearFlags.Skybox;
        camera.backgroundColor = Color.gray;

        // Add the new camera to the dictionary
        if (!cameraDict.ContainsKey(name))
        {
            cameraDict[name] = camera;
        }

        return cameraObject;
    }

    public void SwitchToPreviousCamera()
    {
        if (!previousCamera)
            return;
        SwitchCamera(previousCamera);
    }

    public void DestroyAndLeaveCurrentCamera()
    {
        // Check if the cameraObject exists in the dictionary
        string cameraName = currentCamera.gameObject.name;
        if (cameraDict.TryGetValue(cameraName, out Camera camera))
        {
            // Remove the camera from the dictionary
            cameraDict.Remove(cameraName);

            // Disable the camera before destroying it
            if (camera != null)
            {
                camera.gameObject.SetActive(false);
            }

            // Destroy the camera object
            Destroy(currentCamera);
        }
        else
        {
            Debug.LogWarning("Camera not found in dictionary: " + cameraName);
        }

        SwitchToPreviousCamera();
    }
}
