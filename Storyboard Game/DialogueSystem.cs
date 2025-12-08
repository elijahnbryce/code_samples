using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem _Instance;

    [SerializeField] private DialogueAnim alpha, bravo, charlie;
    [SerializeField] private float speechInterval = 1f;

    private List<IEnumerator> queue = new List<IEnumerator>();

    private void Awake()
    {
        if (null == _Instance)
            _Instance = this;
        else Destroy(gameObject);
    }

    private IEnumerator Play()
    {
        // Custom Update Task for Dequeueing Messages 
        // Run when when empty queue recieves an enqueue 
        // Stops when queue is empty
        while (queue.Count > 0){
            yield return StartCoroutine(queue[0]);
            queue.RemoveAt(0);
            yield return new WaitForEndOfFrame();
        }
    }

    public void EnqueDiag(IEnumerator routine)
    {
        // Handle logic for for custome update Task 
        // If queue empty, start a new Play of queue
        if (queue.Count == 0)
        {
            queue.Add(routine);
            StartCoroutine(Play());
        }
        else queue.Add(routine);
    }

    public IEnumerator CustomSay(string s, DialogueAnim custom)
    {
        // These routines go in the queue.
        // Returns from DialogueAnim after 
        // full message shown and textbox closed
        yield return StartCoroutine(custom.Play(s));
        yield return new WaitForSeconds(speechInterval);
    }

    public IEnumerator PawSay(string s)
    {
        yield return StartCoroutine(bravo.Play(s));
        yield return new WaitForSeconds(speechInterval);
    }

    public IEnumerator BunSay(string s)
    {
        yield return StartCoroutine(charlie.Play(s));
        yield return new WaitForSeconds(speechInterval);
    }

    public IEnumerator PoeSay(string s)
    {
        yield return StartCoroutine(alpha.Play(s));
        yield return new WaitForSeconds(speechInterval);
    }

    public bool QueueEmpty()
    {
        return queue.Count == 0;
    }
}
