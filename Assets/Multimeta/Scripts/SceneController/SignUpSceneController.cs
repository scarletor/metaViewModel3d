using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignUpSceneController : MonoBehaviour
{
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField userNameField;
    [SerializeField] TMP_InputField passwordField;

    [SerializeField] Button loginBtn;
    [SerializeField] Button signupBtn;

    [SerializeField] GameObject loadingAnim;
    [SerializeField] GameObject errorLabel;

    IServices services => MultimetaService.Instance;

    void Start()
    {
        loginBtn.onClick.AddListener(OnLoginClick);
        signupBtn.onClick.AddListener(OnSignUpClick);
    }

    private void OnSignUpClick()
    {
        string email = emailField.text;
        string username = userNameField.text;
        string password = passwordField.text;

        loadingAnim.SetActive(true);
        errorLabel.SetActive(false);
        services.SignUp(email, username, password, OnSignUpSuccess, OnSignUpFailure);
    }

    private void OnSignUpFailure(string obj)
    {
        loadingAnim.SetActive(false);
        errorLabel.SetActive(true);
    }

    private void OnSignUpSuccess(string obj)
    {
        SceneManager.LoadScene(SceneNameConfig.SIGNIN_SCENE);
    }

    private void OnLoginClick()
    {
        SceneManager.LoadScene(SceneNameConfig.SIGNIN_SCENE);
    }
}
