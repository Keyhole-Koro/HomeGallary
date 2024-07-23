using UnityEngine;
using UnityEngine.UI;

public class PresetPanel : MonoBehaviour
{
    public GameObject prefab; // ここに設置物のPrefabをアタッチする（例えばボタンなど）

    void Start()
    {
        SetupCanvas();
        SetupPresetPanel();
        SetupPanelImage();
        SetupLayoutGroup();
        InstantiatePrefab();
    }

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

    void SetupPresetPanel()
    {
        GameObject presetPanel = GameObject.Find("PresetPanel");
        if (presetPanel == null)
        {
            Debug.LogError("PresetPanel not found in the scene!");
            return;
        }

        RectTransform canvasRectTransform = GetComponent<RectTransform>();
        RectTransform presetPanelRectTransform = presetPanel.GetComponent<RectTransform>();
        presetPanelRectTransform.SetAnchor(AnchorPresets.BottomCenter);

        float screenWidth = canvasRectTransform.sizeDelta.x;
        float screenHeight = canvasRectTransform.sizeDelta.y;

        float width = screenWidth * 0.8f;
        float height = screenHeight * 0.2f;
        float xPosition = 0f;
        float yPosition = 0f;

        presetPanelRectTransform.sizeDelta = new Vector2(width, height);
        presetPanelRectTransform.anchoredPosition = new Vector2(xPosition, yPosition);
    }

    void SetupPanelImage()
    {
        GameObject presetPanel = GameObject.Find("PresetPanel");
        if (presetPanel == null)
        {
            Debug.LogError("PresetPanel not found in the scene!");
            return;
        }

        Image panelImage = presetPanel.GetComponent<Image>();
        if (panelImage == null)
        {
            panelImage = presetPanel.AddComponent<Image>();
        }

        panelImage.color = new Color(58f / 255f, 58f / 255f, 58f / 255f, 0.3f);
    }

    void SetupLayoutGroup()
    {
        GameObject presetPanel = GameObject.Find("PresetPanel");
        if (presetPanel == null)
        {
            Debug.LogError("PresetPanel not found in the scene!");
            return;
        }

        HorizontalLayoutGroup layoutGroup = presetPanel.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = presetPanel.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 10f;
        }
    }

    void InstantiatePrefab()
    {
        GameObject presetPanel = GameObject.Find("PresetPanel");
        if (presetPanel == null)
        {
            Debug.LogError("PresetPanel not found in the scene!");
            return;
        }

        GameObject item = Instantiate(prefab);
        item.transform.SetParent(presetPanel.transform, false);

        RectTransform rectTransform = item.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y * 0.5f);
        rectTransform.anchoredPosition -= new Vector2(0f, rectTransform.rect.height * 0.25f);
    }
}
