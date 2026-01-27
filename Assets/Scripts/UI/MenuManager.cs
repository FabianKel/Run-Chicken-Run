using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Paneles")]
    public GameObject menuPanel;
    public GameObject winPanel;
    public GameObject gameHUD;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ShowMenu();
    }

    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        winPanel.SetActive(false);
        gameHUD.SetActive(false);
        Time.timeScale = 0f;
    }

    public void PlayGame()
    {
        menuPanel.SetActive(false);
        gameHUD.SetActive(true);
        Time.timeScale = 1f;
    }

    public void ShowWinScreen()
    {
        winPanel.SetActive(true);
        gameHUD.SetActive(false);
        Time.timeScale = 0f;
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}