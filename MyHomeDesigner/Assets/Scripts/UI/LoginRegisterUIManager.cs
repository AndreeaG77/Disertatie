using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;

public class LoginRegisterUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject forgotPasswordPanel;

    [Header("Login References")]
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;
    public GameObject loginErrorText;

    [Header("Password Visibility Buttons")]
    public GameObject seePasswordButton;
    public GameObject hidePasswordButton;


    [Header("Login Options")]
    public Toggle rememberMeToggle;

    [Header("Register References")]
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerConfirmPasswordInput;
    public GameObject registerErrorEmailExistsText;
    public GameObject registerErrorPasswordMismatchText;

    [Header("Register Password Toggles")]
    public GameObject seePasswordButton1;
    public GameObject hidePasswordButton1;

    public GameObject seePasswordButton2;
    public GameObject hidePasswordButton2;

    [Header("Forgot password References")]
    public TMP_InputField forgotPasswordEmailInput;
    public GameObject emailNotFoundText;
    public GameObject passwordResetMessage;
    public GameObject submitButton;
    public GameObject backButton;

    [Header("Popup")]
    public GameObject successPopup;


    void Start()
    {
        // Init panels
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);

        loginErrorText.SetActive(false);
        registerErrorEmailExistsText.SetActive(false);
        registerErrorPasswordMismatchText.SetActive(false);
        successPopup.SetActive(false);

        hidePasswordButton.SetActive(true);
        seePasswordButton.SetActive(false);
        loginPasswordInput.contentType = TMP_InputField.ContentType.Password;
        loginPasswordInput.ForceLabelUpdate();

        registerPasswordInput.contentType = TMP_InputField.ContentType.Password;
        registerConfirmPasswordInput.contentType = TMP_InputField.ContentType.Password;
    
        hidePasswordButton1.SetActive(true);
        seePasswordButton1.SetActive(false);
    
        hidePasswordButton2.SetActive(true);
        seePasswordButton2.SetActive(false);
    
        registerPasswordInput.ForceLabelUpdate();
        registerConfirmPasswordInput.ForceLabelUpdate();

        if (PlayerPrefs.GetInt("rememberMe", 0) == 1)
        {
            string savedEmail = PlayerPrefs.GetString("userEmail");
            loginEmailInput.text = savedEmail;
            loginPasswordInput.text = PlayerPrefs.GetString("userPassword");;
            rememberMeToggle.isOn = true;
        }
    }

    // ==== Panel Switches ====
    public void OpenRegisterPanel()
    {
        loginErrorText.SetActive(false);
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    public void OpenLoginPanel()
    {
        registerPanel.SetActive(false);
        registerEmailInput.text = "";
        registerPasswordInput.text = "";
        registerConfirmPasswordInput.text = "";
        loginPanel.SetActive(true);
    }

    // ==== Login ====
    public void OnLoginClicked()
    {
        if (PlayerPrefs.HasKey("userToken"))
        {
            string savedEmail = PlayerPrefs.GetString("userEmail");
            string savedToken = PlayerPrefs.GetString("userToken");
            StartCoroutine(TokenLoginRequest(savedEmail, savedToken));
        }
        else
        {
            string email = loginEmailInput.text;
            string password = loginPasswordInput.text;
            StartCoroutine(LoginRequest(email, password));
        }

    }

    IEnumerator LoginRequest(string email, string password)
    {
        string jsonData = JsonUtility.ToJson(new LoginPayload(email, password));

        UnityWebRequest request = new UnityWebRequest("https://disertatie-backend.onrender.com/api/login", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {

            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

            if (rememberMeToggle.isOn)
            {
                PlayerPrefs.SetString("userEmail", email);
                PlayerPrefs.SetString("userToken", response.token);
                PlayerPrefs.SetString("userPassword", password);
                PlayerPrefs.SetInt("rememberMe", 1);
                PlayerPrefs.Save();
            }

            PlayerPrefs.SetString("sessionEmail", email);
            PlayerPrefs.SetString("sessionToken", response.token);
            PlayerPrefs.SetString("sessionPassword", password);
            PlayerPrefs.Save();
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.LogWarning("Login failed: " + request.downloadHandler.text);
            loginErrorText.SetActive(true);
        }
    }

    IEnumerator TokenLoginRequest(string email, string token)
    {
        string url = "https://disertatie-backend.onrender.com/api/token-login";
        TokenLoginPayload data = new TokenLoginPayload { email = email, token = token };
        string jsonData = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            PlayerPrefs.SetString("sessionEmail", email);
            PlayerPrefs.SetString("sessionToken", token);
            PlayerPrefs.Save();
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.LogWarning("Token login failed: " + request.downloadHandler.text);
            loginErrorText.SetActive(true);
        }
    }

    public void OnSeePasswordClicked()
    {
        seePasswordButton.SetActive(false);
        hidePasswordButton.SetActive(true);
        loginPasswordInput.contentType = TMP_InputField.ContentType.Password;
        loginPasswordInput.ForceLabelUpdate();
    }

    public void OnHidePasswordClicked()
    {
        seePasswordButton.SetActive(true);
        hidePasswordButton.SetActive(false);
        loginPasswordInput.contentType = TMP_InputField.ContentType.Standard;
        loginPasswordInput.ForceLabelUpdate();
    }


    // ==== Register ====
    public void OnSignupClicked()
    {
        string email = registerEmailInput.text.Trim();
        string password = registerPasswordInput.text;
        string confirmPassword = registerConfirmPasswordInput.text;

        // Errors reset
        registerErrorEmailExistsText.SetActive(false);
        registerErrorPasswordMismatchText.SetActive(false);

        if (password != confirmPassword)
        {
            registerErrorPasswordMismatchText.SetActive(true);
            return;
        }

        StartCoroutine(RegisterUser(email, password));
    }

    IEnumerator RegisterUser(string email, string password)
    {
        string url = "https://disertatie-backend.onrender.com/api/register";

        RegisterRequest data = new RegisterRequest { email = email, password = password };
        string jsonData = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            registerPanel.SetActive(false);
            registerEmailInput.text = "";
            registerPasswordInput.text = "";
            registerConfirmPasswordInput.text = "";
            loginPanel.SetActive(true);
            StartCoroutine(ShowSuccessPopup());
        }
        else
        {
            Debug.Log("Register failed: " + request.error);
            if (request.responseCode == 400)
            {
                registerErrorEmailExistsText.SetActive(true);
            }
        }
    }


    IEnumerator ShowSuccessPopup()
    {
        successPopup.SetActive(true);
        yield return new WaitForSeconds(1f);
        successPopup.SetActive(false);
    }

    public void OnSeePasswordRegister1Clicked()
    {
        registerPasswordInput.contentType = TMP_InputField.ContentType.Password;
        registerPasswordInput.ForceLabelUpdate();

        seePasswordButton1.SetActive(false);
        hidePasswordButton1.SetActive(true);
    }

    public void OnHidePasswordRegister1Clicked()
    {
        registerPasswordInput.contentType = TMP_InputField.ContentType.Standard;
        registerPasswordInput.ForceLabelUpdate();

        hidePasswordButton1.SetActive(false);
        seePasswordButton1.SetActive(true);
    }

    public void OnSeePasswordRegister2Clicked()
    {
        registerConfirmPasswordInput.contentType = TMP_InputField.ContentType.Password;
        registerConfirmPasswordInput.ForceLabelUpdate();

        seePasswordButton2.SetActive(false);
        hidePasswordButton2.SetActive(true);
    }

    public void OnHidePasswordRegister2Clicked()
    {
        registerConfirmPasswordInput.contentType = TMP_InputField.ContentType.Standard;
        registerConfirmPasswordInput.ForceLabelUpdate();

        hidePasswordButton2.SetActive(false);
        seePasswordButton2.SetActive(true);
    }

    public void OnRememberMeToggled()
    {
        if (rememberMeToggle.isOn)
        {
            PlayerPrefs.SetInt("rememberMe", 1);
        }
        else
        {
            PlayerPrefs.DeleteKey("userEmail");
            PlayerPrefs.DeleteKey("userToken");
            PlayerPrefs.DeleteKey("userPassword");
            PlayerPrefs.SetInt("rememberMe", 0);
            if (loginPasswordInput.text == "********")
                loginPasswordInput.text = "";
        }

        PlayerPrefs.Save();
    }

    public void OnForgotPasswordClicked()
    {
        loginPanel.SetActive(false);
        forgotPasswordPanel.SetActive(true);
    }

    public void OnResetPasswordClicked()
    {
        string email = forgotPasswordEmailInput.text.Trim();
        if (!string.IsNullOrEmpty(email))
        {
            StartCoroutine(SendResetPasswordRequest(email));
        }
    }

    IEnumerator SendResetPasswordRequest(string email)
    {
        string url = "https://disertatie-backend.onrender.com/api/reset-password";
        string jsonData = JsonUtility.ToJson(new EmailOnlyPayload { email = email });

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            emailNotFoundText.SetActive(false);
            passwordResetMessage.SetActive(true);

        }
        else
        {
            if (request.responseCode == 404)
            {
                emailNotFoundText.SetActive(true);
                passwordResetMessage.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Server error la reset: " + request.downloadHandler.text);
            }
        }
    }

    public void OnBackButtonClicked()
    {
        emailNotFoundText.SetActive(false);
        passwordResetMessage.SetActive(false);
        forgotPasswordEmailInput.text = "";

        forgotPasswordPanel.SetActive(false);
        loginPanel.SetActive(true);

    }

    public void OnExitAppClicked()
    {
        Application.Quit();
    }

    public void OnDownloadAppClicked()
    {
        Application.OpenURL("https://drive.google.com/uc?export=download&id=1kfnjRKD7QGf4mIaktbxJ9V0mDyJiBUQS");
    }


}

[System.Serializable]
public class LoginPayload
{
    public string email;
    public string password;

    public LoginPayload(string email, string password)
    {
        this.email = email;
        this.password = password;
    }
}

[System.Serializable]
public class LoginResponse
{
    public string message;
    public string token;
    public string userId;
    public string email;
}

[System.Serializable]
public class TokenLoginPayload
{
    public string email;
    public string token;
}

[System.Serializable]
public class RegisterRequest
{
    public string email;
    public string password;
}

