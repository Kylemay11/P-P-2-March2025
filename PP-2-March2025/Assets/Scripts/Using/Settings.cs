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
    public AudioMixer audiomix;

    // resolution & fullscreen
    [SerializeField] public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    private List<Resolution> resolutionList;
    private float currentRefreshRate;
    private int currentResolutionIndex = 0;

    public Toggle fullScreenToggle;

    //fps
    public Toggle fpsDispalyToggle;
    public TextMeshProUGUI FpsText;
    private float pollingTime = 1f;
    private float time;
    private int frameCount;

    //sens
    public Slider sensSlider;
    public TextMeshProUGUI sensValueText;

    public void Start()
    {
        instance = this;
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
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(setRes);

        // fullscreen
        fullScreenToggle.isOn = Screen.fullScreen;
        fullScreenToggle.onValueChanged.AddListener(setFullscreen);

        //sens
        if (sensSlider != null && cameraComtroller.instance != null)
        {
            sensSlider.minValue = 100f;
            sensSlider.maxValue = 1000f;
            sensSlider.value = cameraComtroller.instance.sens;
            sensSlider.value = cameraComtroller.instance.sens;
            sensSlider.onValueChanged.AddListener(SetSensitivity);
        }
        if (sensValueText != null)
        {
            sensValueText.text = Mathf.RoundToInt(sensSlider.value).ToString();
        }
    }

    public void Update()
    {
        if (fpsDispalyToggle.isOn)
        {
           if (!FpsText.gameObject.activeSelf)
                FpsText.gameObject.SetActive(true);
            fpsCounter();
        }
        else
        {
            if (FpsText.gameObject.activeSelf)
            {
                FpsText.gameObject.SetActive(false);
            }
        }
    }

    void Awake()
    {
        
    }
    public void setAudio(float vol)
    {
        audiomix.SetFloat("volume", vol);
    }

    public void setRes(int resIndex)
    {
        Resolution resolution = resolutionList[resIndex];

        if (Screen.width != resolution.width || Screen.height != resolution.height)
        {
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }

    public void setQuality(int qual)
    {

    }

    public void setFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void fpsCounter()
    {
        gameManager.instance.menufpsDisplay.SetActive(true);
        time += Time.deltaTime;

        frameCount++;

        if (time >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / time);
            FpsText.text = frameRate.ToString() + " FPS";

            time -= pollingTime;
            frameCount = 0;
        }
    }

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

}
