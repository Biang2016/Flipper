using System;
using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
using BiangStudio.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
{
    private Transform FragmentRoot;

    private Dictionary<string, int> LevelRetryTimeRecordDict = new Dictionary<string, int>();

    public List<LevelConfig> LevelList = new List<LevelConfig>();
    private LevelConfig currentLevel;
    private int currentLevelIndex = 0;

    private int currentWidth;
    private int currentHeight;
    private string currentLevelName;

    private Rect currentPicRect;

    private int currentFrontCount = 0;

    internal float CurrentLevelGridSize;

    public int CurrentFrontCount
    {
        get { return currentFrontCount; }
        set
        {
            if (currentFrontCount != value)
            {
                currentFrontCount = value;
                Debug.Log($"Front {currentFrontCount}");
                if (currentFrontCount == currentWidth * currentHeight)
                {
                    LevelPass();
                }
            }
        }
    }

    public void Awake()
    {
        FragmentRoot = new GameObject("FragmentRoot").transform;
    }

    public void StartLevel()
    {
        currentLevelIndex = 0;
        currentLevel = LevelList[currentLevelIndex];
        LoadLevel(currentLevel);
        GameStateManager.Instance.SetState(GameState.Playing);
    }

    internal bool[,] FragmentFrontMatrix;
    internal Fragment[,] FragmentMatrix;

    private void LoadLevel(LevelConfig levelConfig)
    {
        ClearFragments();
        if (LevelRetryTimeRecordDict.ContainsKey(levelConfig.LevelName))
        {
            LevelRetryTimeRecordDict[levelConfig.LevelName] += 1;
            if (levelConfig.RetryConfigs != null && levelConfig.RetryConfigs.Count > 0)
            {
                int retryPicMax = levelConfig.RetryConfigs[levelConfig.RetryConfigs.Count - 1].RetryTime;
                if (retryPicMax < LevelRetryTimeRecordDict[levelConfig.LevelName])
                {
                    LevelRetryTimeRecordDict[levelConfig.LevelName] = retryPicMax;
                }

                foreach (LevelConfig.RetryConfig retryConfig in levelConfig.RetryConfigs)
                {
                    if (retryConfig.RetryTime == LevelRetryTimeRecordDict[levelConfig.LevelName])
                    {
                        levelConfig.LevelName = $"{levelConfig.LevelName}_{retryConfig.RetryPostfix}";
                        break;
                    }
                }
            }
        }
        else
        {
            LevelRetryTimeRecordDict.Add(levelConfig.LevelName, 0);
        }

        CurrentFrontCount = 0;
        UIManager.Instance.ShowUIForms<HUDPanel>().Initialize(levelConfig.TickTimer);
        currentLevelName = levelConfig.LevelName;
        currentWidth = levelConfig.Width;
        currentHeight = levelConfig.Height;
        CurrentLevelGridSize = levelConfig.GridSize / 100f;

        float halfX = currentWidth * CurrentLevelGridSize / 2f;
        float halfY = currentHeight * CurrentLevelGridSize / 2f;
        currentPicRect = new Rect(-halfX, -halfY, 2 * halfX, 2 * halfY);

        FragmentFrontMatrix = new bool[levelConfig.Height, levelConfig.Width];
        FragmentMatrix = new Fragment[levelConfig.Height, levelConfig.Width];

        string folder = $"FlipperSprites/{levelConfig.LevelName}";
        Sprite[] frontSprites = Resources.LoadAll<Sprite>($"{folder}/Front");
        Sprite[] backSprites = Resources.LoadAll<Sprite>($"{folder}/Back");
        for (int y = 0; y < levelConfig.Height; y++)
        {
            for (int x = 0; x < levelConfig.Width; x++)
            {
                SquareFragment fragment = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.SquareFragment].AllocateGameObject<SquareFragment>(FragmentRoot);
                fragment.Initialize(frontSprites[y * levelConfig.Width + x], backSprites[y * levelConfig.Width + x], new GridPos(x, y), false, levelConfig.DefaultFragmentConfig);
                fragment.transform.position = new Vector3((x - levelConfig.Width / 2 + 0.5f) * CurrentLevelGridSize, (levelConfig.Height - y - levelConfig.Height / 2 - 0.5f) * CurrentLevelGridSize, 0);
                FragmentFrontMatrix[y, x] = false;
                FragmentMatrix[y, x] = fragment;
            }
        }
    }

    public void FixedUpdate()
    {
        Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);

        float rayX = Mathf.Clamp(ray.origin.x, currentPicRect.xMin, currentPicRect.xMax);
        float rayY = Mathf.Clamp(ray.origin.y, currentPicRect.yMin, currentPicRect.yMax);

        ray.origin = new Vector3(rayX, rayY, ray.origin.z);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerManager.Instance.LayerMask_Fragment))
        {
            FragmentCollider fc = hit.collider.gameObject.GetComponent<FragmentCollider>();
            fc?.MouseHover();
        }
    }

    private void ClearFragments()
    {
        if (FragmentMatrix != null && FragmentFrontMatrix != null)
        {
            for (int i = 0; i < FragmentMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < FragmentMatrix.GetLength(1); j++)
                {
                    FragmentMatrix[i, j]?.PoolRecycle();
                    FragmentMatrix[i, j] = null;
                    FragmentFrontMatrix[i, j] = false;
                }
            }

            FragmentMatrix = null;
            FragmentFrontMatrix = null;
        }

        CurrentFrontCount = 0;
    }

    [Button("Cheat_LevelPass", ButtonSizes.Large)]
    [GUIColor(0, 1, 1)]
    public void LevelPass()
    {
        currentLevelIndex++;
        if (currentLevelIndex < LevelList.Count)
        {
            currentLevel = LevelList[currentLevelIndex];
            LoadLevel(currentLevel);
        }
        else
        {
            GameWin();
        }
    }

    [Button("Cheat_GameWin", ButtonSizes.Large)]
    [GUIColor(0, 1, 0)]
    public void GameWin()
    {
        UIManager.Instance.GetBaseUIForm<HUDPanel>().GameWin();
        GameStateManager.Instance.SetState(GameState.Win);
    }

    [Button("Cheat_LevelFailed", ButtonSizes.Large)]
    [GUIColor(1, 0, 0)]
    public void LevelFailed()
    {
        StartLevel();
    }

    [Serializable]
    public struct LevelConfig
    {
        public string LevelName;
        public int Width;
        public int Height;
        public FragmentConfig DefaultFragmentConfig;
        public int TickTimer;
        public int GridSize;

        [ListDrawerSettings(ListElementLabelName = "GetDisplayName")]
        public List<RetryConfig> RetryConfigs;

        [Serializable]
        public struct RetryConfig
        {
            public int RetryTime;
            public string RetryPostfix;

            public string GetDisplayName()
            {
                return $"{RetryTime}: _{RetryPostfix}";
            }
        }
    }
}