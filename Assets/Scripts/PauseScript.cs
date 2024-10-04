using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public void OnGOBack()
    {
        Time.timeScale = 1;
        var actions = GameObject.Find("Orc").GetComponent<PlayerController>();
        actions.PlayerActions.UI.Disable();
        var actions2 = GameObject.Find("Human").GetComponent<PlayerController>();
        actions2.PlayerActions.UI.Disable();
        if (new Settings().GetMode(PlayerPrefs.GetInt("Slot")) == "multiplayer")
        {
            actions.PlayerActions.Multiplayer.Enable();
            actions2.PlayerActions.Multiplayer.Enable();
        }
        else
        {
            actions.PlayerActions.Singleplayer.Enable();
            actions2.PlayerActions.Singleplayer.Enable();

        }
        gameObject.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}