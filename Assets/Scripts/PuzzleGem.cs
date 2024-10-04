using System.Collections;
using UnityEngine;

public class PuzzleGem : MonoBehaviour
{
    public GameObject gem;
    public int number;
    
    private Animator _animator;
    private bool _completed;

    private void Start()
    {
        _completed = false;
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Human")) && !_completed)
        {
            if (other.gameObject.GetComponent<Animator>().GetInteger("Anim") < 2)
            {
                StartCoroutine(Complete());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }

    private IEnumerator Complete()
    {
        _completed = true;
        _animator.enabled = true;
        gem.GetComponent<GemAdvanced>().Completed(number);
        yield return new WaitForSeconds(7f);
        gem.GetComponent<GemAdvanced>().Failed(number);
        _completed = false;
        _animator.enabled = false;
    }
}