using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginSceneController : MonoBehaviour
{
    [SerializeField] TMP_InputField userNameField;
    [SerializeField] TMP_InputField passwordField;

    [SerializeField] Button forgotPasswordBtn;
    [SerializeField] Button guestBtn;
    [SerializeField] Button loginBtn;
    [SerializeField] Button createAccountBtn;

    [SerializeField] GameObject loadingAnim;
    [SerializeField] GameObject errorLabel;

    IServices services => MultimetaService.Instance;

    void Start()
    {
        forgotPasswordBtn.onClick.AddListener(OnForgotPasswordClick);
        guestBtn.onClick.AddListener(OnGuestClick);
        loginBtn.onClick.AddListener(OnLoginClick);
        createAccountBtn.onClick.AddListener(OnCreateAccountClick);
    }

    private void OnCreateAccountClick()
    {
        SceneManager.LoadScene(SceneNameConfig.SIGNUP_SCENE);
    }

    private void OnLoginClick()
    {
        string userName = userNameField.text;
        string password = passwordField.text;

        loadingAnim.SetActive(true);
        errorLabel.SetActive(false);
        services.SignIn(userName, password, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginFailure(string obj)
    {
        loadingAnim.SetActive(false);
        errorLabel.SetActive(true);
    }

    private void OnLoginSuccess(ModelDatas.SignInResData obj)
    {
        loadingAnim.SetActive(false);
        SceneManager.LoadScene(SceneNameConfig.HOME_SCENE);
    }

    private void OnGuestClick()
    {
        SceneManager.LoadScene(SceneNameConfig.HOME_SCENE);
    }

    private void OnForgotPasswordClick()
    {
        throw new NotImplementedException();
    }
}
