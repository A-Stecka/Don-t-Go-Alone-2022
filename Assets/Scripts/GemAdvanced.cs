using System.Linq;
using UnityEngine;

public class GemAdvanced : GemScript
{
    public int[] correctOrder;
    public GameObject[] objects;

    public override void Completed(int ind)
    {
        if (ind < correct.Length)
        {
            correct[ind] = true;
            Order.Add(ind);
        }
    }

    public override void Failed(int ind)
    {
        if (ind > 0)
        {
            correct[ind] = false;
            Order.Remove(ind);
        }
    }

    protected override void Update()
    {
        if (correct[correct.Length - 1] && !showing)
        {
            SpriteRenderer.enabled = true;
            showing = true;
            StartCoroutine(Show());
        }

        if (correctOrder.Length == Order.Count)
        {
            if (correct[correct.Length - 1] && !showing && CorrectOrder())
            {
                SpriteRenderer.enabled = true;
                showing = true;
                StartCoroutine(Show());
            }
            else
            {
                foreach (GameObject obj in objects)
                {
                    obj.GetComponent<Animator>().enabled = false;
                }
            }
        }
    }

    private bool CorrectOrder()
    {
        return !Order.Where((t, i) => t != correctOrder[i]).Any();
    }
}