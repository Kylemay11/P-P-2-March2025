using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class Settings : MonoBehaviour
{
    public static Settings instance;
    //audio
    [SerializeField] Slider Audio;

    // resolution & fullscreen
    [SerializeField] public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    private List<Resolution> resolutionList;
    private float currentRefreshRate;
    private int currentResolutionIndex = 0;

    public Toggle fullScreenToggle;

    //fps
    //public TextMeshProUGUI FpsText;
    //private float pollingTime = 1f;
    //private float time;
    //private int frameCount;

    //sens
    public Slider sensSlider;
    public TextMeshProUGUI sensValueText;

    public void Start()
    {
        instance = this;

        // Audio
        if (!PlayerPrefs.HasKey("Volume"))
            PlayerPrefs.SetFloat("Volume", 1f);

        loadAudio(); // Load and apply volume
        Audio.value = PlayerPrefs.GetFloat("Volume");
        Audio.onValueChanged.AddListener(delegate { setAudio(); saveAudio(); });

        // resoultuion
        resolutions = Screen.resolutions;
        resolutionList = new List<Resolution>();
        resolutionDropdown.ClearOptions();
        currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if ((float)resolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                resolutionList.Add(resolutions[i]);
            }
        }

        resolutionList.Sort((a, b) => {
            if (a.width != b.width)
                return b.width.CompareTo(a.width);
            else
                return b.height.CompareTo(a.height);
        });

        List<string> options = new List<string>();
        for (int i = 0; i < resolutionList.Count; i++)
        {
            string resolutionOption = resolutionList[i].width + "x" + resolutionList[i].height + 
                " " + resolutionList[i].refreshRateRatio.value.ToString("0.##") + " Hz";
            options.Add(resolutionOption);

            if (resolutionList[i].width == Screen.width && resolutionList[i].height == 
                Screen.height && (float)resolutionList[i].refreshRateRatio.value == currentRefreshRate)
            {
                currentResolutionIndex = i;
            }
        }
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.RemoveAllListeners();
        resolutionDropdown.onValueChanged.AddListener(setRes);
        setRes(currentResolutionIndex);

        // fullscreen
        fullScreenToggle.isOn = Screen.fullScreen;
        fullScreenToggle.onValueChanged.AddListener(setFullscreen);

        // sens
        float savedSensitivity = PlayerPrefs.GetFloat("Sensitivity");
        if (sensSlider != null && cameraComtroller.instance != null)
        {
            sensSlider.minValue = 100f;
            sensSlider.maxValue = 1000f;
            sensSlider.value = savedSensitivity;
            cameraComtroller.instance.sens = savedSensitivity;
            sensSlider.onValueChanged.AddListener(OnSensitivityChanged);
        }
        if (sensValueText != null)
        {
            sensValueText.text = Mathf.RoundToInt(sensSlider.value).ToString();
        }
        
    }

    public void Update()
    {
        //if (FPSDisplay.Instance.fpsToggle.isOn)
        //{
        //   if (!FpsText.gameObject.activeSelf)
        //        FpsText.gameObject.SetActive(true);
        //    fpsCounter();
        //}
        //else
        //{
        //    if (FpsText.gameObject.activeSelf)
        //    {
        //        FpsText.gameObject.SetActive(false);
        //    }
        //}
    }

    void Awake()
    {
        
    }
    public void setAudio()
    {
        AudioListener.volume = Audio.value;
        
    }

    public void saveAudio()
    {
        PlayerPrefs.SetFloat("Volume", Audio.value);
        PlayerPrefs.Save();
    }
    public void loadAudio()
    {
        float volume = PlayerPrefs.GetFloat("Volume", 1f);
        Audio.value = volume;
        AudioListener.volume = volume;
    }

    public void setRes(int resIndex)
    {
        Resolution resolution = resolutionList[resIndex];

        
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolutionIndex", resIndex);
        PlayerPrefs.SetInt("IsFullscreen", fullScreenToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();

    }

    public void setQuality(int qual)
    {

    }

    public void setFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    //public void fpsCounter()
    //{
    //    gameManager.instance.menufpsDisplay.SetActive(true);
    //    time += Time.deltaTime;

    //    frameCount++;

    //    if (time >= pollingTime)
    //    {
    //        int frameRate = Mathf.RoundToInt(frameCount / time);
    //        FpsText.text = frameRate.ToString() + " FPS";

    //        time -= pollingTime;
    //        frameCount = 0;
    //    }
    //}

    public void SetSensitivity(float newSens)
    {
        if (cameraComtroller.instance != null)
        {
            cameraComtroller.instance.sens = newSens;
        }
        if (sensValueText != null)
        {
            sensValueText.text = Mathf.RoundToInt(newSens).ToString();
        }
    }
    public void OnSensitivityChanged(float newValue)
    {
        if (cameraComtroller.instance != null)
            cameraComtroller.instance.sens = newValue;

        sensValueText.text = Mathf.RoundToInt(newValue).ToString();
        PlayerPrefs.SetFloat("Sensitivity", newValue);
        PlayerPrefs.Save();
    }
}
