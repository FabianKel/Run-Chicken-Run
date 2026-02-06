using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    private static bool shouldAutoStart = false;

    private bool hasGameStartedInThisScene = false;

    [Header("HUD & Gameplay")]
    [SerializeField] private GameObject gameHUD;
    [SerializeField] private TMP_Text seedText;

    [Header("Menús Principales")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject lostPanel;

    private GameObject currentPanel;
    private GameObject previousPanel;
    private bool isGameActive = false;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        if (shouldAutoStart)
        {
            PlayGame();
            shouldAutoStart = false;
        }
        else
        {
            ShowMainMenu();
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && isGameActive)
        {
            if (pausePanel.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // LÓGICA DE NAVEGACIÓN
    public void ShowMainMenu()
    {
        isGameActive = false;
        Time.timeScale = 1f;
        SwitchPanel(mainMenuPanel);
        gameHUD.SetActive(false);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResumeBackgroundMusic();
        }
    }

    public void PlayGame()
    {
        if (hasGameStartedInThisScene)
        {

            shouldAutoStart = true;
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        isGameActive = true;
        hasGameStartedInThisScene = true;

        Time.timeScale = 1f;
        SwitchPanel(null);
        gameHUD.SetActive(true);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        SwitchPanel(pausePanel);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        SwitchPanel(null);
        gameHUD.SetActive(true);
    }

    public void ShowWinScreen()
    {
        isGameActive = false;
        Time.timeScale = 0f;
        SwitchPanel(winPanel);
        gameHUD.SetActive(false);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayWinSequence();
        }
    }

    public void ShowLostScreen()
    {
        isGameActive = false;
        Time.timeScale = 0f;
        SwitchPanel(lostPanel);
        gameHUD.SetActive(false);
    }

    // SETTINGS
    public void OpenSettings()
    {
        if (mainMenuPanel.activeSelf) previousPanel = mainMenuPanel;
        else if (pausePanel.activeSelf) previousPanel = pausePanel;

        if (previousPanel == null) previousPanel = mainMenuPanel;

        SwitchPanel(settingsPanel);
    }

    public void CloseSettings()
    {
        SwitchPanel(previousPanel);
    }


    private void SwitchPanel(GameObject panelToShow)
    {
        mainMenuPanel.SetActive(false);
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        winPanel.SetActive(false);
        lostPanel.SetActive(false);

        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
            currentPanel = panelToShow;
        }
    }

    public void UpdateSeedUI(int collected, int total)
    {
        if (seedText != null)
        {
            seedText.text = $"{collected} / {total}";
        }
    }



    public void PlayAgain()
    {
        shouldAutoStart = true;

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenuReset()
    {
        shouldAutoStart = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    public void SetMasterVolume(float volume)
    {
        // AudioListener.volume = volume; // Simple
        // O usar AudioMixer si tienes uno configurado
    }
}