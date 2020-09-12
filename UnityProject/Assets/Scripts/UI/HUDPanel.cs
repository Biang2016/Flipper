using BiangStudio.GamePlay.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDPanel : BaseUIPanel
{
    public Text TickDownText;

    void Awake()
    {
        UIType.InitUIType(
            false,
            false,
            false,
            UIFormTypes.Fixed,
            UIFormShowModes.Normal,
            UIFormLucencyTypes.Penetrable);
    }

    internal float TickTimer = 60;
    private float Tick;

    private bool isInitialized = false;

    public void Initialize(float tickTimer)
    {
        isInitialized = true;
        TickTimer = tickTimer;
        Tick = tickTimer;
    }

    void Update()
    {
        if (!isInitialized || GameStateManager.Instance.GetState() != GameState.Playing) return;
        Tick -= Time.deltaTime;
        TickDownText.text = Tick.ToString("####");

        if (Tick <= 0)
        {
            LevelManager.Instance.LevelFailed();
        }
    }

    public void GameWin()
    {
        TickDownText.text = "You Win";
    }
}