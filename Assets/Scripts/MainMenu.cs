using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class MainMenu : MonoBehaviour
{

    public GameObject panel;
    public Slider slider;
    
    public GameObject OptionsPanel;
    public AudioMixer audioMixer;
    public TMP_Dropdown Resolutions;
    private int CurrentResolution; 
    Resolution[] resolutions;

    public AudioSource Themeaudio;

    public GameObject SelectTeam;
    public GameObject MainMenuCanvas;

    public class MySettings
    {
        public Resolution resolution;
        public bool Windowed;
        public int GraphicsIndex;
        public float Volume;
    }
    public AudioClip audioClip;
    #region Singelton
    public static MySettings mySettings = new MySettings();
    #endregion

    public void Start()
    {
        PlayerPrefs.DeleteAll();
        Themeaudio.Play();
       // SelectTeam.enabled = false;
        SceneManager.UnloadSceneAsync(1);
       // Resources.UnloadUnusedAssets();
        resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        for(int i=0; i<resolutions.Length;i++)
        {
            if(resolutions[i].Equals(Screen.currentResolution))
            {
                CurrentResolution = i;
            }
            options.Add(resolutions[i].width + " X " + resolutions[i].height);
        }
        Resolutions.ClearOptions();
        Resolutions.AddOptions(options);
        Resolutions.value = CurrentResolution;
        Resolutions.RefreshShownValue();
    }

    void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        Debug.Log("Start Game");
       //AAAAAD MainMenuCanvas.SetActive(false);
        SelectTeam.SetActive(true);
        Debug.Log("Team Select");
        
    }

    public void CTSelected()
    {
        PlayerPrefs.SetString("WhoAmI", "CT");
        panel.SetActive(true);
        StartCoroutine(LoadLevel(1));
        SelectTeam.SetActive(false);
    }
    public void TSelected()
    {
        PlayerPrefs.SetString("WhoAmI", "T");
        panel.SetActive(true);
        StartCoroutine(LoadLevel(1));
        SelectTeam.SetActive(false);
    }


    IEnumerator LoadLevel(int index)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress);
            Debug.Log(progress);
            slider.value = progress;

            yield return null;
        }
    }

    public void Options()
    {
        Debug.Log("Options Menu");
        OptionsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        Debug.Log("To Main Menu From Options");
        OptionsPanel.SetActive(false);
    }
    public void OnVolumeChange(float Value)
    {
        //audioMixer.SetFloat("Volume", Value);
        //float temp;
        //audioMixer.GetFloat("Volume",out temp);
        //Debug.Log(temp);
        mySettings.Volume = Value;
    }
    public void OnGraphicsChanged(int QualityIndex)
    {
        //QualitySettings.SetQualityLevel(QualityIndex);
        mySettings.GraphicsIndex = QualityIndex;
    }
    public void OnWindowedChanged(bool Value)
    {
        //Screen.fullScreen = Value;
        mySettings.Windowed = Value;
    }
    public void OnResolutionChanged(int ResoutionIndex)
    {
        //Screen.SetResolution(resolutions[ResoutionIndex].width, resolutions[ResoutionIndex].height, Screen.fullScreen);
        //Debug.Log(" OnResolutionChanged "+resolutions[ResoutionIndex]);
        mySettings.resolution = resolutions[ResoutionIndex];
    }

    public void ApplySettings()
    {
        Screen.SetResolution(mySettings.resolution.width,mySettings.resolution.height, !mySettings.Windowed);
        Debug.Log(Screen.currentResolution);

        QualitySettings.SetQualityLevel(mySettings.GraphicsIndex);
        Debug.Log(QualitySettings.GetQualityLevel());

        audioMixer.SetFloat("Volume", mySettings.Volume);
        float temp;
        audioMixer.GetFloat("Volume", out temp);
        Debug.Log(temp);
    }
    
}
