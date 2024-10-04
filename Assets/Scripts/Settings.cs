using System.IO;
using static System.Int32;

public class Settings
{
    //Save 1
    private int _humanLives1;
    private int _orcLives1;
    private string _level1;
    private string _mode1;

    //Save 2
    private int _humanLives2;
    private int _orcLives2;
    private string _level2;
    private string _mode2;

    //Save 3
    private int _humanLives3;
    private int _orcLives3;
    private string _level3;
    private string _mode3;

    public Settings()
    {
        string[] lines = File.ReadAllLines(@"Resources\Saves.txt");
        _humanLives1 = Parse(lines[0]);
        _humanLives2 = Parse(lines[1]);
        _humanLives3 = Parse(lines[2]);
        _orcLives1 = Parse(lines[3]);
        _orcLives2 = Parse(lines[4]);
        _orcLives3 = Parse(lines[5]);
        _level1 = lines[6];
        _level2 = lines[7];
        _level3 = lines[8];
        _mode1 = lines[9];
        _mode2 = lines[10];
        _mode3 = lines[11];
    }

    public int GetHumanLives(int save)
    {
        return save switch
        {
            1 => _humanLives1,
            2 => _humanLives2,
            3 => _humanLives3,
            _ => 3
        };
    }

    public int GetOrcLives(int save)
    {
        return save switch
        {
            1 => _orcLives1,
            2 => _orcLives2,
            3 => _orcLives3,
            _ => 3
        };
    }

    public void SetHumanLives(int save, int lives)
    {
        switch (save)
        {
            case 1:
                _humanLives1 = lives;
                break;
            case 2:
                _humanLives2 = lives;
                break;
            case 3:
                _humanLives3 = lives;
                break;
        }

        SaveAll();
    }

    public void SetOrcLives(int save, int lives)
    {
        switch (save)
        {
            case 1:
                _orcLives1 = lives;
                break;
            case 2:
                _orcLives2 = lives;
                break;
            case 3:
                _orcLives3 = lives;
                break;
        }

        SaveAll();
    }

    public string GetLevel(int save)
    {
        return save switch
        {
            1 => _level1,
            2 => _level2,
            3 => _level3,
            _ => "empty"
        };
    }

    public void SetLevel(int save, string level)
    {
        switch (save)
        {
            case 1:
                _level1 = level;
                break;
            case 2:
                _level2 = level;
                break;
            case 3:
                _level3 = level;
                break;
        }

        SaveAll();
    }

    public string GetMode(int save)
    {
        return save switch
        {
            1 => _mode1,
            2 => _mode2,
            3 => _mode3,
            _ => "empty"
        };
    }

    public void SetMode(int save, string mode)
    {
        switch (save)
        {
            case 1:
                _mode1 = mode;
                break;
            case 2:
                _mode2 = mode;
                break;
            case 3:
                _mode3 = mode;
                break;
        }

        SaveAll();
    }

    private void SaveAll()
    {
        string[] lines =
        {
            _humanLives1.ToString(), _humanLives2.ToString(), _humanLives3.ToString(),
            _orcLives1.ToString(), _orcLives2.ToString(), _orcLives3.ToString(),
            _level1, _level2, _level3,
            _mode1, _mode2, _mode3
        };

        File.WriteAllLines(@"Resources\Saves.txt", lines);
    }
}