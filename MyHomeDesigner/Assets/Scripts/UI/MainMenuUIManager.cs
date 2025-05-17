using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

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

    void Start()
    {
        createProjectButton.onClick.AddListener(OnCreateNewProject);
        loadProjectButton.onClick.AddListener(OnLoadProjectClicked);
        logoutButton.onClick.AddListener(OnLogoutClicked);
        loadSelectedButton.onClick.AddListener(OnLoadSelectedProject);
        backButton.onClick.AddListener(OnBackClicked);

        loadSelectedButton.interactable = false;
        projectsPanel.SetActive(false);
    }

    public void OnCreateNewProject()
    {
        SceneManager.LoadScene("DefaultScene"); // ← înlocuiește cu numele corect
    }

    public void OnLogoutClicked()
    {
        SceneManager.LoadScene("LoginPage"); // ← înlocuiește cu numele corect
    }

    public void OnLoadProjectClicked()
    {
        mainMenuPanel.SetActive(false);
        projectsPanel.SetActive(true);
        LoadProjects();
    }

    public void LoadProjects()
    {
        // Simulăm niște proiecte (vezi cum se poate face legătura cu contul activ ulterior)
        List<string> projectNames = new List<string> { "Living Room A", "Modern Kitchen", "Studio Setup"};

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

            btnComponent.onClick.AddListener(() => {
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
        currentImage.color = new Color(0.7f, 0.9f, 1f); // albastru deschis

        lastSelectedButton = btn;

        Debug.Log("Selected project: " + selectedProjectId);
    }


    public void OnLoadSelectedProject()
    {
        if (!string.IsNullOrEmpty(selectedProjectId))
        {
            Debug.Log("Loading project: " + selectedProjectId);
            // Aici o să încarci efectiv proiectul bazat pe ID-ul/numelui
            SceneManager.LoadScene("DefaultWorkScene"); // Simulat
        }
    }

    public void OnBackClicked()
    {
        projectsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
