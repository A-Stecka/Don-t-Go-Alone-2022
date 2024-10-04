using System.Collections;
using UnityEngine;

public class WizardBoss : MonoBehaviour
{
    public float moveSpeed;
    public float maxAttackScale;
    public float minAttackScale;
    public float boundXLeft;
    public float boundXRight;
    
    public GameObject allLives;
    public GameObject[] lives;
    public GameObject attackFog;
    public GameObject finalGem;
    public GameObject targetStart;
    
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private bool _attack;
    private bool _active;
    private int _idx;
    private bool _goRight;
    private bool _canDie;
    private bool _attackExpanding;
    private bool _startedFight;

    private void Start()
    {
        _idx = lives.Length - 1;
        _attack = true;
        _attackExpanding = true;
        _canDie = true;
        _active = false;
        _startedFight = false;
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        allLives.SetActive(false);
        attackFog.SetActive(false);
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
                    _goRight = true;
                    _spriteRenderer.flipX = !_goRight;
                    if (_animator.GetInteger("Action") != 1)
                    {
                        _animator.SetInteger("Action", 1);
                    }
                }
                else
                {
                    _startedFight = true;
                    attackFog.SetActive(true);
                    StartCoroutine(Attack());
                }
            }
            else
            {
                allLives.SetActive(true);
                if (_goRight)
                {
                    if (transform.position.x >= boundXRight)
                    {
                        _goRight = false;
                    }
                    else
                    {
                        gameObject.transform.position = Vector2.MoveTowards(transform.position,
                            new Vector2(transform.position.x + 1f, gameObject.transform.position.y),
                            moveSpeed * Time.deltaTime);
                    }
                }
                else
                {
                    if (transform.position.x <= boundXLeft)
                    {
                        _goRight = true;
                    }
                    else
                    {
                        gameObject.transform.position = Vector2.MoveTowards(transform.position,
                            new Vector2(transform.position.x - 1f, transform.position.y),
                            moveSpeed * Time.deltaTime);
                    }
                }

                if (_animator.GetInteger("Action") != 1 && _canDie)
                {
                    _animator.SetInteger("Action", 1);
                }

                _spriteRenderer.flipX = !_goRight;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("UserAttack") || (other.CompareTag("Player") || other.CompareTag("Human")) &&
            other.GetComponent<Animator>() == null)
        {
            if (!_attack || !_active || !_startedFight) return;
            StartCoroutine(Die());
        }
        else if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Human"))
        {
            if (!_attack || !_active || !_startedFight) return;
            if ((other.gameObject.GetComponent<Animator>() == null ||
                 other.gameObject.GetComponent<Animator>().GetInteger("Anim") < 2) && _canDie)
            {
                StartCoroutine(Die());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }

    private IEnumerator Attack()
    {
        attackFog.tag = "Attack";
        yield return new WaitForSeconds(0.1f);
        if (_attack)
        {
            if (_attackExpanding)
            {
                if (attackFog.transform.localScale.x <= maxAttackScale)
                {
                    attackFog.transform.localScale =
                        new Vector3(attackFog.transform.localScale.x + 0.1f,
                            attackFog.transform.localScale.y + 0.1f);
                }
                else
                {
                    _attackExpanding = false;
                }
            }
            else
            {
                if (attackFog.transform.localScale.x >= minAttackScale)
                {
                    attackFog.transform.localScale =
                        new Vector3(attackFog.transform.localScale.x - 0.1f,
                            attackFog.transform.localScale.y - 0.1f);
                }
                else
                {
                    _attackExpanding = true;
                }
            }

            StartCoroutine(Attack());
        }
    }

    private IEnumerator Die()
    {
        attackFog.tag = "Ground";
        _canDie = false;
        attackFog.SetActive(false);
        _attack = false;
        _animator.SetInteger("Action", 2);
        lives[_idx].GetComponent<SpriteRenderer>().enabled = false;
        _idx -= 1;
        attackFog.transform.localScale = new Vector3(minAttackScale, minAttackScale);
        if (_idx < 0)
        {
            _active = false;
            Instantiate(finalGem, new Vector3(transform.position.x, 1), transform.rotation);
            yield return new WaitForSeconds(3f);
            Destroy(gameObject);
        }
        yield return new WaitForSeconds(2f);
        _canDie = true;
        _attack = true;
        attackFog.SetActive(true);
        StartCoroutine(Attack());
    }
}