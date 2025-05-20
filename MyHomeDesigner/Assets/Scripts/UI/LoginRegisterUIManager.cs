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

    [Header("Login Options")]
    public Toggle rememberMeToggle;

    [Header("Register References")]
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerConfirmPasswordInput;
    public GameObject registerErrorEmailExistsText;
    public GameObject registerErrorPasswordMismatchText;

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

        if (PlayerPrefs.GetInt("rememberMe", 0) == 1)
        {
            string savedEmail = PlayerPrefs.GetString("userEmail");
            loginEmailInput.text = savedEmail;
            loginPasswordInput.text = "********";
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

        UnityWebRequest request = new UnityWebRequest("http://localhost:3000/api/login", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {

            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            Debug.Log("JWT Token: " + response.token);

            if (rememberMeToggle.isOn)
            {
                PlayerPrefs.SetString("userEmail", email);
                PlayerPrefs.SetString("userToken", response.token);
                PlayerPrefs.SetInt("rememberMe", 1);
                PlayerPrefs.Save();
            }

            PlayerPrefs.SetString("sessionEmail", email);
            PlayerPrefs.SetString("sessionToken", response.token);
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
        string url = "http://localhost:3000/api/token-login";
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


    // ==== Register ====
    public void OnSignupClicked()
    {
        string email = registerEmailInput.text.Trim();
        string password = registerPasswordInput.text;
        string confirmPassword = registerConfirmPasswordInput.text;

        // Reset erori
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
        string url = "http://localhost:3000/api/register";

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

    // ==== Placeholder pentru alte butoane ====
    public void OnRememberMeToggled()
    {
        if (rememberMeToggle.isOn)
        {
            //Debug.Log("remember me");
            PlayerPrefs.SetInt("rememberMe", 1);
        }
        else
        {
            //Debug.Log("forget");
            PlayerPrefs.DeleteKey("userEmail");
            PlayerPrefs.DeleteKey("userToken");
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
        string url = "http://localhost:3000/api/reset-password";
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

            submitButton.SetActive(false);
            backButton.SetActive(true);
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
        submitButton.SetActive(true);
        backButton.SetActive(false);
        forgotPasswordEmailInput.text = "";

        forgotPasswordPanel.SetActive(false);
        loginPanel.SetActive(true);

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

