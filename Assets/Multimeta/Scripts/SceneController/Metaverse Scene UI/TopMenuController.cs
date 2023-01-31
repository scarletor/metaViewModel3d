using UnityEngine;
using UnityEngine.UI;

public class TopMenuController : MonoBehaviour
{
    [SerializeField] Button showMainMenuButton;
    [SerializeField] Button showBalanceButton;

    private UIManager UIManager => UIManager.Instance;

    void Start()
    {
        showMainMenuButton.onClick.AddListener(ShowMainMenuButtonClick);
        showBalanceButton.onClick.AddListener(ShowBalanceMenuButtonClick);
    }

    private void ShowBalanceMenuButtonClick()
    {
        UIManager.OpenMenu(UIManager.MenuType.BalanceMenu);
    }

    private void ShowMainMenuButtonClick()
    {
        UIManager.OpenMenu(UIManager.MenuType.MainMenu);
    }
}
