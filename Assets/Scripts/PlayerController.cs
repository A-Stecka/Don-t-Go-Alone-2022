using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Animator _animator;
    private GameObject _orc;
    private GameObject _human;

    private bool _groundCheckEnabled = true;
    private bool _jumping;
    private bool _doubleJumpEnabled;
    private bool _chargedAttackEnabled;
    private int _jumps;
    private bool _attackEnabled;
    private bool _dieEnabled;
    
    public bool MovementEnabled { get; set; }
    public bool JumpEnabled { get; set; }

    public GameObject pause;
    public GameObject lose;
    public GameObject livesObj;
    public GameObject attackArea;
    public GameObject chargedAttack;
    public PlayerActions PlayerActions;

    public string player;
    public int mode;

    [Header("Movements")] 
    public float groundOverlapHeight;
    public LayerMask groundMask;
    public float disableGCTime;
    public float speed;
    public float jumpPower;
    public float jumpFallGravityMultiplier;
    public float initialGravityScale;
    
    [Header("Lives")] 
    public int health;
    public GameObject[] lives;

    private Rigidbody2D _rigidbody;
    private CapsuleCollider2D _collider;
    private Vector2 _boxCenter;
    private Vector2 _boxSize;
    private SpriteRenderer _spriteRenderer;
    private ParticleSystem _chargedAttackSystem;
    private CircleCollider2D _chargedAttackCollider;
    private Vector2 _lastRespawn;
    private Settings _settings;


    private void Awake()
    {
        _settings = new Settings();
        _jumps = 0;
        _orc = GameObject.Find("Orc");
        _human = GameObject.Find("Human");
        _dieEnabled = true;
        PlayerActions = new PlayerActions();
        _rigidbody = GetComponent<Rigidbody2D>();
        _chargedAttackSystem = chargedAttack.GetComponent<ParticleSystem>();
        _chargedAttackCollider = chargedAttack.GetComponent<CircleCollider2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _animator.SetFloat("X", 0f);
        PlayerActions.Multiplayer.OrcJump.performed += ctx => Jump();
        PlayerActions.Singleplayer.Jump.performed += ctx => Jump();
        PlayerActions.Multiplayer.HumanJump.performed += ctx => Jump();
        PlayerActions.Multiplayer.Pause.performed += ctx => Pause();
        PlayerActions.Singleplayer.Pause.performed += ctx => Pause();
        PlayerActions.Multiplayer.OrcFire.performed += Attack;
        PlayerActions.Singleplayer.Fire.performed += Attack;
        PlayerActions.Multiplayer.HumanFire.performed += Attack;
        PlayerActions.Multiplayer.HumanCharged.performed += ChargedAttack;
        PlayerActions.Multiplayer.OrcCharged.performed += ChargedAttack;
        PlayerActions.Singleplayer.Charged.performed += ChargedAttack;
        health = gameObject.CompareTag("Player")
            ? _settings.GetOrcLives(PlayerPrefs.GetInt("Slot"))
            : _settings.GetHumanLives(PlayerPrefs.GetInt("Slot"));

        for (var i = health; i < lives.Length; i++)
            lives[i].GetComponent<SpriteRenderer>().enabled = false;
        if (health == 4)
            lives[3].GetComponent<SpriteRenderer>().enabled = true;
        
        if (health == 0)
        {
            health = 3;
            if (gameObject.CompareTag("Player"))
            {
                _human.GetComponent<PlayerController>().Lives(3);
            }
            else
            {
                _orc.GetComponent<PlayerController>().Lives(3);
            }
        }
    }

    private void Start()
    {
        _lastRespawn = gameObject.transform.position;
        MovementEnabled = true;
        SetAnimation("Idle");
    }

    public void Lives(int newLives)
    {
        health = newLives;
        if (gameObject.CompareTag("Player"))
        {
            _human.GetComponent<PlayerController>().health = 3;
        }
        else
        {
            _orc.GetComponent<PlayerController>().health = 3;
        }
        for (var i = health; i < lives.Length; i++)
            lives[i].GetComponent<SpriteRenderer>().enabled = false;
        if (health == 4)
            lives[3].GetComponent<SpriteRenderer>().enabled = true;
    }

    private void Update()
    {
        HandleGravity();
        if (MovementEnabled)
        {
            if (player == "Orc")
            {
                OrcMove();
            }
            else if (player == "Human")
            {
                HumanMove();
            }
        }
    }

    private void OnEnable()
    {
        _settings.SetLevel(PlayerPrefs.GetInt("Slot"), SceneManager.GetActiveScene().name);

        if (_settings.GetMode(PlayerPrefs.GetInt("Slot")) == "multiplayer")
        {
            PlayerActions.Multiplayer.Enable();
            mode = 2;
        }
        else
        {
            PlayerActions.Singleplayer.Enable();
            mode = 1;
        }

        EnableMovements();
    }

    private void OnDisable()
    {
        mode = 0;
        PlayerActions.Singleplayer.Disable();
        PlayerActions.Multiplayer.Disable();
    }

    private void OrcMove()
    {
        var moveInput = mode == 1
            ? PlayerActions.Singleplayer.Move.ReadValue<Vector2>()
            : PlayerActions.Multiplayer.OrcMove.ReadValue<Vector2>();

        _rigidbody.velocity = new Vector2(moveInput.x * speed, _rigidbody.velocity.y);
        Debug.Log(moveInput);
        if (moveInput.x > 0)
        {
            SetAnimation("Walk");
            _spriteRenderer.flipX = false;
        }
        else if (moveInput.x < 0)
        {
            SetAnimation("Walk");
            _spriteRenderer.flipX = true;
        }
        else
        {
            SetAnimation("Idle");
        }
    }

    private void HumanMove()
    {
        if (mode == 2)
        {
            var moveInput = PlayerActions.Multiplayer.HumanMove.ReadValue<Vector2>();
            _rigidbody.velocity = new Vector2(moveInput.x * speed, _rigidbody.velocity.y);
        }
        else if (mode == 1)
        {
            var x = _orc.transform.position.x < transform.position.x
                ? +1f
                : -1f;
            var bounds = _collider.bounds;
            _boxCenter = new Vector2(bounds.center.x - x / 2, bounds.center.y);
            var groundBox = Physics2D.OverlapBox(_boxCenter, _boxSize, 0f, groundMask);
            if (groundBox != null)
            {
                PerformJump();
            }

            _rigidbody.velocity = Math.Abs(transform.position.x - _orc.transform.position.x) > 0.4f
                ? new Vector2(speed * -x, _rigidbody.velocity.y)
                : new Vector2(0, _rigidbody.velocity.y);
        }

        if (_rigidbody.velocity.x > 0)
        {
            SetAnimation("Walk");
            _spriteRenderer.flipX = false;
        }
        else if (_rigidbody.velocity.x < 0)
        {
            SetAnimation("Walk");
            _spriteRenderer.flipX = true;
        }
        else
        {
            SetAnimation("Idle");
        }
    }

    private void HandleGravity()
    {
        if (_groundCheckEnabled && IsGrounded())
        {
            _jumping = false;
            _jumps = 0;
        }
        else if (_jumping && _rigidbody.velocity.y < 0f)
        {
            _rigidbody.gravityScale = initialGravityScale * jumpFallGravityMultiplier;
        }
        else
        {
            _rigidbody.gravityScale = initialGravityScale;
        }
    }

    public bool IsGrounded()
    {
        var bounds = _collider.bounds;
        _boxCenter = new Vector2(bounds.center.x, bounds.center.y) +
                     (Vector2.down * (bounds.extents.y + (groundOverlapHeight / 2f)));
        _boxSize = new Vector2(bounds.size.x, groundOverlapHeight);
        var groundBox = Physics2D.OverlapBox(_boxCenter, _boxSize, 0f, groundMask);
        return groundBox != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            transform.SetParent(collision.gameObject.transform, true);
        }
        else if (collision.gameObject.CompareTag("Killzone"))
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (mode == 1 && other.CompareTag("jumpJohn") && gameObject.CompareTag("Human"))
        {
            PerformJump();
        }
        else if (mode == 1 && other.CompareTag("stayJohn") && gameObject.CompareTag("Human"))
        {
            gameObject.isStatic = true;
        }
        else if (other.gameObject.CompareTag("Respawn"))
        {
            _lastRespawn = other.transform.position;
        }
        else if (other.gameObject.CompareTag("Killzone") || other.gameObject.CompareTag("Attack"))
        {
            Die();
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        transform.parent = null;
    }

    public void AddLife()
    {
        if (health < 4)
        {
            health++;
            lives[health - 1].GetComponent<SpriteRenderer>().enabled = true;
            if (gameObject.CompareTag("Human"))
            {
                _settings.SetHumanLives(PlayerPrefs.GetInt("Slot"), health);
            }
            else
            {
                _settings.SetOrcLives(PlayerPrefs.GetInt("Slot"), health);
            }
        }
    }


    public void Die(bool back = true)
    {
        if (!_dieEnabled) return;
        _dieEnabled = false;
        MovementEnabled = false;
        _attackEnabled = false;
        JumpEnabled = false;
        SetAnimation("Die");
        _rigidbody.velocity = new Vector2(0, 0);
        StartCoroutine(AfterDie(back));
    }

    private IEnumerator AfterDie(bool back = true)
    {
        yield return new WaitForSeconds(2f);
        EnableMovements();
        if (back)
        {
            _orc.GetComponent<Rigidbody2D>().position = _lastRespawn;
            _human.GetComponent<Rigidbody2D>().position = _lastRespawn;
        }

        health -= 1;
        if (gameObject.CompareTag("Player"))
        {
            _settings.SetHumanLives(PlayerPrefs.GetInt("Slot"), health);
        }
        else
        {
            _settings.SetOrcLives(PlayerPrefs.GetInt("Slot"), health);
        }

        lives[health].GetComponent<SpriteRenderer>().enabled = false;
        SetAnimation("Idle");
        _dieEnabled = true;
        if (health == 0)
        {
            livesObj.SetActive(false);
            lose.SetActive(true);
            PlayerActions.Singleplayer.Disable();
            PlayerActions.Multiplayer.Disable();
            PlayerActions.UI.Enable();
        }
    }

    private void Pause()
    {
        Time.timeScale = 0;
        pause.SetActive(true);
        PlayerActions.Singleplayer.Disable();
        PlayerActions.Multiplayer.Disable();
        PlayerActions.UI.Enable();
    }

    private void Jump()
    {
        if ((PlayerActions.Multiplayer.OrcJump.triggered || PlayerActions.Singleplayer.Jump.triggered) &&
            player == "Orc")
        {
            PerformJump();
        }
        else if (player == "Human" && PlayerActions.Multiplayer.enabled &&
                 PlayerActions.Multiplayer.HumanJump.triggered)
        {
            PerformJump();
        }
    }

    private void PerformJump()
    {
        if ((_jumps < 1 || _jumps > 0 && _doubleJumpEnabled && _jumps < 2) && MovementEnabled &&
            JumpEnabled)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpPower);
            _jumping = true;
            SetAnimation("Jump");
            _jumps++;
            StartCoroutine(EnableGroundCheckAfterJump());
        }
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (!_attackEnabled) return;
        if ((PlayerActions.Multiplayer.OrcFire.triggered || PlayerActions.Singleplayer.Fire.triggered) &&
            player == "Orc")
        {
            PerformAttack();
        }
        else if (player == "Human" && PlayerActions.Multiplayer.enabled &&
                 PlayerActions.Multiplayer.HumanFire.triggered)
        {
            PerformAttack();
        }
    }

    private void PerformChargedAttack()
    {
        MovementEnabled = false;
        _attackEnabled = false;
        chargedAttack.tag = "Player";
        _chargedAttackSystem.transform.localPosition =
            _spriteRenderer.flipX ? new Vector3(-3f, 0f) : new Vector3(3f, 0f);
        _chargedAttackCollider.enabled = true;
        _chargedAttackSystem.Play();
        SetAnimation("ChargedAttack");
        StartCoroutine(EnableAttack());
    }

    private void ChargedAttack(InputAction.CallbackContext context)
    {
        if (!_chargedAttackEnabled || !_attackEnabled) return;
        if ((PlayerActions.Multiplayer.OrcCharged.triggered || PlayerActions.Singleplayer.Charged.triggered) &&
            player == "Orc")
        {
            PerformChargedAttack();
        }
        else if (player == "Human" && PlayerActions.Multiplayer.enabled &&
                 PlayerActions.Multiplayer.HumanCharged.triggered)
        {
            PerformChargedAttack();
        }
    }

    private void PerformAttack()
    {
        MovementEnabled = false;
        _attackEnabled = false;
        _chargedAttackSystem.Stop();
        SetAnimation("Attack");
        attackArea.SetActive(true);
        StartCoroutine(EnableAttack());
    }

    private IEnumerator EnableAttack()
    {
        yield return new WaitForSeconds(0.4f);
        _attackEnabled = true;
        MovementEnabled = true;
        attackArea.SetActive((false));
        _chargedAttackCollider.enabled = false;
        chargedAttack.tag = "Player";
        SetAnimation("Idle");
    }

    private IEnumerator EnableGroundCheckAfterJump()
    {
        _groundCheckEnabled = false;
        yield return new WaitForSeconds(disableGCTime);
        _groundCheckEnabled = true;
    }

    private void SetAnimation(string animationClip)
    {
        switch (animationClip)
        {
            case "Attack":
                if (_animator.GetInteger("Anim") != 0)
                {
                    _animator.SetInteger("Anim", 0);
                }

                break;
            case "ChargedAttack":
                if (_animator.GetInteger("Anim") != 1)
                {
                    _animator.SetInteger("Anim", 1);
                }

                break;
            case "Dmg":
                if (_animator.GetInteger("Anim") != 2)
                {
                    _animator.SetInteger("Anim", 2);
                }

                break;
            case "Walk":
                if (_animator.GetInteger("Anim") != 3)
                {
                    _animator.SetInteger("Anim", 3);
                }

                break;
            case "Die":
                if (_animator.GetInteger("Anim") != 4)
                {
                    _animator.SetInteger("Anim", 4);
                }

                break;
            case "Jump":
                if (_animator.GetInteger("Anim") != 5)
                {
                    _animator.SetInteger("Anim", 5);
                }

                break;
            default: //Idle
                if (_animator.GetInteger("Anim") != 6)
                {
                    _animator.SetInteger("Anim", 6);
                }
                break;
        }
    }

    public void UnlockAttack()
    {
        PlayerPrefs.SetInt("Attack", 0);
        _attackEnabled = true;
    }

    public void UnlockJump()
    {
        JumpEnabled = true;
    }

    public void UnlockDoubleJump()
    {
        JumpEnabled = true;
        _doubleJumpEnabled = true;
    }

    public void UnlockChargedAttack()
    {
        _chargedAttackEnabled = true;
        _attackEnabled = true;
    }

    public bool AttackEnabled
    {
        get => _attackEnabled;
        set => _attackEnabled = value;
    }

    public void EnableMovements()
    {
        MovementEnabled = true;
        switch (SceneManager.GetActiveScene().name)
        {
            case "Level 5":
                JumpEnabled = true;
                _attackEnabled = true;
                _doubleJumpEnabled = true;
                _chargedAttackEnabled = true;
                break;
            case "Level 4":
                JumpEnabled = true;
                _attackEnabled = true;
                _doubleJumpEnabled = true;
                _chargedAttackEnabled = false;
                break;
            case "Level 3":
                JumpEnabled = true;
                _attackEnabled = true;
                _doubleJumpEnabled = false;
                _chargedAttackEnabled = false;
                break;
            case "Level 2":
                JumpEnabled = true;
                _attackEnabled = false;
                _doubleJumpEnabled = false;
                _chargedAttackEnabled = false;
                if (PlayerPrefs.GetInt("Attack") == 0)
                {
                    _attackEnabled = true;
                }

                break;
            case "Level 1":
                PlayerPrefs.SetInt("Attack", 1);
                JumpEnabled = false;
                _attackEnabled = false;
                _doubleJumpEnabled = false;
                _chargedAttackEnabled = false;
                break;
        }
    }
}