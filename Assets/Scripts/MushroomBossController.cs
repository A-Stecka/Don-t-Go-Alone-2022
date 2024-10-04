using System.Collections;
using UnityEngine;

public class MushroomBossController : MonoBehaviour
{
    public float moveSpeed;
    public float followDistance;
    public GameObject allLives;
    public GameObject[] lives;
    public GameObject targetStart;

    public GameObject leftEnd;
    public GameObject rightEnd;

    private Animator _animator;
    private GameObject _target;
    private SpriteRenderer _spriteRenderer;
    private GameObject _orc;
    private GameObject _human;
    private bool _fight;
    private bool _active;
    private bool _startedFight;
    private int _idx;

    private void Start()
    {
        _startedFight = false;
        _idx = lives.Length - 1;
        _fight = true;
        _active = false;
        _animator = GetComponent<Animator>();
        _target = null;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _orc = GameObject.Find("Orc");
        _human = GameObject.Find("Human");
        allLives.SetActive(false);
    }

    private void Update()
    {
        if (!_active)
        {
            _active = !gameObject.GetComponent<DialogStart>().enabled;
        }
        else
        {
            if (!_startedFight)
            {
                allLives.SetActive(true);
                if (transform.position.x < targetStart.transform.position.x)
                {
                    transform.position = Vector2.MoveTowards(transform.position,
                        new Vector2(targetStart.transform.position.x + 2f, transform.position.y),
                        moveSpeed * Time.deltaTime);
                    if (_animator.GetInteger("Action") != 3)
                    {
                        _animator.SetInteger("Action", 3);
                    }
                }
                else
                {
                    _startedFight = true;
                }
            }
            else
            {
                if (Vector2.Distance(transform.position, _human.transform.position) < followDistance)
                {
                    _target = _human;
                }
                else if (Vector2.Distance(transform.position, _orc.transform.position) < followDistance)
                {
                    _target = _orc;
                }
                else
                {
                    _target = null;
                }

                if (_target != null && transform.position.x < leftEnd.transform.position.x &&
                    transform.position.x > rightEnd.transform.position.x)
                {
                    allLives.SetActive(true);
                    transform.position = Vector2.MoveTowards(transform.position,
                        new Vector2(_target.transform.position.x, transform.position.y),
                        moveSpeed * Time.deltaTime);
                    if (_animator.GetInteger("Action") != 3)
                    {
                        _animator.SetInteger("Action", 3);
                    }

                    _spriteRenderer.flipX = _target.transform.position.x <= transform.position.x;
                }
                else
                {
                    if (_animator.GetInteger("Action") != 0)
                    {
                        _animator.SetInteger("Action", 0);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("UserAttack"))
        {
            if (!_fight || !_active || !_startedFight) return;
            StartCoroutine(Die());
        }

        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Human"))
        {
            if (other.gameObject.GetComponent<Animator>().GetInteger("Anim") < 2)
            {
                if (!_fight || !_active || !_startedFight) return;
                StartCoroutine(Die());
            }
            else
            {
                if (!_fight || !_active || !_startedFight) return;
                StartCoroutine(Kill());
                other.gameObject.GetComponent<PlayerController>().Die(); // Kill
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }

    private IEnumerator Kill()
    {
        _active = false;
        _animator.SetInteger("Action", 1);
        _fight = false;
        yield return new WaitForSeconds(1f);
        _fight = true;
        _active = true;
    }

    private IEnumerator Die()
    {
        _target = null;
        _animator.SetInteger("Action", 0);
        _fight = false;
        lives[_idx].GetComponent<SpriteRenderer>().enabled = false;
        _idx -= 1;
        if (_idx < 0)
        {
            _animator.SetInteger("Action", 1);
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            _fight = true;
        }
    }
}