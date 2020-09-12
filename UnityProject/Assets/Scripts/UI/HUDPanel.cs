using System.Collections;
using System.Collections.Generic;
using BiangStudio.GamePlay.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDPanel : BaseUIPanel
{
    public Text TickDownText;

    public Animator TickDownAnim;
    public Animator FlipItTextAnim;
    public Animator TryAgainTextAnim;
    public Animator CoolTextAnim;

    public Transform RankPanel;
    public Animator RankPanelAnim;
    private List<ScoreRow> ScoreRowList = new List<ScoreRow>();

    void Awake()
    {
        UIType.InitUIType(
            false,
            false,
            false,
            UIFormTypes.Fixed,
            UIFormShowModes.Normal,
            UIFormLucencyTypes.Penetrable);
        WholeGameTimeTick = 0;
        RankPanel.gameObject.SetActive(false);
    }

    internal float TickTimer = 60;
    private float Tick;
    internal float WholeGameTimeTick;

    private bool isInitialized = false;

    private bool infiniteTime = false;

    public void Initialize(float tickTimer, bool showFlipIt)
    {
        TickDownAnim.SetTrigger("Jump");
        if (showFlipIt) FlipItTextAnim.SetTrigger("Jump");
        infiniteTime = tickTimer < 0;
        isInitialized = true;
        TickTimer = tickTimer;
        Tick = tickTimer;
    }

    public void ShowTryAgainText()
    {
        TryAgainTextAnim.SetTrigger("Jump");
    }

    public void ShowCoolText()
    {
        CoolTextAnim.SetTrigger("Jump");
    }

    void Update()
    {
        if (!isInitialized || GameStateManager.Instance.GetState() != GameState.Playing) return;
        if (!infiniteTime)
        {
            WholeGameTimeTick += Time.deltaTime;
            Tick -= Time.deltaTime;
            TickDownText.text = Tick.ToString("####");

            if (Tick <= 0)
            {
                LevelManager.Instance.LevelFailed();
            }
        }
        else
        {
            TickDownText.text = "";
        }
    }

    public void GameWin()
    {
        TickDownText.text = "You Win";
        TickDownAnim.SetTrigger("Jump");
        ShowRecords();
    }

    private int availableSlotNum = 7;

    public void ShowRecords()
    {
        RankManager.Instance.LoadRecords();

        foreach (ScoreRow sr in ScoreRowList)
        {
            sr.PoolRecycle();
        }

        ScoreRowList.Clear();

        RankManager.Instance.AddRecord(WholeGameTimeTick);
        int rank = 0;
        int myRank = 0;
        List<(int, float, bool)> validRankList = new List<(int, float, bool)>();

        foreach (DictionaryEntry de in RankManager.Instance.SortedPlayerRecords)
        {
            if (((float) de.Key).Equals(WholeGameTimeTick)) myRank = rank;
            rank++;
        }

        if (myRank < availableSlotNum - 2)
        {
            rank = 0;
            foreach (DictionaryEntry de in RankManager.Instance.SortedPlayerRecords)
            {
                if (validRankList.Count >= 7) break;
                validRankList.Add((rank + 1, (float) de.Key, rank == myRank));
                rank++;
            }
        }
        else
        {
            validRankList.Add((1, (float) RankManager.Instance.SortedPlayerRecords.GetByIndex(0), false));
            validRankList.Add((2, (float) RankManager.Instance.SortedPlayerRecords.GetByIndex(1), false));
            validRankList.Add((3, (float) RankManager.Instance.SortedPlayerRecords.GetByIndex(2), false));
            validRankList.Add((-1, 0, false));

            if (RankManager.Instance.SortedPlayerRecords.Count - 1 > myRank)
            {
                validRankList.Add((myRank - 1 + 1, (float) RankManager.Instance.SortedPlayerRecords.GetByIndex(myRank - 1), false));
                validRankList.Add((myRank + 1, (float) RankManager.Instance.SortedPlayerRecords.GetByIndex(myRank), true));
                validRankList.Add((myRank + 1 + 1, (float) RankManager.Instance.SortedPlayerRecords.GetByIndex(myRank + 1), false));
            }
            else
            {
                validRankList.Add((myRank - 2 + 1, (float) RankManager.Instance.SortedPlayerRecords.GetByIndex(myRank - 2), false));
                validRankList.Add((myRank - 1 + 1, (float) RankManager.Instance.SortedPlayerRecords.GetByIndex(myRank - 1), false));
                validRankList.Add((myRank + 1, (float) RankManager.Instance.SortedPlayerRecords.GetByIndex(myRank), true));
            }
        }

        foreach ((int, float, bool) tuple in validRankList)
        {
            ScoreRow sr = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ScoreRow].AllocateGameObject<ScoreRow>(RankPanel);
            sr.Initialize(tuple.Item1, tuple.Item2, tuple.Item3);
            ScoreRowList.Add(sr);
        }

        RankManager.Instance.SaveRecords();
        RankPanel.gameObject.SetActive(true);
        RankPanelAnim.SetTrigger("Show");
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}