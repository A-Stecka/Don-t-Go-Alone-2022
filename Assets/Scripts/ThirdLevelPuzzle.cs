using System.Collections;
using System.Linq;
using UnityEngine;

public sealed class ThirdLevelPuzzle : MonoBehaviour
{
    public bool[] correct;

    private int _nextIndex;
    private Vector3 _finalPosition;
    private bool _completed;
    private bool _finished;

    [Header("Water Objects")] private Vector3 _waterToLowerFinal;
    private Vector3 _waterToHigherFinal;
    public GameObject waterToLower;
    public GameObject waterToHigher;

    private void Awake()
    {
        _completed = false;
        _nextIndex = 0;
        _finished = false;
        _finalPosition = transform.position - Vector3.up * 0.15f;
        transform.position += Vector3.up * 0.15f;
        _waterToLowerFinal = waterToLower.transform.position;
        _waterToHigherFinal = waterToHigher.transform.position;
        waterToHigher.transform.position -= Vector3.up * 0.41f;
        waterToLower.transform.position += Vector3.up * 0.75f;
    }

    private void Update()
    {
        if (AllCorrects() && !_completed)
        {
            _completed = true;
            StartCoroutine(Show());
        }

        if (transform.position.y - _finalPosition.y <= 0.05f && _completed && !_finished)
        {
            StartCoroutine(LowerWater());
            StartCoroutine(HigherWater());
        }
    }

    private IEnumerator Show()
    {
        while (transform.position.y - _finalPosition.y >= 0.05f)
        {
            transform.position -= Vector3.up * 0.02f;
            yield return new WaitForSeconds(0.4f);
            StartCoroutine(Show());
        }
    }

    private IEnumerator LowerWater()
    {
        _finished = true;
        while (waterToLower.transform.position.y - _waterToLowerFinal.y >= 0.05f)
        {
            waterToLower.transform.position -= Vector3.up * 0.02f;
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(LowerWater());
        }
    }

    private IEnumerator HigherWater()
    {
        _finished = true;
        while (_waterToHigherFinal.y - waterToHigher.transform.position.y >= 0.05f)
        {
            waterToHigher.transform.position += Vector3.up * 0.02f;
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(HigherWater());
        }
    }

    public void Completed(int ind = -1)
    {
        if (ind == -1)
        {
            ind = _nextIndex++;
        }

        if (ind < correct.Length)
        {
            correct[ind] = true;
        }
    }

    private bool AllCorrects()
    {
        return correct.All(c => c);
    }
}