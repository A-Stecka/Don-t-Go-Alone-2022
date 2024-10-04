using System;
using UnityEngine;

public class PlatformPuzzle : MonoBehaviour
{
    public float minY;
    public float maxY;
    public float change;
    public int number;
    public GameObject gem;
    
    private bool _hasPlayer;
    private long _lastTime;

    private void Start()
    {
        _hasPlayer = false;
        _lastTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    void Update()
    {
        if (gameObject.transform.position.y >= maxY - 0.2f)
        {
            gem.GetComponent<GemScript>().Completed(number);
        }
        else
        {
            gem.GetComponent<GemScript>().Failed(number);
        }

        if (gameObject.transform.position.y > minY && !_hasPlayer &&
            DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastTime > 1000 &&
            new Settings().GetMode(PlayerPrefs.GetInt("Slot")) == "multiplayer")
        {
            Lower(change);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Human"))
        {
            Higher(change);
            _hasPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Human"))
        {
            _hasPlayer = false;
        }
    }

    private void Lower(float x)
    {
        if (gameObject.transform.position.y - x > minY)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x,
                gameObject.transform.position.y - x);
            _lastTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }

    private void Higher(float x)
    {
        if (gameObject.transform.position.y + x <= maxY)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x,
                gameObject.transform.position.y + x);
            _lastTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}