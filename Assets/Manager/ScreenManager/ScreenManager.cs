using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    private bool isInWorkMode = false;

    public static ScreenManager instance;

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

    public static ScreenManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ScreenManager>();
                if (instance == null)
                {
                    Debug.LogError("ScreenManager instance not found in the scene!");
                }
            }
            return instance;
        }
    }

    void Start()
    {
        // 初期状態で非表示にする
        if (ItemMenu.Instance != null)
        {
            ItemMenu.Instance.SetPanelVisibility(false);
        }
    }

    void Update() { }

    public void OnIKeyDown()
    {
        isInWorkMode = !isInWorkMode;

        // Update panel visibility based on work mode
        if (ItemMenu.Instance != null)
        {
            ItemMenu.Instance.SetPanelVisibility(isInWorkMode);
        }
    }

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
