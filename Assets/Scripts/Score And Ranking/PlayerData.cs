public struct PlayerData
{
    // rank is just the index, order of the sorted list according to score
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