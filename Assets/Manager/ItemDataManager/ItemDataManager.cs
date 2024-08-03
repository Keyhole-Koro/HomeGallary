using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class ItemDataManager : Singleton<ItemDataManager>
{
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
            SaveData();
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
            Texture2D texture = new Texture2D(2, 2);
            byte[] imageData = File.ReadAllBytes(filePath);
            texture.LoadImage(imageData);

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
        GameObject model = new GameObject("LoadedModel");
        return model;
    }

    public ItemData FindItemById(string id)
    {
        return itemDataList.Find(item => item.id == id);
    }

    public void RemoveItemById(string id)
    {
        ItemData itemToRemove = FindItemById(id);
        if (itemToRemove != null)
        {
            itemDataList.Remove(itemToRemove);
            SaveData();
        }
        else
        {
            Debug.LogError($"Item with ID {id} not found!");
        }
    }

    public void AddItem(ItemData newItem)
    {
        itemDataList.Add(newItem);
        SaveData();
    }

    void SaveData()
    {
        string directoryPath = Path.GetDirectoryName(jsonFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string json = JsonConvert.SerializeObject(itemDataList, Formatting.Indented);
        File.WriteAllText(jsonFilePath, json);

        lastModifiedTime = File.GetLastWriteTime(jsonFilePath).ToString();
    }
}
