using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneController : MonoBehaviour
{
    [SerializeField] Button signInBtn;
    [SerializeField] Button connectWalletBtn;
    [SerializeField] Button playAsGuestBtn;

    void Start()
    {
        signInBtn.onClick.AddListener(OnSignInClick);
        connectWalletBtn.onClick.AddListener(OnConnectWalletClick);
        playAsGuestBtn.onClick.AddListener(OnPlayAsGuestClick);
    }

    private void OnPlayAsGuestClick()
    {
        SceneManager.LoadScene(SceneNameConfig.HOME_SCENE);
    }

    private void OnConnectWalletClick()
    {
        throw new NotImplementedException();
    }

    private void OnSignInClick()
    {
        SceneManager.LoadScene(SceneNameConfig.SIGNIN_SCENE);
    }
}
