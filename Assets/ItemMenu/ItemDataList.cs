using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpawnedItemDataList
{
    public List<SpawnedItemData> items;

    public SpawnedItemDataList()
    {
        items = new List<SpawnedItemData>();
    }
}
