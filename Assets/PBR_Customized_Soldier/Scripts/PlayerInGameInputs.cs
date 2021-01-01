using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;



public class PlayerInGameInputs : MonoBehaviour
{
    public GameObject PlayerReference;
    public GameObject PauseCanvasGO;
    public Canvas Crosshair;
    public Canvas HUD;
    private Image panel;
    public bool IsPaused = false;
    public GameObject OptionPanel;
    public AudioMixer audioMixer;
    public TMP_Dropdown Resolutions;
    private int CurrentResolution;
    Resolution[] resolutions;

    public class MySettings
    {
        public Resolution resolution;
        public bool Windowed;
        public int GraphicsIndex;
        public float Volume;
    }
    #region Singelton
    public static MySettings mySettings = new MySettings();
    #endregion

    private bool cursorvisible;
    private CursorLockMode cursorlockstate;

    // Start is called before the first frame update
    void Start()
    {
        IsPaused = false;
        PauseCanvasGO.SetActive(false);
        panel = PauseCanvasGO.GetComponentInChildren<Image>();
        panel.CrossFadeAlpha(0, 0, true);
        resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].Equals(Screen.currentResolution))
            {
                CurrentResolution = i;
            }
            options.Add(resolutions[i].width + " X " + resolutions[i].height);
        }
        Resolutions.ClearOptions();
        Resolutions.AddOptions(options);
        Resolutions.value = CurrentResolution;
        Resolutions.RefreshShownValue();

        cursorvisible = Cursor.visible;
        cursorlockstate = Cursor.lockState;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (IsPaused)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Debug.Log(Cursor.visible+"  "+Cursor.lockState);

                PlayerReference.SetActive(true);
                PauseCanvasGO.SetActive(false);
                OptionPanel.SetActive(false);
                panel.CrossFadeAlpha(0, 0.5f, true);
                Time.timeScale = 1;
                IsPaused = !IsPaused;

                Crosshair.enabled = true;
                HUD.enabled = true;
            }
            else
            {
                // PlayerReference.SetActive(false);
                PauseCanvasGO.SetActive(true);
                panel.CrossFadeAlpha(1, 0.5f, true);
                Time.timeScale = 0;
                IsPaused = !IsPaused;

                
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                Crosshair.enabled = false;
                HUD.enabled = false;
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void OnExitToMainMenuClicked()
    {
        Time.timeScale = 1;
       
        SceneManager.LoadSceneAsync(0);
        
        
    }
    public void OnOptionClicked()
    {
        
        OptionPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
 
        OptionPanel.SetActive(false);
        
        
    }
    public void OnVolumeChange(float Value)
    {
        mySettings.Volume = Value;
    }
    public void OnGraphicsChanged(int QualityIndex)
    {
        mySettings.GraphicsIndex = QualityIndex;
    }
    public void OnWindowedChanged(bool Value)
    {
        mySettings.Windowed = Value;
    }
    public void OnResolutionChanged(int ResoutionIndex)
    {   
        mySettings.resolution = resolutions[ResoutionIndex];
    }

    public void ApplySettings()
    {
        Screen.SetResolution(mySettings.resolution.width, mySettings.resolution.height, !mySettings.Windowed);
        Debug.Log(Screen.currentResolution);

        QualitySettings.SetQualityLevel(mySettings.GraphicsIndex);
        Debug.Log(QualitySettings.GetQualityLevel());

        audioMixer.SetFloat("Volume", mySettings.Volume);
        float temp;
        audioMixer.GetFloat("Volume", out temp);
        Debug.Log(temp);
    }
}
