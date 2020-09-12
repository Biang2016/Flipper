using System;
using System.Collections.Generic;
using BiangStudio;
using BiangStudio.GameDataFormat;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
using BiangStudio.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
{
    private static uint RandomSeedMultiplier = 1;

    private Transform FragmentRoot;

    private Dictionary<string, int> LevelRetryTimeRecordDict = new Dictionary<string, int>();

    public List<LevelConfig> LevelList = new List<LevelConfig>();
    private LevelConfig currentLevel;
    private int currentLevelIndex = 0;

    private int currentSpriteTotalCount;
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
                if (currentFrontCount == currentSpriteTotalCount)
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
        RandomSeedMultiplier = (uint) (DateTime.Now.Ticks % 100);
        LoadLevel(0);
        GameStateManager.Instance.SetState(GameState.Playing);
    }

    internal bool[,] FragmentFrontMatrix;
    internal Fragment[,] FragmentMatrix;

    private void LoadLevel(int index)
    {
        currentLevelIndex = index;
        currentLevel = LevelList[currentLevelIndex];
        LoadLevel(currentLevel);
    }

    private void LoadLevel(string levelName)
    {
        for (int i = 0; i < LevelList.Count; i++)
        {
            LevelConfig levelConfig = LevelList[i];
            if (levelConfig.LevelName == levelName)
            {
                LoadLevel(i);
                break;
            }
        }
    }

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
        currentSpriteTotalCount = 0;
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

        SRandom sRandom = new SRandom(levelConfig.RandomSeed * RandomSeedMultiplier);

        for (int y = 0; y < levelConfig.Height - 2; y++)
        {
            for (int x = 0; x < levelConfig.Width - 2; x++)
            {
                bool generate = currentLevel.Grid3x3_Probability.Probable(sRandom);
                if (generate) GenerateSquareFragment(levelConfig, x, y, 3, 3, frontSprites, backSprites);
            }
        }

        for (int y = 0; y < levelConfig.Height - 1; y++)
        {
            for (int x = 0; x < levelConfig.Width - 1; x++)
            {
                bool generate = currentLevel.Grid2x2_Probability.Probable(sRandom);
                if (generate) GenerateSquareFragment(levelConfig, x, y, 2, 2, frontSprites, backSprites);
            }
        }

        for (int y = 0; y < levelConfig.Height; y++)
        {
            for (int x = 0; x < levelConfig.Width; x++)
            {
                GenerateSquareFragment(levelConfig, x, y, 1, 1, frontSprites, backSprites);
            }
        }
    }

    private void GenerateSquareFragment(LevelConfig levelConfig, int x, int y, int width, int height, Sprite[] frontSprites, Sprite[] backSprites)
    {
        for (int _y = y; _y < y + height; _y++)
        for (int _x = x; _x < x + width; _x++)
            if (FragmentMatrix[_y, _x] != null)
                return;

        SquareFragment fragment = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.SquareFragment].AllocateGameObject<SquareFragment>(FragmentRoot);
        FragmentConfig fragmentConfig = levelConfig.DefaultFragmentConfig;
        fragmentConfig.Width = width;
        fragmentConfig.Height = height;

        Sprite sprite_front = null;
        Sprite sprite_back = null;
        if (width == 1 && height == 1)
        {
            sprite_front = frontSprites[y * levelConfig.Width + x];
            sprite_back = backSprites[y * levelConfig.Width + x];
        }
        else
        {
            Sprite startSprite_front = frontSprites[y * levelConfig.Width + x];
            Sprite endSprite_front = frontSprites[(y + height - 1) * levelConfig.Width + (x + width - 1)];
            sprite_front = CombineSpriteUtils.CombineSprite(width, height, startSprite_front, endSprite_front);

            Sprite startSprite_back = backSprites[y * levelConfig.Width + x];
            Sprite endSprite_back = backSprites[(y + height - 1) * levelConfig.Width + (x + width - 1)];
            sprite_back = CombineSpriteUtils.CombineSprite(width, height, startSprite_back, endSprite_back);
        }

        fragment.Initialize(sprite_front, sprite_back, new GridPos(x, y), false, fragmentConfig);
        Vector3 center = new Vector3((levelConfig.Width) / 2f * CurrentLevelGridSize, -(levelConfig.Height) / 2f * CurrentLevelGridSize, 0);
        fragment.transform.position = new Vector3((x + width / 2f) * CurrentLevelGridSize, (-y - height / 2f) * CurrentLevelGridSize) - center;

        for (int _y = y; _y < y + height; _y++)
        for (int _x = x; _x < x + width; _x++)
        {
            FragmentFrontMatrix[_y, _x] = false;
            FragmentMatrix[_y, _x] = fragment;
        }

        currentSpriteTotalCount++;
    }

    public void FixedUpdate()
    {
        Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
        float rayX = Mathf.Clamp(ray.origin.x, currentPicRect.xMin + 0.1f, currentPicRect.xMax - 0.1f);
        float rayY = Mathf.Clamp(ray.origin.y, currentPicRect.yMin + 0.1f, currentPicRect.yMax - 0.1f);
        ray.origin = new Vector3(rayX, rayY, ray.origin.z);
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 0.1f);
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
        LoadLevel(currentLevel.LoseGoToLevelName);
    }

    [Serializable]
    public struct LevelConfig
    {
        public string LevelName;
        public string LoseGoToLevelName;
        public int Width;
        public int Height;
        public FragmentConfig DefaultFragmentConfig;
        public int TickTimer;
        public int GridSize;
        public uint RandomSeed;

        [Range(0, 1)]
        public float Grid2x2_Probability;

        [Range(0, 1)]
        public float Grid3x3_Probability;

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