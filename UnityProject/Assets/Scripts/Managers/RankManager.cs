using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using BiangStudio.Singleton;
using UnityEngine;

class RankManager : TSingleton<RankManager>
{
    public PlayerRecord PlayerRecord = new PlayerRecord();

    public SortedList SortedPlayerRecords = new SortedList();

    private string SavePath = Application.persistentDataPath + "/PlayerRecords.dat";

    public void AddRecord(float seconds)
    {
        while (SortedPlayerRecords.ContainsKey(seconds))
        {
            seconds += 0.01f;
        }

        SortedPlayerRecords.Add(seconds, seconds);
        PlayerRecord.Records.Add(seconds);
    }

    public void SaveRecords()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(SavePath);
        bf.Serialize(file, PlayerRecord);
        file.Close();
    }

    public void LoadRecords()
    {
        if (File.Exists(SavePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(SavePath, FileMode.Open);
            PlayerRecord = (PlayerRecord) bf.Deserialize(file);
            file.Close();

            SortedPlayerRecords.Clear();
            foreach (float seconds in PlayerRecord.Records)
            {
                SortedPlayerRecords.Add(seconds, seconds);
            }
        }
    }

    public void DeleteRecords()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
    }
}

[Serializable]
public class PlayerRecord
{
    public List<float> Records = new List<float>();
}