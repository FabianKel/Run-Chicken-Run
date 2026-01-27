using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    #region Referencias y Configuración

    [Header("Referencias Globales")]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform directionalLight;
    [SerializeField] private GameObject player;

    [Header("Configuración de Niveles")]
    public int currentLevel = 1;
    public List<LevelConfig> levels;

    [Header("Ajustes de Transición")]
    [SerializeField] private float transitionSpeed = 2f;
    [SerializeField] private Vector3 cameraOffsetPerLevel = new Vector3(0, 0, 40);

    [Header("Respawn y Efectos")]
    [SerializeField] private GameObject deathOverlay;
    [SerializeField] private float overlayDuration = 0.5f;

    private Vector3 currentLevelSpawnPoint;


    private int seedsInCurrentLevel;
    private bool isTransitioning = false;


    // UI
    private int totalSeedsInLevel;

    #endregion

    #region Estructuras de Datos

    [System.Serializable]
    public class LevelConfig
    {
        public string levelName;
        public GameObject levelParent;
        public GameObject door;
        public GameObject doorCollision;
        public GameObject levelCamera;

        [HideInInspector] public Transform exitTrigger;
        [HideInInspector] public Transform nextLevelSpawn;
    }

    #endregion

    #region Ciclo de Vida (Unity)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SetupLevelReferences();
    }

    void Start()
    {
        PrepareLevel();
        SetCurrentSpawn(player.transform.position);
    }

    #endregion

    #region Lógica de Inicialización

    void SetupLevelReferences()
    {
        foreach (var level in levels)
        {
            if (level.levelParent != null)
            {
                level.exitTrigger = level.levelParent.transform.Find("ExitTrigger");
                level.nextLevelSpawn = level.levelParent.transform.Find("NextSpawn");

                if (level.exitTrigger == null || level.nextLevelSpawn == null)
                {
                    Debug.LogWarning($"[LevelManager] Faltan hijos en {level.levelName}. Revisa 'ExitTrigger' y 'NextSpawn'.");
                }
            }
        }
    }
    public void SetCurrentSpawn(Vector3 newSpawn)
    {
        currentLevelSpawnPoint = newSpawn;
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        if (deathOverlay != null) deathOverlay.SetActive(true);

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.position = currentLevelSpawnPoint;

        if (cc != null) cc.enabled = true;

        yield return new WaitForSeconds(overlayDuration);

        if (deathOverlay != null) deathOverlay.SetActive(false);
    }

    public void PrepareLevel()
    {
        if (currentLevel > levels.Count) return;

        isTransitioning = false;

        SwitchCamera();

        Transform currentParent = levels[currentLevel - 1].levelParent.transform;

        GameObject[] allSeeds = GameObject.FindGameObjectsWithTag("Seed");
        seedsInCurrentLevel = 0;



        foreach (GameObject seed in allSeeds)
        {
            if (seed.transform.IsChildOf(currentParent))
            {
                seedsInCurrentLevel++;
                SetCurrentSpawn(levels[currentLevel - 1].nextLevelSpawn.position);
            }
        }

        // UI
        totalSeedsInLevel = seedsInCurrentLevel;
        UIManager.Instance.UpdateSeedUI(0, totalSeedsInLevel);


        Debug.Log($"<color=green>Nivel {currentLevel} iniciado.</color> Semillas: {seedsInCurrentLevel}");
    }

    #endregion

    #region Lógica de Juego (Semillas y Puertas)

    public void SeedCollected()
    {
        if (seedsInCurrentLevel <= 0) return;

        seedsInCurrentLevel--;
        Debug.Log($"Semilla recogida. Quedan: {seedsInCurrentLevel}");
        // UI
        int collected = totalSeedsInLevel - seedsInCurrentLevel;
        UIManager.Instance.UpdateSeedUI(collected, totalSeedsInLevel);

        if (seedsInCurrentLevel <= 0)
        {
            OpenCurrentDoor();
        }
    }

    void OpenCurrentDoor()
    {
        if (levels[currentLevel - 1].door != null)
            StartCoroutine(RotateDoor(levels[currentLevel - 1].door.transform));
    }

    IEnumerator RotateDoor(Transform doorTransform)
    {
        float elapsed = 0;
        Quaternion startRot = doorTransform.localRotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, -100, 0);

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * 2f;
            doorTransform.localRotation = Quaternion.Slerp(startRot, endRot, elapsed);
            yield return null;
        }
    }

    #endregion

    #region Lógica de Transición de Nivel

    public void GoToNextLevel()
    {
        if (isTransitioning) return;

        isTransitioning = true;

        Debug.Log("Iniciando transición de nivel...");
        StartCoroutine(LevelTransitionRoutine());
    }

    IEnumerator LevelTransitionRoutine()
    {
        Vector3 startCamPos = mainCamera.position;
        Vector3 startLightPos = directionalLight.position;

        Vector3 targetCamPos = startCamPos + cameraOffsetPerLevel;
        Vector3 targetLightPos = startLightPos + cameraOffsetPerLevel;

        float elapsed = 0;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * transitionSpeed;
            float t = Mathf.SmoothStep(0, 1, elapsed);

            mainCamera.position = Vector3.Lerp(startCamPos, targetCamPos, t);
            directionalLight.position = Vector3.Lerp(startLightPos, targetLightPos, t);

            yield return null;
        }

        if (levels[currentLevel - 1].doorCollision != null)
            levels[currentLevel - 1].doorCollision.SetActive(true);

        currentLevel++;

        if (currentLevel <= levels.Count)
        {
            PrepareLevel();
        }
        else
        {
            Debug.Log("<color=yellow>¡JUEGO COMPLETADO!</color>");
            MenuManager.Instance.ShowWinScreen();
        }
    }

    private void SwitchCamera()
    {
        foreach (var config in levels)
        {
            if (config.levelCamera != null)
                config.levelCamera.SetActive(false);
        }

        GameObject activeCam = levels[currentLevel - 1].levelCamera;
        if (activeCam != null)
        {
            activeCam.SetActive(true);
            Debug.Log($"Cámara activada: {activeCam.name}");
        }
    }

    public bool IsCurrentExitTrigger(Transform triggerTransform)
    {
        if (isTransitioning) return false;

        if (currentLevel > 0 && currentLevel <= levels.Count)
        {
            return levels[currentLevel - 1].exitTrigger == triggerTransform;
        }
        return false;
    }

    #endregion
}