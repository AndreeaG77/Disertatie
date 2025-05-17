using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoginRegisterUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;

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
        string email = loginEmailInput.text.Trim();
        string password = loginPasswordInput.text;

        // TODO: Aici se va verifica dacă există contul în DB
        bool credentialsMatch = true; // înlocuit mai târziu cu validare reală

        if (credentialsMatch)
        {
            SceneManager.LoadScene("MainMenu"); // sau numele scenei tale
        }
        else
        {
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

        // TODO: Înlocuiește cu validare reală din DB
        bool emailAlreadyExists = false; // mock
        bool passwordsMatch = password == confirmPassword;

        if (emailAlreadyExists)
        {
            registerErrorEmailExistsText.SetActive(true);
        }
        else if (!passwordsMatch)
        {
            registerErrorPasswordMismatchText.SetActive(true);
        }
        else
        {
            // TODO: Creare cont în baza de date

            // Schimb paneluri
            registerPanel.SetActive(false);
            loginPanel.SetActive(true);

            // Afișare popup
            StartCoroutine(ShowSuccessPopup());
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
        //rememberMeToggle.isOn = !rememberMeToggle.isOn;
        Debug.Log("Remember me: " + rememberMeToggle.isOn);
    }

    public void OnForgotPasswordClicked()
    {
        Debug.Log("Forgot password clicked");
    }
}
