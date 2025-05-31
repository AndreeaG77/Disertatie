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
    private Button lastDeleteButton = null; 


    [Header("Change Password References")]
    public TMP_InputField oldPasswordInput;
    public TMP_InputField newPasswordInput;
    public TMP_InputField confirmPasswordInput;
    public GameObject wrongOldPasswordMessage;
    public GameObject newPasswordMismatchMessage;
    public GameObject changePasswordPopupPanel;

    [Header("New Project Panel")]
    public GameObject newProjectPanel;
    public TMP_InputField projectNameInput;
    public Button saveProjectButton;
    public GameObject enterNameText;



    void Start()
    {
        createProjectButton.onClick.AddListener(OnCreateNewProject);
        loadProjectButton.onClick.AddListener(OnLoadProjectClicked);
        logoutButton.onClick.AddListener(OnLogoutClicked);
        loadSelectedButton.onClick.AddListener(OnLoadSelectedProject);
        backButton.onClick.AddListener(OnBackClicked);
        //saveProjectButton.onClick.AddListener(OnCreateProjectConfirmed);

        loadSelectedButton.interactable = false;
        projectsPanel.SetActive(false);
        accountPanel.SetActive(false);
        changePasswordPanel.SetActive(false);
        deleteAccountPanel.SetActive(false);
    }

    public void OnCreateNewProject()
    {
        mainMenuPanel.SetActive(false);
        newProjectPanel.SetActive(true);

    }

    public void OnCreateProjectConfirmed()
    {
        string projectName = projectNameInput.text.Trim();
        if (string.IsNullOrEmpty(projectName))
        {
            enterNameText.SetActive(true);
            return;
        }

        PlayerPrefs.SetString("projectName", projectName);
        PlayerPrefs.Save();

        Debug.Log("Creating project: " + projectName);

        newProjectPanel.SetActive(false);
        projectNameInput.text = "";
        mainMenuPanel.SetActive(true);
        enterNameText.SetActive(false);
        PlayerPrefs.DeleteKey("loadedProjectData");
        PlayerPrefs.Save();

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
        StartCoroutine(FetchProjectsFromBackend());
    }

    IEnumerator FetchProjectsFromBackend()
    {
        string token = PlayerPrefs.GetString("sessionToken", "");
        string userId = "";

        UnityWebRequest validateReq = UnityWebRequest.Get("https://disertatie-backend.onrender.com/api/validate-token");
        validateReq.SetRequestHeader("Authorization", "Bearer " + token);
        yield return validateReq.SendWebRequest();

        if (validateReq.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<UserValidationResponse>(validateReq.downloadHandler.text);
            userId = response.user._id;
        }
        else
        {
            Debug.LogError("Token invalid sau expirat.");
            yield break;
        }

        string url = $"https://disertatie-backend.onrender.com/api/projects/{userId}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            foreach (Transform child in projectsContentPanel)
                Destroy(child.gameObject);

            ProjectBackendList backendList = JsonUtility.FromJson<ProjectBackendList>("{\"projects\":" + request.downloadHandler.text + "}");

            foreach (ProjectBackend proj in backendList.projects)
            {
                GameObject newBtn = Instantiate(projectButtonPrefab, projectsContentPanel);
                newBtn.GetComponentInChildren<TMP_Text>().text = proj.name;

                Button btnComponent = newBtn.GetComponent<Button>();
                Button deleteBtn = newBtn.transform.Find("DeleteButton").GetComponent<Button>();
                deleteBtn.gameObject.SetActive(false);
                string projectJson = JsonUtility.ToJson(proj);

                btnComponent.onClick.AddListener(() =>
                {
                    PlayerPrefs.SetString("loadedProjectData", projectJson);
                    SelectProject(proj.name, btnComponent, deleteBtn);
                });

                deleteBtn.onClick.AddListener(() =>
                {
                    StartCoroutine(DeleteProjectFromBackend(proj._id, newBtn));
                });
            }
        }
        else
        {
            Debug.LogError("Eroare la încărcarea proiectelor: " + request.downloadHandler.text);
        }
    }


    public void SelectProject(string projectId, Button btn, Button deleteBtn)
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

        if (lastDeleteButton != null)
            lastDeleteButton.gameObject.SetActive(false);

        deleteBtn.gameObject.SetActive(true);
        lastDeleteButton = deleteBtn;

        Debug.Log("Selected project: " + selectedProjectId);
    }

    IEnumerator DeleteProjectFromBackend(string projectId, GameObject buttonToRemove)
    {
        string token = PlayerPrefs.GetString("sessionToken", "");

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("No token found");
            yield break;
        }

        UnityWebRequest request = UnityWebRequest.Delete("https://disertatie-backend.onrender.com/api/projects/" + projectId);
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Project deleted successfully");
            Destroy(buttonToRemove);
        }
        else
        {
            Debug.LogWarning("Failed to delete project: " + request.downloadHandler.text);
        }
    }

    public void OnLoadSelectedProject()
    {
        if (!string.IsNullOrEmpty(selectedProjectId))
        {
            PlayerPrefs.SetString("projectName", selectedProjectId);
            PlayerPrefs.Save();
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
        string token = PlayerPrefs.GetString("sessionToken", "");
        string jsonData = JsonUtility.ToJson(new EmailOnlyPayload { email = newEmail });

        UnityWebRequest request = new UnityWebRequest("https://disertatie-backend.onrender.com/api/update-email", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Email updated successfully");
            UpdateEmailResponse response = JsonUtility.FromJson<UpdateEmailResponse>(request.downloadHandler.text);
            string newToken = response.token;

            PlayerPrefs.SetString("sessionEmail", newEmail);
            PlayerPrefs.SetString("sessionToken", newToken);
            if (PlayerPrefs.GetInt("rememberMe", 0) == 1)
            {
                PlayerPrefs.SetString("userEmail", newEmail);
                PlayerPrefs.SetString("userToken", newToken);
            }
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

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("JWT token is missing from PlayerPrefs.");
            yield break;
        }

        UnityWebRequest request = UnityWebRequest.Delete("https://disertatie-backend.onrender.com/api/delete-account");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        Debug.Log("Delete response code: " + request.responseCode);
        Debug.Log("UnityWebRequest result: " + request.result);


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

    public void OnChangePasswordCancelClicked()
    {
        changePasswordPanel.SetActive(false);
        accountPanel.SetActive(true);

        oldPasswordInput.text = "";
        newPasswordInput.text = "";
        confirmPasswordInput.text = "";
        wrongOldPasswordMessage.SetActive(false);
        newPasswordMismatchMessage.SetActive(false);
    }

    public void OnChangePasswordSaveClicked()
    {
        string oldPass = oldPasswordInput.text.Trim();
        string newPass = newPasswordInput.text.Trim();
        string confirmPass = confirmPasswordInput.text.Trim();

        wrongOldPasswordMessage.SetActive(false);
        newPasswordMismatchMessage.SetActive(false);

        if (newPass != confirmPass)
        {
            newPasswordMismatchMessage.SetActive(true);
            return;
        }

        StartCoroutine(ChangePasswordRequest(oldPass, newPass));
    }

    IEnumerator ChangePasswordRequest(string oldPassword, string newPassword)
    {
        string token = PlayerPrefs.GetString("sessionToken", "");

        ChangePasswordPayload payload = new ChangePasswordPayload
        {
            oldPassword = oldPassword,
            newPassword = newPassword
        };

        string jsonData = JsonUtility.ToJson(payload);

        UnityWebRequest request = new UnityWebRequest("https://disertatie-backend.onrender.com/api/change-password", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Password changed");

            ChangePasswordResponse response = JsonUtility.FromJson<ChangePasswordResponse>(request.downloadHandler.text);

            // Update PlayerPrefs
            PlayerPrefs.SetString("sessionToken", response.token);
            if (PlayerPrefs.GetInt("rememberMe", 0) == 1)
            {
                PlayerPrefs.SetString("userToken", response.token);
            }
            PlayerPrefs.Save();

            // UI Feedback
            changePasswordPanel.SetActive(false);
            wrongOldPasswordMessage.SetActive(false);
            newPasswordMismatchMessage.SetActive(false);
            accountPanel.SetActive(true);
            StartCoroutine(ShowPasswordChangedPopup());
        }
        else
        {
            if (request.responseCode == 401)
            {
                wrongOldPasswordMessage.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Password change failed: " + request.downloadHandler.text);
            }
        }
    }

    IEnumerator ShowPasswordChangedPopup()
    {
        changePasswordPopupPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        changePasswordPopupPanel.SetActive(false);
    }


}


[System.Serializable]
public class ProjectBackend
{
    public string _id;
    public string name;
    public ProjectSaveData data;
    public string userId;
}

[System.Serializable]
public class ProjectBackendList
{
    public List<ProjectBackend> projects;
}

[System.Serializable]
public class UserValidationResponse
{
    public UserInfo user;
}

[System.Serializable]
public class UserInfo
{
    public string _id;
    public string email;
}
