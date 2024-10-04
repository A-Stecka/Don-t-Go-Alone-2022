using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject main;
    public GameObject help;
    public GameObject @continue;
    public GameObject newGame;

    public TextMeshProUGUI[] continueSlotsMode;
    public TextMeshProUGUI[] continueSlotsLevel;

    public TextMeshProUGUI[] newSlotsMode;
    public TextMeshProUGUI[] newSlotsLevel;

    private Settings _settings;

    private void Start()
    {
        _settings = new Settings();
    }

    public void OnContinue()
    {
        @continue.SetActive(true);
        main.SetActive(false);
        var mode1 = _settings.GetMode(1);
        var mode2 = _settings.GetMode(2);
        var mode3 = _settings.GetMode(3);
        var level1 = _settings.GetLevel(1);
        var level2 = _settings.GetLevel(2);
        var level3 = _settings.GetLevel(3);
        if (mode1 != "empty")
        {
            continueSlotsMode[0].text = "Mode: " + mode1;
            continueSlotsLevel[0].text = "Level: " + level1;
        }
        else
        {
            continueSlotsMode[0].text = "empty";
            continueSlotsLevel[0].text = "empty";
        }

        if (mode2 != "empty")
        {
            continueSlotsMode[1].text = "Mode: " + mode2;
            continueSlotsLevel[1].text = "Level: " + level2;
        }
        else
        {
            continueSlotsMode[1].text = "empty";
            continueSlotsLevel[1].text = "empty";
        }

        if (mode3 != "empty")
        {
            continueSlotsMode[2].text = "Mode: " + mode3;
            continueSlotsLevel[2].text = "Level: " + level3;
        }
        else
        {
            continueSlotsMode[2].text = "empty";
            continueSlotsLevel[2].text = "empty";
        }
    }

    public void OnSlot(int slot)
    {
        if (_settings.GetLevel(slot) != "empty")
        {
            PlayerPrefs.SetInt("Slot", slot);
            SceneManager.LoadScene(_settings.GetLevel(slot), LoadSceneMode.Single);
        }
    }

    public void OnNewGame()
    {
        newGame.SetActive(true);
        main.SetActive(false);
        var mode1 = _settings.GetMode(1);
        var mode2 = _settings.GetMode(2);
        var mode3 = _settings.GetMode(3);
        var level1 = _settings.GetLevel(1);
        var level2 = _settings.GetLevel(2);
        var level3 = _settings.GetLevel(3);
        if (mode1 != "empty")
        {
            newSlotsMode[0].text = "Mode: " + mode1;
            newSlotsLevel[0].text = "Level: " + level1;
        }
        else
        {
            newSlotsMode[0].text = "empty";
            newSlotsLevel[0].text = "empty";
        }

        if (mode2 != "empty")
        {
            newSlotsMode[1].text = "Mode: " + mode2;
            newSlotsLevel[1].text = "Level: " + level2;
        }
        else
        {
            newSlotsMode[1].text = "empty";
            newSlotsLevel[1].text = "empty";
        }

        if (mode3 != "empty")
        {
            newSlotsMode[2].text = "Mode: " + mode3;
            newSlotsLevel[2].text = "Level: " + level3;
        }
        else
        {
            newSlotsMode[2].text = "empty";
            newSlotsLevel[2].text = "empty";
        }
    }

    public void OnHelp()
    {
        help.SetActive(true);
        main.SetActive(false);
    }

    public void OnBack()
    {
        main.SetActive(true);
        help.SetActive(false);
    }

    public void ContinueBack()
    {
        main.SetActive(true);
        @continue.SetActive(false);
    }

    public void NewGameBack()
    {
        main.SetActive(true);
        newGame.SetActive(false);
    }

    public void PickSlot(int slot)
    {
        _settings.SetHumanLives(slot, 3);
        _settings.SetOrcLives(slot, 3);
        PlayerPrefs.SetInt("Slot", slot);
    }

    public void StartNewMode(int mode)
    {
        SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
        if (mode == 1)
            _settings.SetMode(PlayerPrefs.GetInt("Slot"), "singleplayer");
        else
            _settings.SetMode(PlayerPrefs.GetInt("Slot"), "multiplayer");
    }
    
    public void Exit()
    {
        Application.Quit();
    }
}