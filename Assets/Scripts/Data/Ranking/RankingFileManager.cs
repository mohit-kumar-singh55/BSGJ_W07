using System.Collections.Generic;
using System.IO;
using UnityEngine;

// save & load
public class RankingFileManager
{
    // ranking file path
    private readonly string RANKING_PATH = Application.persistentDataPath + "/leaderboard.json";

    // file will be created automatically, when saved for the first time
    public void SaveRankingData(List<PlayerData> playerDatas)
    {
        RankingData data = new() { playerDatas = playerDatas };

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(RANKING_PATH, json);
    }

    public List<PlayerData> LoadRankingData()
    {
        if (!File.Exists(RANKING_PATH))
            return new List<PlayerData>();

        string json = File.ReadAllText(RANKING_PATH);

        RankingData data = JsonUtility.FromJson<RankingData>(json);

        return data.playerDatas;
    }
}