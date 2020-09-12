using BiangStudio.GamePlay;
using BiangStudio.GamePlay.UI;
using BiangStudio.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : MonoSingleton<ClientGameManager>
{
    #region Managers

    #region Mono

    private AudioManager AudioManager => AudioManager.Instance;
    private UIManager UIManager => UIManager.Instance;

    #endregion

    #region TSingletonBaseManager

    #region Resources

    private ConfigManager ConfigManager => ConfigManager.Instance;
    private LayerManager LayerManager => LayerManager.Instance;
    private PrefabManager PrefabManager => PrefabManager.Instance;
    private GameObjectPoolManager GameObjectPoolManager => GameObjectPoolManager.Instance;

    #endregion

    #region Framework

    private GameStateManager GameStateManager => GameStateManager.Instance;
    private RoutineManager RoutineManager => RoutineManager.Instance;

    #endregion

    #region GamePlay

    #region Level

    private FXManager FXManager => FXManager.Instance;

    #endregion

    #endregion

    #endregion

    #endregion

    private void Awake()
    {
        UIManager.Init(
            (prefabName) => Instantiate(PrefabManager.GetPrefab(prefabName)),
            Debug.LogError,
            () => Input.GetMouseButtonDown(0),
            () => Input.GetMouseButtonDown(1),
            () => Input.GetKeyDown(KeyCode.Escape),
            () => Input.GetKeyDown(KeyCode.Return),
            () => Input.GetKeyDown(KeyCode.Tab)
        );

        ConfigManager.Awake();
        LayerManager.Awake();
        PrefabManager.Awake();
        GameObjectPoolManager.Init(new GameObject("GameObjectPoolRoot").transform);
        GameObjectPoolManager.Awake();

        RoutineManager.LogErrorHandler = Debug.LogError;
        RoutineManager.Awake();
        GameStateManager.Awake();

        FXManager.Awake();
    }

    private void Start()
    {
        ConfigManager.Start();
        LayerManager.Start();
        PrefabManager.Start();
        GameObjectPoolManager.Start();

        RoutineManager.Start();
        GameStateManager.Start();

        FXManager.Start();

        StartGame();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F9))
        {
            LevelManager.Instance.LevelPass();
            return;
        }

        if (Input.GetKeyUp(KeyCode.F10))
        {
            SceneManager.LoadScene(0);
            return;
        }

        if (Input.GetKeyUp(KeyCode.F11))
        {
            LevelManager.Instance.LevelFailed();
            return;
        }

        if (Input.GetKeyUp(KeyCode.F12))
        {
            RankManager.Instance.DeleteRecords();
            return;
        }

        ConfigManager.Update(Time.deltaTime);
        LayerManager.Update(Time.deltaTime);
        PrefabManager.Update(Time.deltaTime);
        GameObjectPoolManager.Update(Time.deltaTime);

        RoutineManager.Update(Time.deltaTime, Time.frameCount);
        GameStateManager.Update(Time.deltaTime);

        FXManager.Update(Time.deltaTime);
    }

    void LateUpdate()
    {
        ConfigManager.LateUpdate(Time.deltaTime);
        LayerManager.LateUpdate(Time.deltaTime);
        PrefabManager.LateUpdate(Time.deltaTime);
        GameObjectPoolManager.LateUpdate(Time.deltaTime);

        RoutineManager.LateUpdate(Time.deltaTime);
        GameStateManager.LateUpdate(Time.deltaTime);

        FXManager.LateUpdate(Time.deltaTime);
    }

    void FixedUpdate()
    {
        ConfigManager.FixedUpdate(Time.fixedDeltaTime);
        LayerManager.FixedUpdate(Time.fixedDeltaTime);
        PrefabManager.FixedUpdate(Time.fixedDeltaTime);
        GameObjectPoolManager.FixedUpdate(Time.fixedDeltaTime);

        RoutineManager.FixedUpdate(Time.fixedDeltaTime);
        GameStateManager.FixedUpdate(Time.fixedDeltaTime);

        FXManager.FixedUpdate(Time.fixedDeltaTime);
    }

    private void StartGame()
    {
        LevelManager.Instance.StartLevel();
    }

    private void ShutDownGame()
    {
    }
}