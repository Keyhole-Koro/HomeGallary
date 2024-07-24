using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : MonoBehaviour
{
    public static ItemMenu instance;
    private string objectTag = "Preset"; // Tag to identify objects to be added to the menu
    private GameObject menuPanel;

    void Awake()
    {
        // シングルトンインスタンスの設定
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーンが切り替わってもオブジェクトを保持
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetupCanvas();
        SetupMenuPanel();
        SetupPanelImage();
        SetupLayoutGroup();
    }

    public static ItemMenu Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ItemMenu>();
                if (instance == null)
                {
                    Debug.LogError("ItemMenu instance not found in the scene!");
                }
            }
            return instance;
        }
    }

    // Method to set up the Canvas component
    void SetupCanvas()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas component not found on this GameObject!");
            return;
        }

        CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
        if (canvasScaler == null)
        {
            canvasScaler = canvas.gameObject.AddComponent<CanvasScaler>();
        }

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.3f;
    }

    // Method to set up the menu panel
    void SetupMenuPanel()
    {
        menuPanel = GameObject.Find("MenuPanel");
        if (menuPanel == null)
        {
            Debug.LogError("MenuPanel not found in the scene!");
            return;
        }

        RectTransform canvasRectTransform = GetComponent<RectTransform>();
        RectTransform menuPanelRectTransform = menuPanel.GetComponent<RectTransform>();
        menuPanelRectTransform.SetAnchor(AnchorPresets.BottomCenter);

        float screenWidth = canvasRectTransform.sizeDelta.x;
        float screenHeight = canvasRectTransform.sizeDelta.y;

        float width = screenWidth * 0.8f;
        float height = screenHeight * 0.2f;
        float xPosition = 0f;
        float yPosition = 0f;

        menuPanelRectTransform.sizeDelta = new Vector2(width, height);
        menuPanelRectTransform.anchoredPosition = new Vector2(xPosition, yPosition);
    }

    // Method to set up the panel's image component
    void SetupPanelImage()
    {
        if (menuPanel == null)
        {
            Debug.LogError("MenuPanel not found in the scene!");
            return;
        }

        Image panelImage = menuPanel.GetComponent<Image>();
        if (panelImage == null)
        {
            panelImage = menuPanel.AddComponent<Image>();
        }

        panelImage.color = new Color(58f / 255f, 58f / 255f, 58f / 255f, 0.3f);
    }

    // Method to set up the menu panel's layout group
    void SetupLayoutGroup()
    {
        if (menuPanel == null)
        {
            Debug.LogError("MenuPanel not found in the scene!");
            return;
        }

        HorizontalLayoutGroup layoutGroup = menuPanel.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = menuPanel.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 10f;
        }
    }

    // Method to set the visibility of the menu panel
    public void SetPanelVisibility(bool visible)
    {
        if (menuPanel == null)
        {
            Debug.LogError("MenuPanel not found in the scene!");
            return;
        }

        menuPanel.SetActive(visible);
    }

    // Method to instantiate a prefab as a menu item
    public void InstantiateMenuItem(GameObject prefab)
    {
        if (menuPanel == null)
        {
            Debug.LogError("MenuPanel not found in the scene!");
            return;
        }

        GameObject item = Instantiate(prefab);
        item.transform.SetParent(menuPanel.transform, false);

        // You can add additional customization here if needed
    }

    // Example method for handling client-triggered prefab instantiation
    public void HandleClientPrefabInstantiation(GameObject clientPrefab)
    {
        InstantiateMenuItem(clientPrefab);
    }
}
