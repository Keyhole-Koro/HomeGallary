using UnityEngine;
using UnityEngine.EventSystems;

public class ItemMenuEventSystem : Singleton<ItemMenuEventSystem>
{
    private EventSystem eventSystem;
    private StandaloneInputModule inputModule;

    void Start()
    {
        eventSystem = gameObject.AddComponent<EventSystem>();

        inputModule = gameObject.AddComponent<StandaloneInputModule>();

        inputModule.forceModuleActive = true;
    }

    public EventSystem GetEventSystem()
    {
        return eventSystem;
    }

    public StandaloneInputModule GetInputModule()
    {
        return inputModule;
    }
}
