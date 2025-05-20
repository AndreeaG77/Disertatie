using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;



public class MainMenuUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject projectsPanel;

    [Header("Buttons")]
    public Button createProjectButton;
    public Button loadProjectButton;
    public Button logoutButton;
    public Button loadSelectedButton;
    public Button backButton;

    [Header("Projects")]
    public Transform projectsContentPanel;
    public GameObject projectButtonPrefab;

    private string selectedProjectId = null;
    private Button lastSelectedButton = null;

    [Header("Account")]
    public GameObject accountPanel;
    public GameObject changePasswordPanel;
    public GameObject deleteAccountPanel;

    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    private string originalEmail;
    private bool isEditingEmail = false;


    void Start()
    {
        createProjectButton.onClick.AddListener(OnCreateNewProject);
        loadProjectButton.onClick.AddListener(OnLoadProjectClicked);
        logoutButton.onClick.AddListener(OnLogoutClicked);
        loadSelectedButton.onClick.AddListener(OnLoadSelectedProject);
        backButton.onClick.AddListener(OnBackClicked);

        loadSelectedButton.interactable = false;
        projectsPanel.SetActive(false);
        accountPanel.SetActive(false);
        changePasswordPanel.SetActive(false);
        deleteAccountPanel.SetActive(false);
    }

    public void OnCreateNewProject()
    {
        SceneManager.LoadScene("DefaultScene");
    }

    public void OnLogoutClicked()
    {
        PlayerPrefs.DeleteKey("sessionEmail");
        PlayerPrefs.DeleteKey("sessionToken");
        PlayerPrefs.Save();
        SceneManager.LoadScene("LoginPage");
    }

    public void OnLoadProjectClicked()
    {
        mainMenuPanel.SetActive(false);
        projectsPanel.SetActive(true);
        LoadProjects();
    }

    public void LoadProjects()
    {
        List<string> projectNames = new List<string> { "Living Room A", "Modern Kitchen", "Studio Setup" };

        foreach (Transform child in projectsContentPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (string projectName in projectNames)
        {
            GameObject newBtn = Instantiate(projectButtonPrefab, projectsContentPanel);
            newBtn.GetComponentInChildren<TMP_Text>().text = projectName;

            Button btnComponent = newBtn.GetComponent<Button>();
            string thisProjectId = projectName;

            btnComponent.onClick.AddListener(() =>
            {
                SelectProject(thisProjectId, btnComponent);
            });
        }
    }

    public void SelectProject(string projectId, Button btn)
    {
        selectedProjectId = projectId;
        loadSelectedButton.interactable = true;

        if (lastSelectedButton != null)
        {
            Image lastImage = lastSelectedButton.GetComponent<Image>();
            lastImage.color = Color.white;
        }

        Image currentImage = btn.GetComponent<Image>();
        currentImage.color = new Color(0.7f, 0.9f, 1f);

        lastSelectedButton = btn;

        Debug.Log("Selected project: " + selectedProjectId);
    }


    public void OnLoadSelectedProject()
    {
        if (!string.IsNullOrEmpty(selectedProjectId))
        {
            Debug.Log("Loading project: " + selectedProjectId);
            SceneManager.LoadScene("DefaultScene");
        }
    }

    public void OnBackClicked()
    {
        projectsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OnOpenAccountPanel()
    {
        mainMenuPanel.SetActive(false);
        accountPanel.SetActive(true);

        originalEmail = PlayerPrefs.GetString("sessionEmail", "");
        emailInput.text = originalEmail;
        emailInput.interactable = false;

        passwordInput.text = "********";
        passwordInput.interactable = false;

        isEditingEmail = false;
    }

    public void OnChangeEmailClicked()
    {
        emailInput.interactable = true;
        isEditingEmail = true;
    }

    public void OnChangePasswordClicked()
    {
        accountPanel.SetActive(false);
        changePasswordPanel.SetActive(true);
    }

    public void OnDeleteAccountClicked()
    {
        accountPanel.SetActive(false);
        deleteAccountPanel.SetActive(true);
    }

    public void OnAccountSaveClicked()
    {
        accountPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        if (isEditingEmail && emailInput.text != originalEmail)
        {
            StartCoroutine(UpdateEmailRequest(emailInput.text));
        }

        isEditingEmail = false;
    }

    public void OnAccountCancelClicked()
    {
        emailInput.text = originalEmail;
        emailInput.interactable = false;

        passwordInput.text = "********";
        passwordInput.interactable = false;

        isEditingEmail = false;

        accountPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    IEnumerator UpdateEmailRequest(string newEmail)
    {
        string token = PlayerPrefs.GetString("userToken", "");
        string jsonData = JsonUtility.ToJson(new EmailOnlyPayload { email = newEmail });

        UnityWebRequest request = new UnityWebRequest("http://localhost:3000/api/update-email", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Email updated successfully");
            PlayerPrefs.SetString("sessionEmail", newEmail);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Failed to update email: " + request.downloadHandler.text);
        }
    }

    public void OnDeleteCancelClicked()
    {
        deleteAccountPanel.SetActive(false);
        accountPanel.SetActive(true);
    }

    public void OnDeleteConfirmClicked()
    {
        StartCoroutine(DeleteAccountRequest());
    }

    IEnumerator DeleteAccountRequest()
    {
        string token = PlayerPrefs.GetString("sessionToken", "");

        UnityWebRequest request = UnityWebRequest.Delete("http://localhost:3000/api/delete-account");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Account deleted successfully");

            PlayerPrefs.DeleteKey("userEmail");
            PlayerPrefs.DeleteKey("userToken");
            PlayerPrefs.DeleteKey("sessionEmail");
            PlayerPrefs.DeleteKey("sessionToken");
            PlayerPrefs.SetInt("rememberMe", 0);
            PlayerPrefs.Save();

            SceneManager.LoadScene("LoginPage");
        }
        else
        {
            Debug.LogWarning("Failed to delete account: " + request.downloadHandler.text);
        }
    }


}
