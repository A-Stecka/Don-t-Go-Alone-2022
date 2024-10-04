using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogStart : MonoBehaviour
{
    private bool _started;
    private bool _completed;
    private bool _waitingForChoice;
    private PlayerController _orc;
    private PlayerController _human;

    private int _idx;

    public GameObject slider;
    public Camera mainCamera;
    public GameObject dialogCamera;
    public string[] texts;
    public string[] speakers;
    public string[] choices;
    public int[] choicesIdxs;

    public GameObject speakerObject;
    public GameObject textGameObject;
    public GameObject borders;
    private TextMeshProUGUI _speaker;
    private TextMeshProUGUI _text;
    public GameObject responsePick;
    public GameObject[] _picks;

    public GameObject EndScreen;

    private Settings _settings;

    private void Start()
    {
        _completed = false;
        _orc = GameObject.Find("Orc").GetComponent<PlayerController>();
        _human = GameObject.Find("Human").GetComponent<PlayerController>();
        _speaker = speakerObject.GetComponent<TextMeshProUGUI>();
        _text = textGameObject.GetComponent<TextMeshProUGUI>();
        _settings = new Settings();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!_completed && (col.CompareTag("Human") || col.CompareTag("Player")))
        {
            slider.SetActive(false);
            _idx = 0;
            _waitingForChoice = false;
            _orc.PlayerActions.Singleplayer.NextText.performed += ctx => NextText();
            _orc.PlayerActions.Multiplayer.NextText.performed += ctx => NextText();
            _orc.MovementEnabled = false;
            _orc.JumpEnabled = false;
            _orc.AttackEnabled = false;
            _human.MovementEnabled = false;
            _human.JumpEnabled = false;
            _human.AttackEnabled = false;
            _orc.GetComponent<Rigidbody2D>().velocity = new Vector2(0, _orc.GetComponent<Rigidbody2D>().velocity.y);
            _human.GetComponent<Rigidbody2D>().velocity = new Vector2(0, _human.GetComponent<Rigidbody2D>().velocity.y);
            _orc.GetComponent<Animator>().SetInteger("Anim", 6);
            _human.GetComponent<Animator>().SetInteger("Anim", 6);
            _started = true;
            NextText();
            dialogCamera.SetActive(true);
            mainCamera.orthographicSize = 1.2f;
        }
    }

    private void NextText()
    {
        if (_started && !_waitingForChoice)
        {
            if (texts[_idx] == "End")
            {
                End();
            }
            else if (texts[_idx] == "Choice")
            {
                _waitingForChoice = true;
                speakerObject.SetActive(false);
                textGameObject.SetActive(false);
                int temp = 0;
                borders.SetActive(false);
                responsePick.SetActive(true);
                foreach (string text in choices)
                {
                    var helper = temp;
                    _picks[temp].SetActive(true);
                    _picks[temp].GetComponent<Button>().onClick.AddListener(() => Pick(choicesIdxs[helper]));
                    _picks[temp++].GetComponentInChildren<TextMeshProUGUI>().text = text;
                }

                _orc.PlayerActions.Singleplayer.Disable();
                _orc.PlayerActions.Multiplayer.Disable();
                _orc.PlayerActions.UI.Enable();
            }
            else if (texts[_idx] == "Life--")
            {
                _orc.Die(false);
                _human.Die(false);
                _idx++;
            }
            else if (texts[_idx] == "Life++")
            {
                _orc.AddLife();
                _human.AddLife();
                _idx++;
            }
            else if (texts[_idx] == "Fight")
            {
                GameObject.Find("MrScaryBird").GetComponent<BirdBossController>().Friendly = false;
                _idx++;
                End();
            }
            else if (texts[_idx] == "Friendly")
            {
                GameObject.Find("MrScaryBird").GetComponent<BirdBossController>().Friendly = true;
                _idx++;
                End();
            }
            else if (texts[_idx] == "FinalEnd")
            {
                EndScreen.SetActive(true);
                _idx++;
                End();
            }
            else
            {
                speakerObject.SetActive(true);
                textGameObject.SetActive(true);
                _speaker.SetText(speakers[_idx]);
                _text.SetText(texts[_idx++]);
            }
        }
    }

    private void Pick(int newIndex)
    {
        _idx = newIndex;
        _waitingForChoice = false;
        responsePick.SetActive(false);
        borders.SetActive(true);
        var temp = 0;
        foreach (var text in choices)
        {
            _picks[temp].GetComponent<Button>().onClick.RemoveAllListeners();
            _picks[temp++].SetActive(false);
        }

        if (_settings.GetMode(PlayerPrefs.GetInt("Slot")) == "multiplayer")
            _orc.PlayerActions.Multiplayer.Enable();
        else
            _orc.PlayerActions.Singleplayer.Enable();
        _orc.PlayerActions.UI.Disable();
        NextText();
    }

    private void End()
    {
        _orc.EnableMovements();
        _human.EnableMovements();
        _human.GetComponent<Rigidbody2D>().isKinematic = false;
        _orc.GetComponent<Rigidbody2D>().isKinematic = false;
        dialogCamera.SetActive(false);
        mainCamera.orthographicSize = 2f;
        _completed = true;
        _started = false;
        slider.SetActive(true);
        gameObject.GetComponent<DialogStart>().enabled = false;
    }
}