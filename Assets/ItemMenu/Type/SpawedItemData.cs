using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpawnedItemData
{
    public string itemId;
    public string room;
    public Vector3 localPosition;
    public Vector3 rotation;

    public SpawnedItemData(string itemId, string room, Vector3 localPosition, Vector3 rotation)
    {
        this.itemId = itemId;
        this.room = room;
        this.localPosition = localPosition;
        this.rotation = rotation;
    }
}
