using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public string nextSceneName;
    public GameObject orc;
    public GameObject human;
    public GameObject gem;
    private Settings _settings;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((!other.CompareTag("Player") && !other.CompareTag("Human")) || 
            !gem.gameObject.GetComponent<GemScript>().isShowing())
            return;

        _settings = new Settings();
        _settings.SetHumanLives(PlayerPrefs.GetInt("Slot"), human.GetComponent<PlayerController>().health);
        _settings.SetOrcLives(PlayerPrefs.GetInt("Slot"), orc.GetComponent<PlayerController>().health);
        SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
    }
}