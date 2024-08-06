using UnityEngine;
using UnityEngine.UI;

public class ItemPlacementButton : Singleton<ItemPlacementButton>
{
    private PlaceItem placeItem;

    GameObject canvasObj;
    public LayerMask ButtonLayer { get; private set; }
    public string buttonName = "ItemPlacementDoneButton";
    private Button completeButton;
    private RectTransform buttonRect;
    private Image buttonImage;

    public void Initialize(PlaceItem placeItemInstance)
    {
        placeItem = placeItemInstance;
        CreateUI();
    }

    public void SetVisible()
    {
        completeButton.gameObject.SetActive(true);
    }

    public void SetInvisible()
    {
        completeButton.gameObject.SetActive(false);
    }

    public void DestroyButton()
    {
        Destroy(canvasObj);
    }

    public ItemPlacementButton CreateUI()
    {
        // Create Canvas
        canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        RectTransform rectTransform = canvasObj.GetComponent<RectTransform>();
        rectTransform.SetAnchor(AnchorPresets.MiddleCenter);
        rectTransform.SetPivot(PivotPresets.MiddleCenter);

        // Create Button
        GameObject buttonObj = new GameObject(buttonName);
        buttonObj.transform.SetParent(canvas.transform);

        buttonObj.AddComponent<BoxCollider2D>();

        buttonObj.transform.localPosition = new Vector3(100, -100, 0);

        // Add RectTransform and set size
        buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(50, 50); // Size of the rounded button

        completeButton = buttonObj.AddComponent<Button>();

        // Set button background
        buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = Color.gray;
        buttonImage.sprite = CreateRoundedSprite(50); // Create rounded button

        // Set button layer
        ButtonLayer = LayerMask.GetMask("ButtonLayer");
        buttonObj.layer = LayerMask.NameToLayer("ButtonLayer");

        // Set button click event
        completeButton.onClick.AddListener(OnCompleteButtonClicked);

        // Initially hide the button
        completeButton.gameObject.SetActive(false);

        return this;
    }

    void OnCompleteButtonClicked()
    {
        if (placeItem != null && placeItem.transform.position == placeItem.targetPosition)
        {
            // Event handling code here
            Debug.Log("Complete button clicked.");
        }
    }

    Sprite CreateRoundedSprite(float diameter)
    {
        Texture2D texture = new Texture2D((int)diameter, (int)diameter);
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        texture.SetPixels(pixels);

        // Draw rounded button
        float radius = diameter / 2f;
        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                float dx = x - radius;
                float dy = y - radius;
                if (dx * dx + dy * dy < radius * radius)
                {
                    texture.SetPixel(x, y, Color.gray);
                }
            }
        }
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, diameter, diameter), new Vector2(0.5f, 0.5f));
    }
}
