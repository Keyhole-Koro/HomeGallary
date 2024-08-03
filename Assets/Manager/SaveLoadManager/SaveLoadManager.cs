using UnityEngine;
using System.Collections.Generic;
using System.IO;

public interface ISaveLoadManager
{
    void SaveItemData(List<SpawnedItemData> itemDataList);
    List<SpawnedItemData> LoadItemData();
}

public class SaveLoadManager : ISaveLoadManager
{
    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "spawnedItemData.json");
    }

    public void SaveItemData(List<SpawnedItemData> itemDataList)
    {
        SpawnedItemDataList dataList = new SpawnedItemDataList { items = itemDataList };
        string json = JsonUtility.ToJson(dataList, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Spawned item data saved to " + filePath);
    }

    public List<SpawnedItemData> LoadItemData()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("No save file found at " + filePath);
            return new List<SpawnedItemData>();
        }

        string json = File.ReadAllText(filePath);
        SpawnedItemDataList dataList = JsonUtility.FromJson<SpawnedItemDataList>(json);
        return dataList.items;
    }
}
