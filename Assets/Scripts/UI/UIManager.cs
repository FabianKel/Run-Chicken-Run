using UnityEngine;
using TMPro; // Si usas TextMeshPro


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TMP_Text seedText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void UpdateSeedUI(int collected, int total)
    {
        if (seedText != null)
        {
            seedText.text = collected + "/" + total;
        }
    }
}