using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.IO;
using UnityEngine.UIElements;

public class DialogueAnim : MonoBehaviour
{
    [SerializeField] private Transform targ;
    [SerializeField] private Vector3 _scale = Vector3.one, _rot = Vector3.one;
    [SerializeField] private float _duration = 3f, _rotMod = 3f;
    [SerializeField] public string toSay;
    private float _length;

    private Sequence plop;
    private Tween slam, shake;

    public bool diag;
    [SerializeField] private ProcDialogue textFX;

    private void Start()
    {
        // store for later
        _length = Mathf.Max(_duration, (_duration * _rotMod));

        slam = ConfigTween(targ.transform.DOPunchScale(_scale, _duration).SetEase(Ease.OutElastic));
        shake = ConfigTween(targ.transform.DOPunchRotation(_rot, _duration * _rotMod).SetEase(Ease.InOutBounce));

        plop = DOTween.Sequence().Pause().SetAutoKill(false).SetLink(targ.gameObject, LinkBehaviour.KillOnDestroy)
            .Insert(0, shake)
            .Insert(0, slam)
            .OnComplete( () => { diag = true; })
            .OnRewind( () => { diag = false; targ.transform.gameObject.SetActive(diag); })
            ;
    }

    private Tween ConfigTween(Tween t)
    {
        t
            .Pause()
            .SetAutoKill(false)
            .SetLink(targ.gameObject, LinkBehaviour.KillOnDestroy)
            ;
        return t ;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowUp(diag);
        }
    }

    public IEnumerator Play(string s)
    {
        toSay = s;
        ShowUp(diag) ;
        yield return StartCoroutine(CloseWhenDone());
    }

    private void ShowUp(bool b)
    {
        if (!b) 
        { 
            targ.gameObject.SetActive(!b);  
            plop.Restart();
        }
        else 
        { 
            plop.PlayBackwards();
            textFX.Clear();
        }
    }

    private IEnumerator CloseWhenDone()
    {
        yield return StartCoroutine(textFX.ShowDialogue(toSay));
        ShowUp(diag);
        yield return new WaitUntil(() => !diag);
    }
}
