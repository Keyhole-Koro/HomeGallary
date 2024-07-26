using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class ItemDataManager : Singleton<ItemDataManager>
{
    [System.Serializable]
    public class ItemData
    {
        public string id;
        public string dataType;
        public string filePath;
        public List<string> tags;
    }

    string jsonFilePath = "Assets/ItemMenu/ItemData/data.json";
    public List<ItemData> itemDataList = new List<ItemData>();
    private string lastModifiedTime;

    void Start()
    {
        CheckAndLoadData();
    }

    void CheckAndLoadData()
    {
        string directoryPath = Path.GetDirectoryName(jsonFilePath);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (File.Exists(jsonFilePath))
        {
            // Check if file modification time has changed
            string currentModifiedTime = File.GetLastWriteTime(jsonFilePath).ToString();
            if (lastModifiedTime != currentModifiedTime)
            {
                Debug.Log("File has been modified. Reloading data.");
                LoadData();
                lastModifiedTime = currentModifiedTime;
            }
        }
        else
        {
            Debug.LogWarning("JSON file not found. Creating a new one.");
            SaveData(); // SaveData() with no arguments
        }
    }

    void LoadData()
    {
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            itemDataList =
                JsonConvert.DeserializeObject<List<ItemData>>(json) ?? new List<ItemData>();

            ItemMenu.Instance.UpdateMenu(itemDataList);
        }
        else
        {
            Debug.LogError("JSON file not found!");
        }
    }

    void LoadImage(string filePath)
    {
        if (File.Exists(filePath))
        {
            // Load and process the image
            Texture2D texture = new Texture2D(2, 2);
            byte[] imageData = File.ReadAllBytes(filePath);
            texture.LoadImage(imageData);

            // Display the image in the menu, etc.
            Debug.Log($"Image loaded from {filePath}");
        }
        else
        {
            Debug.LogError($"Image file not found: {filePath}");
        }
    }

    void Load3DModel(string filePath)
    {
        if (File.Exists(filePath))
        {
            // Load and process the 3D model
            GameObject model = LoadModelFromFile(filePath);
            Instantiate(model, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError($"3D model file not found: {filePath}");
        }
    }

    GameObject LoadModelFromFile(string filePath)
    {
        // Load the 3D model from the file
        // Simplified example
        GameObject model = new GameObject("LoadedModel");
        // Actual implementation will depend on the file format
        return model;
    }

    public ItemData FindItemById(string id)
    {
        // Find and return item by ID
        return itemDataList.Find(item => item.id == id);
    }

    public void RemoveItemById(string id)
    {
        // Find item by ID and remove it from the list
        ItemData itemToRemove = FindItemById(id);
        if (itemToRemove != null)
        {
            itemDataList.Remove(itemToRemove);
            SaveData(); // SaveData() with no arguments
        }
        else
        {
            Debug.LogError($"Item with ID {id} not found!");
        }
    }

    public void AddItem(ItemData newItem)
    {
        // Add new item to the list and save the data
        itemDataList.Add(newItem);
        SaveData(); // SaveData() with no arguments
    }

    void SaveData()
    {
        // Ensure the directory exists
        string directoryPath = Path.GetDirectoryName(jsonFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Serialize the data and write to file
        string json = JsonConvert.SerializeObject(itemDataList, Formatting.Indented);
        File.WriteAllText(jsonFilePath, json);

        // Update the last modified time
        lastModifiedTime = File.GetLastWriteTime(jsonFilePath).ToString();
    }
}
