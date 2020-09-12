using BiangStudio.ObjectPool;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRow : PoolObject
{
    public Text RankText;
    public Text TimeText;

    public Image Border;

    public void Initialize(int rank, float seconds, bool isMe)
    {
        if (rank == -1)
        {
            RankText.text = " - ";
            TimeText.text = " - ";
            return;
        }

        if (rank == 1) RankText.text = "1st";
        if (rank == 2) RankText.text = "2nd";
        if (rank == 3) RankText.text = "3rd";
        if (rank > 3) RankText.text = $"{rank}th";

        int secondInt = Mathf.RoundToInt(seconds);
        int minutes = secondInt / 60;
        int second = secondInt % 60;
        TimeText.text = $"{minutes}:{second.ToString("D2")}";

        Border.enabled = isMe;
    }
}