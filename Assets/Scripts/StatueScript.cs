using System.Collections;
using UnityEngine;

public class StatueScript : MonoBehaviour
{
    public float secondsTillTurn;
    public bool flipX;
    public int rotationIdx;
    public Sprite activeSprite;
    public GameObject gem;
    public BoxCollider2D leftCollider, rightCollider;
    
    private bool _completed;
    private Settings _settings;
    private SpriteRenderer _spriteRenderer;
    private bool _perm;
    private Sprite _inactiveSprite;

    private void Start()
    {
        _perm = false;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _settings = new Settings();
        _completed = false;
        _inactiveSprite = _spriteRenderer.sprite;
        StartCoroutine(Turn());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<SpriteRenderer>().flipX == flipX && !_completed)
        {
            gem.GetComponent<GemScript>().Completed();
            _completed = true;
            _spriteRenderer.sprite = activeSprite;
            if (_settings.GetMode(PlayerPrefs.GetInt("Slot")) != "multiplayer" &&
                (other.CompareTag("Player") || other.CompareTag("Human")))
            {
                _perm = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!_completed || _perm) return;
        _spriteRenderer.sprite = _inactiveSprite;
        _completed = false;
        gem.GetComponent<GemScript>().Failed();
    }

    private IEnumerator Turn()
    {
        yield return new WaitForSeconds(secondsTillTurn);
        rotationIdx = (rotationIdx + 1) % 2;
        _spriteRenderer.flipX = !_spriteRenderer.flipX;
        if (flipX)
        {
            if (_spriteRenderer.flipX)
            {
                leftCollider.enabled = true;
                rightCollider.enabled = false;
            }
            else
            {
                leftCollider.enabled = false;
                rightCollider.enabled = true;
            }
        }
        else
        {
            if (_spriteRenderer.flipX)
            {
                leftCollider.enabled = false;
                rightCollider.enabled = true;
            }
            else
            {
                leftCollider.enabled = true;
                rightCollider.enabled = false;
            }
        }

        StartCoroutine(Turn());
    }
}