using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem _Instance;

    [SerializeField] private DialogueAnim paw;
    [SerializeField] private DialogueAnim bun;
    [SerializeField] private DialogueAnim poe;
    [SerializeField] private float speechInterval = 1f;

    private List<IEnumerator> queue = new List<IEnumerator>();

    private void Awake()
    {
        if (null == _Instance)
            _Instance = this;
        else Destroy(gameObject);
    }

    public void EnqueDiag(IEnumerator routine)
    {
        if (queue.Count == 0)
        {
            queue.Add(routine);
            StartCoroutine(Play());
        }
        else queue.Add(routine);
    }

    public IEnumerator PawSay(string s)
    {
        yield return StartCoroutine(paw.Play(s));
        yield return new WaitForSeconds(speechInterval);
    }

    public IEnumerator BunSay(string s)
    {
        yield return StartCoroutine(bun.Play(s));
        yield return new WaitForSeconds(speechInterval);
    }

    public IEnumerator PoeSay(string s)
    {
        yield return StartCoroutine(poe.Play(s));
        yield return new WaitForSeconds(speechInterval);
    }


    public IEnumerator CustomSay(string s, DialogueAnim custom)
    {
        yield return StartCoroutine(custom.Play(s));
        yield return new WaitForSeconds(speechInterval);
    }

    private IEnumerator Play()
    {
        while (queue.Count > 0){
            yield return StartCoroutine(queue[0]);
            queue.RemoveAt(0);
            yield return new WaitForEndOfFrame();
        }
    }

    public bool QueueEmpty()
    {
        return queue.Count == 0;
    }
}
