using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


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
    public Button settingsButton;
    public Button saveButton;
    public Button exitButton;
    public Button returnButton;
    public Button backFromSettingsButton;
    public Button soundOnButton;
    public Button soundOffButton;

    private bool isMenuOpen = false;
    private AudioSource musicSource;

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
                settingsPanel.SetActive(false);
                buttonsPanel.SetActive(true);
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
        Debug.Log("TODO: Salvează progresul proiectului curent.");
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

}
