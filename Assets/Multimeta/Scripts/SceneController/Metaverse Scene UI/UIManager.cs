using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : SingletonMonoBehaviorBase<UIManager>
{
    [Header("Top Down Menu")]
    [SerializeField] public MainMenuController mainMenu;
    [SerializeField] public BalanceMenuController balanceMenu;

    [Header("Center Popup Menu")]
    [SerializeField] public MediaMenuController mediaMenu;
    [SerializeField] public MediaMenuDetailController mediaMenuDetail;
    [SerializeField] public EbookMenuController ebookMenu;
    [SerializeField] public EbookMenuDetailController ebookMenuDetail;

    [Serializable]
    public enum MenuType
    {
        MainMenu,
        BalanceMenu,
        MediaMenu,
        MediaDetailMenu,
        EbookMenu,
        EbookDetailMenu,
    }

    void Awake()
    {
        SceneManager.sceneUnloaded += (scene) =>
        {
            if (scene.name == SceneNameConfig.METAVERSE_SCENE)
                Destroy(gameObject);
        };
    }

    public void HideAll()
    {
        mainMenu.gameObject.SetActive(false);
        balanceMenu.gameObject.SetActive(false);
        //-----
        mediaMenu.gameObject.SetActive(false);
        mediaMenuDetail.gameObject.SetActive(false);
        ebookMenu.gameObject.SetActive(false);
        ebookMenuDetail.gameObject.SetActive(false);
    }

    public void OpenMenu(MenuType type)
    {
        HideAll();

        switch (type)
        {
            case MenuType.MainMenu:
                mainMenu.gameObject.SetActive(true);
                break;
            case MenuType.BalanceMenu:
                balanceMenu.gameObject.SetActive(true);
                break;
            case MenuType.MediaMenu:
                mediaMenu.gameObject.SetActive(true);
                break;
            case MenuType.MediaDetailMenu:
                mediaMenuDetail.gameObject.SetActive(true);
                break;
            case MenuType.EbookMenu:
                ebookMenu.gameObject.SetActive(true);
                break;
            case MenuType.EbookDetailMenu:
                ebookMenuDetail.gameObject.SetActive(true);
                break;
        }
    }
}
