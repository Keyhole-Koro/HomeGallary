using UnityEngine;

public class ScreenManager : Singleton<ScreenManager>
{
    private bool isInWorkMode = false;

    void Start()
    {
        // 初期状態で非表示にする
        if (ItemMenu.Instance != null)
        {
            ItemMenu.Instance.SetPanelVisibility(false);
        }
    }

    void Update() { }

    public void ShowItemMenu()
    {
        if (ItemMenu.Instance != null)
        {
            ItemMenu.Instance.SetPanelVisibility(true);
        }
    }

    public void HideItemMenu()
    {
        if (ItemMenu.Instance != null)
        {
            ItemMenu.Instance.SetPanelVisibility(false);
        }
    }
}
