[System.Serializable]
public struct PlayerData
{
    // rankはスコア順に並べたリストのインデックス
    public int RankNo;
    public string Name;
    public int Score;

    public PlayerData(int rankNo, string name, int score)
    {
        RankNo = rankNo;
        Name = name;
        Score = score;
    }
}