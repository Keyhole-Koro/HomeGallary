using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : Singleton<ItemMenu>
{
    private ScrollRect scrollRect;
    public GameObject itemPrefab;
    public float scrollSpeed = 2.0f; // Scroll speed

    private int contentWidth = 200;
    private int contentHeight = 100;

    void Start()
    {
        InitializeCanvas();
        InitializeScrollView();
        //AddColorChangingImages();
    }

    void Update()
    {
        HandleScrollInput();
    }

    // Initializes the Canvas for the UI
    private void InitializeCanvas()
    {
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler canvasScaler = canvasGO.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();
    }

    // Initializes the Scroll View within the Canvas
    private void InitializeScrollView()
    {
        GameObject scrollViewGO = new GameObject("ScrollView");
        scrollViewGO.transform.SetParent(transform);
        RectTransform scrollViewRect = scrollViewGO.AddComponent<RectTransform>();
        scrollViewRect.sizeDelta = new Vector2(Screen.width, contentHeight); // Adjust size as needed
        scrollViewRect.anchorMin = new Vector2(0.5f, 0f); // Anchor to the bottom
        scrollViewRect.anchorMax = new Vector2(0.5f, 0f); // Anchor to the bottom
        scrollViewRect.pivot = new Vector2(0.5f, 0f); // Pivot to the bottom center
        scrollViewRect.anchoredPosition = new Vector2(0, 10); // Position with some padding
        scrollRect = scrollViewGO.AddComponent<ScrollRect>();
        scrollRect.horizontal = true;
        scrollRect.vertical = false;

        // Scroll View Mask Image
        Image scrollViewImage = scrollViewGO.AddComponent<Image>();
        scrollViewImage.color = new Color(58f / 255f, 58f / 255f, 58f / 255f, 0.3f);
        scrollViewGO.AddComponent<Mask>();

        InitializeScrollViewContent(scrollViewGO);
    }

    // Initializes the Content area of the Scroll View
    private void InitializeScrollViewContent(GameObject scrollViewGO)
    {
        GameObject contentGO = new GameObject("Content");
        contentGO.transform.SetParent(scrollViewGO.transform);
        RectTransform contentRect = contentGO.AddComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(Screen.width, contentHeight); // Adjust size as needed
        contentRect.anchoredPosition = new Vector2(0, 0);

        HorizontalLayoutGroup layoutGroup = contentGO.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.spacing = 10f; // Space between items
        layoutGroup.padding = new RectOffset(10, 10, 10, 10); // Padding of 10px on all sides

        // Link Scroll View and Content
        scrollRect.content = contentRect;
    }

    // Handles user input to scroll the Scroll View
    private void HandleScrollInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            scrollRect.horizontalNormalizedPosition += scrollSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            scrollRect.horizontalNormalizedPosition -= scrollSpeed * Time.deltaTime;
        }
    }

    // Sets the visibility of the menu panel
    public void SetPanelVisibility(bool visible)
    {
        if (scrollRect == null)
        {
            Debug.LogError("ScrollRect not found in the scene!");
            return;
        }

        scrollRect.gameObject.SetActive(visible);
    }

    // Updates the menu with a list of item data
    public void UpdateMenu(List<ItemData> itemDataList)
    {
        if (scrollRect == null || itemPrefab == null)
        {
            Debug.LogError("ScrollRect or ItemPrefab is not assigned.");
            return;
        }

        // Clear existing items
        foreach (Transform child in scrollRect.content)
        {
            Destroy(child.gameObject);
        }

        // Fixed size for each item
        Vector2 fixedItemSize = new Vector2(contentWidth, contentHeight);

        // Calculate the total width needed for the content
        int numberOfItems = itemDataList.Count;
        float spacing = 10f; // Spacing between items
        float totalWidth = (numberOfItems * (fixedItemSize.x + spacing)) - spacing; // -spacing to remove extra spacing at the end

        // Update the Content RectTransform size
        RectTransform contentRect = scrollRect.content;
        contentRect.sizeDelta = new Vector2(totalWidth, contentRect.sizeDelta.y);

        // Add new items
        foreach (ItemData item in itemDataList)
        {
            // Instantiate a new item from the prefab
            GameObject itemGO = Instantiate(itemPrefab, scrollRect.content);
            RectTransform itemRect = itemGO.GetComponent<RectTransform>();

            // Set fixed size for the item
            itemRect.sizeDelta = fixedItemSize;

            // Get or add the Image component
            Image itemImage = itemGO.GetComponent<Image>();
            if (itemImage == null)
            {
                itemImage = itemGO.AddComponent<Image>();
            }

            // Get or add the Button component and set up click listener
            Button itemButton = itemGO.GetComponent<Button>();
            if (itemButton == null)
            {
                itemButton = itemGO.AddComponent<Button>();
            }

            // Set up the click event
            itemButton.onClick.AddListener(() => InputManager.Instance.OnItemInMenuClicked(item));

            // Set the Image component to preserve aspect ratio
            itemImage.preserveAspect = true;

            // Process based on item type
            if (item.dataType == "image")
            {
                // Set the image
                Texture2D texture = LoadImage(item.filePath);
                if (texture != null)
                {
                    itemImage.sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );
                }
            }
            else if (item.dataType == "3DModel")
            {
                // Handle 3D model (simplified here)
                itemImage.color = Color.gray; // Example color
            }

            // Place item in Scroll View content
            itemGO.transform.SetParent(scrollRect.content.transform, false);
        }
    }

    private Texture2D LoadImage(string filePath)
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

    // Adds a new item to the menu (not yet implemented)
    public void AddItem(Sprite itemSprite)
    {
        //ItemDataManager.Instance.AddItem();
    }
}
