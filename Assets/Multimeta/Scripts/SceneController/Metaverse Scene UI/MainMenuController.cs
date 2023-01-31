using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Button mediaButton;
    [SerializeField] Button ebookButton;
    [SerializeField] Button shopButton;
    [SerializeField] Button supportButton;
    [SerializeField] Button homeButton;

    private UIManager UIManager => UIManager.Instance;

    void Start()
    {
        mediaButton.onClick.AddListener(OnMediaButtonClick);
        ebookButton.onClick.AddListener(OnEbookButtonClick);
        homeButton.onClick.AddListener(OnHomeButtonClick);
    }

    private void OnHomeButtonClick()
    {
        SceneManager.LoadScene(SceneNameConfig.HOME_SCENE);
    }

    private void OnEbookButtonClick()
    {
        UIManager.OpenMenu(UIManager.MenuType.EbookMenu);
    }

    private void OnMediaButtonClick()
    {
        UIManager.OpenMenu(UIManager.MenuType.MediaMenu);
    }
}
