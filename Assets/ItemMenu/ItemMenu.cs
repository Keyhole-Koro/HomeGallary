using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : Singleton<ItemMenu>
{
    private ScrollRect scrollRect;
    public float scrollSpeed = 2.0f; // Scroll speed

    void Start()
    {
        // Create Canvas
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler canvasScaler = canvasGO.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // Create Scroll View
        GameObject scrollViewGO = new GameObject("ScrollView");
        scrollViewGO.transform.SetParent(canvasGO.transform);
        RectTransform scrollViewRect = scrollViewGO.AddComponent<RectTransform>();
        scrollViewRect.sizeDelta = new Vector2(Screen.width, 100); // Adjust size as needed
        scrollViewRect.anchorMin = new Vector2(0.5f, 0f); // Anchor to the bottom
        scrollViewRect.anchorMax = new Vector2(0.5f, 0f); // Anchor to the bottom
        scrollViewRect.pivot = new Vector2(0.5f, 0f); // Pivot to the bottom center
        scrollViewRect.anchoredPosition = new Vector2(0, 10); // Position with some padding
        scrollRect = scrollViewGO.AddComponent<ScrollRect>();
        scrollRect.horizontal = true;
        scrollRect.vertical = false;

        // Scroll View Mask Image
        Image scrollViewImage = scrollViewGO.AddComponent<Image>();
        scrollViewImage.color = new Color(1, 1, 1, 0.5f);
        scrollViewGO.AddComponent<Mask>();

        // Create Content
        GameObject contentGO = new GameObject("Content");
        contentGO.transform.SetParent(scrollViewGO.transform);
        RectTransform contentRect = contentGO.AddComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(1200, 200); // Adjust size as needed
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

        // Add color-changing Image elements
        for (int i = 0; i < 5; i++)
        {
            GameObject imageGO = new GameObject("Image" + i);
            imageGO.transform.SetParent(contentGO.transform);
            RectTransform imageRect = imageGO.AddComponent<RectTransform>();
            imageRect.sizeDelta = new Vector2(200, 200); // Size of each item

            Image image = imageGO.AddComponent<Image>();
            image.color = new Color(Random.value, Random.value, Random.value); // Random color
        }
    }

    void Update()
    {
        // Input for left arrow key
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            scrollRect.horizontalNormalizedPosition -= scrollSpeed * Time.deltaTime;
        }
        // Input for right arrow key
        if (Input.GetKey(KeyCode.RightArrow))
        {
            scrollRect.horizontalNormalizedPosition += scrollSpeed * Time.deltaTime;
        }
    }

    // Method to set the visibility of the menu panel
    public void SetPanelVisibility(bool visible)
    {
        if (scrollRect == null)
        {
            Debug.LogError("ScrollRect not found in the scene!");
            return;
        }

        scrollRect.gameObject.SetActive(visible);
    }
}
