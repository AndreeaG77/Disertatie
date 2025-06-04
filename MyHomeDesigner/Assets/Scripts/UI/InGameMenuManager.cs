using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;


public class InGameMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject buttonsPanel;
    public GameObject settingsPanel;

    [Header("Audio Settings")]
    public Slider volumeSlider;
    public TMP_Text volumeValueText;
    //public GameObject soundOnIcon;
    //public GameObject soundOffIcon;

    [Header("Buttons")]
    public Button userGuideButton;
    public Button settingsButton;
    public Button saveButton;
    public Button exitButton;
    public Button returnButton;
    public Button backFromSettingsButton;
    public Button soundOnButton;
    public Button soundOffButton;
    public Button roomTotalButton;
    public Button hideTotalButton;

    private bool isMenuOpen = false;
    private AudioSource musicSource;

    [Header("Save Project")]
    public GameObject saveProjectPanel;
    public TMP_InputField projectNameInput;
    public GameObject savedText;
    public Button changeNameButton;

    //private bool isEditingProjectName = false;

    public RoomTotalManager roomTotalManager;

    [Header("User Guide")]
    public GameObject userGuidePanelStart;
    public GameObject userGuidePanelMenu;
    public GameObject closeUserGuideButton;
    public GameObject backFromUserGuideButton;


    void Start()
    {
        settingsButton.onClick.AddListener(OnSettingsClicked);
        saveButton.onClick.AddListener(OnSaveProgressClicked);
        exitButton.onClick.AddListener(OnExitProjectClicked);
        returnButton.onClick.AddListener(OnReturnToProjectClicked);
        backFromSettingsButton.onClick.AddListener(OnBackFromSettingsClicked);
        soundOnButton.onClick.AddListener(OnSoundOnIconClicked);
        soundOffButton.onClick.AddListener(OnSoundOffIconClicked);

        volumeSlider.onValueChanged.AddListener(UpdateVolumeUI);

        menuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        buttonsPanel.SetActive(true);
        userGuidePanelStart.SetActive(true);
        userGuidePanelMenu.SetActive(false);
        saveProjectPanel.SetActive(false);

        GameObject musicManager = GameObject.Find("MusicManager");
        if (musicManager != null)
            musicSource = musicManager.GetComponent<AudioSource>();

        if (musicSource != null)
        {
            float initialVolume = musicSource.volume;
            initialVolume = initialVolume * 100f;
            volumeSlider.value = initialVolume;
            UpdateVolumeUI(initialVolume);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isMenuOpen = !isMenuOpen;
            menuPanel.SetActive(isMenuOpen);

            if (isMenuOpen)
            {
                GameObject existingPanel = GameObject.FindWithTag("EditorOnly");
                if (existingPanel != null)
                {
                    FurnitureManipulator.Instance.ClearMode();
                    Destroy(existingPanel);
                }

                settingsPanel.SetActive(false);
                buttonsPanel.SetActive(true);
                userGuidePanelMenu.SetActive(false);
                userGuidePanelStart.SetActive(false);
                saveProjectPanel.SetActive(false);

                roomTotalManager.totalsAreShown = true;
                roomTotalButton.gameObject.SetActive(false);
                hideTotalButton.gameObject.SetActive(false);

                UIBlocker.IsUIBlockingFurniture = true;
            }
            else
            {
                if (ViewState.CurrentMode == ViewMode.Mode2D && roomTotalManager.totalsAreShown)
                {
                    roomTotalManager.totalsAreShown = false;
                    roomTotalButton.gameObject.SetActive(true);
                }
                UIBlocker.IsUIBlockingFurniture = false;

            }
        }
    }


    public void OnReturnToProjectClicked()
    {
        menuPanel.SetActive(false);
        isMenuOpen = false;
    }

    public void OnExitProjectClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnSaveProgressClicked()
    {
        string currentName = PlayerPrefs.GetString("projectName", "Unnamed Project");

        projectNameInput.text = currentName;
        projectNameInput.interactable = false;

        buttonsPanel.SetActive(false);
        saveProjectPanel.SetActive(true);
        savedText.SetActive(false);

    }

    public List<SceneObjectData> CollectAllSceneObjects()
    {
        List<SceneObjectData> dataList = new List<SceneObjectData>();

        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("FurnitureItemPrefab"))
                continue;
            if (obj.name.Contains("RoomButtonPrefab"))
                continue;
            if (obj.layer == LayerMask.NameToLayer("UI"))
                continue;
            if (obj.scene.IsValid() && obj.scene.isLoaded && IsSavable(obj))
            {
                dataList.Add(new SceneObjectData
                {
                    prefabName = obj.name.Replace("(Clone)", "").Trim(),
                    position = obj.transform.position,
                    rotation = obj.transform.rotation,
                    scale = obj.transform.localScale
                });
            }
        }

        return dataList;
    }

    private bool IsSavable(GameObject obj)
    {
        string[] ignored = { "Canvas", "EventSystem", "Main Camera", "Directional Light", "GridManager", "GridLines", "MusicManager" };
        return !ignored.Contains(obj.name) && obj.name.EndsWith("(Clone)");
    }



    public void OnChangeProjectNameClicked()
    {
        projectNameInput.interactable = true;
        //isEditingProjectName = true;
    }

    public void OnBackFromSaveProject()
    {
        saveProjectPanel.SetActive(false);
        savedText.SetActive(false);
        buttonsPanel.SetActive(true);
    }

    public void OnSaveProjectNameConfirmed()
    {
        string newName = projectNameInput.text.Trim();

        if (!string.IsNullOrEmpty(newName))
        {
            PlayerPrefs.SetString("projectName", newName);
            PlayerPrefs.Save();
            Debug.Log("Saved project name: " + newName);
        }

        savedText.SetActive(true);

        ProjectSaveData saveData = new ProjectSaveData
        {
            projectName = newName,
            objects = CollectAllSceneObjects()
        };

        ProjectWrapper wrapper = new ProjectWrapper
        {
            name = newName,
            data = saveData
        };

        string json = JsonUtility.ToJson(wrapper, true);

        StartCoroutine(SendProjectToBackend(json));
    }

    IEnumerator SendProjectToBackend(string json)
    {
        //string url = "http://localhost:3000/api/projects/save";
        string url = "https://disertatie-backend.onrender.com/api/projects/save";
        string token = PlayerPrefs.GetString("sessionToken", "");

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Project saved to backend.");
        }
        else
        {
            Debug.LogWarning("Failed to save project: " + request.downloadHandler.text);
        }
    }


    public void OnSettingsClicked()
    {
        buttonsPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnBackFromSettingsClicked()
    {
        settingsPanel.SetActive(false);
        buttonsPanel.SetActive(true);
    }

    public void OnSoundOnIconClicked()
    {
        volumeSlider.value = 0f;
        UpdateVolumeUI(0f);
        soundOnButton.gameObject.SetActive(false);
        soundOffButton.gameObject.SetActive(true);
    }

    public void OnSoundOffIconClicked()
    {
        volumeSlider.value = 50f;
        UpdateVolumeUI(50f);
        soundOffButton.gameObject.SetActive(false);
        soundOnButton.gameObject.SetActive(true);
    }

    private void UpdateVolumeUI(float value)
    {
        int percent = Mathf.RoundToInt(value);
        volumeValueText.text = percent.ToString();

        bool isMuted = percent == 0;
        soundOnButton.gameObject.SetActive(!isMuted);
        soundOffButton.gameObject.SetActive(isMuted);

        if (musicSource != null)
            musicSource.volume = value / 100f;
    }

    public void OnCloseUserGuideStartClicked()
    {
        userGuidePanelStart.SetActive(false);
    }

    public void OnUserGuideClicked()
    {
        buttonsPanel.SetActive(false);
        userGuidePanelMenu.SetActive(true);
    }

    public void OnBackFromUserGuideClicked()
    {
        userGuidePanelMenu.SetActive(false);
        buttonsPanel.SetActive(true);
    }

}

[System.Serializable]
public class ProjectWrapper
{
    public string name;
    public ProjectSaveData data;
}
