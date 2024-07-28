using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.I))
        {
            ModeManager.Instance.On_I_KeyDown();
        }
    }

    public void OnItemInMenuClicked(ItemDataManager.ItemData item)
    {
        GameObject spawnedItem = SpawnItem.Instance.SpawnItemObject(item);
        ModeManager.Instance.TrunOffOpenItemMenuMode();
    }
}
