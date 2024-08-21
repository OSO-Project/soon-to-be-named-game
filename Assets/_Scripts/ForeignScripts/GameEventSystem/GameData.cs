public class GameData
{
    private int _upgradePoints;

    public GameData()
    {
        _upgradePoints = 0;
    }

    public void AddUpgradePoints(int points)
    {
        _upgradePoints += points;
    }
}
