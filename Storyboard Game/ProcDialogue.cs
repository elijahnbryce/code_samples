using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProcDialogue : MonoBehaviour
{
    [SerializeField] private GameObject indicator;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField, Range(0,1)] private float speed;
    [SerializeField] private int maxDiag = 99;
    [SerializeField] private KeyCode diagButton = KeyCode.Space;
    private float resetSpeed, count;

    private delegate void IDiagAction();
    private IDiagAction _action;

    // Start is called before the first frame update
    private void OnEnable()
    {
        resetSpeed = speed;
    }

    private void OnDisable()
    {
        _action = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(diagButton))
        {
            _action?.Invoke();
        }
    }

    public void Clear()
    {
        text.text = "";
        speed = resetSpeed;
        count = 0;
        _action = SkipDiag;
        indicator.SetActive(false);
    }

    private void SkipDiag()
    {
        //resetSpeed = speed;
        speed = 1;
        _action = null;
    }

    public IEnumerator ShowDialogue(string s)
    {
        Clear();
        PanelHandle._Instance.currentMsg = s;
        yield return StartCoroutine(Typewriter(s));
    }

    private IEnumerator Typewriter(string s)
    {
        foreach (char c in s)
        {
            count++;
            if (count > maxDiag && (char)32 == c)
            {
                yield return StartCoroutine(LoadNextDiag()); // new daig page
            }
            text.text += c;
            yield return new WaitForSeconds(1-speed);
        }
        yield return StartCoroutine(LoadNextDiag());
    }

    private IEnumerator LoadNextDiag() 
    { 
        indicator.SetActive(true);
        // play indicator animation
        yield return new WaitUntil( ()=> Input.GetKeyDown(diagButton) );
        Clear();
    }
}
