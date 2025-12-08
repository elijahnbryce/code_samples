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
        // Setup Tweens for continuous reuse
        _length = Mathf.Max(_duration, (_duration * _rotMod));

        slam = ConfigTween(targ.transform.DOPunchScale(_scale, _duration).SetEase(Ease.OutElastic));
        shake = ConfigTween(targ.transform.DOPunchRotation(_rot, _duration * _rotMod).SetEase(Ease.InOutBounce));

        // Self-contain the tween's handling
        // Set self inactive for task control 
        plop = DOTween.Sequence().Pause().SetAutoKill(false).SetLink(targ.gameObject, LinkBehaviour.KillOnDestroy)
            .Insert(0, shake)
            .Insert(0, slam)
            .OnComplete( () => { diag = true; })
            .OnRewind( () => { diag = false; targ.transform.gameObject.SetActive(diag); })
            ;
    }

    private Tween ConfigTween(Tween t)
    {
        // We're configuring the tweens on start but
        // we don't want to play until we call them 
        // also link them to their respective objects
        // because want them to be destroyed together
        t
            .Pause()
            .SetAutoKill(false)
            .SetLink(targ.gameObject, LinkBehaviour.KillOnDestroy)
            ;
        return t ;
    }

    public IEnumerator Play(string s)
    {
        // Control dialogue visuals
        toSay = s;
        ShowUp(diag);
        yield return StartCoroutine(CloseWhenDone());
    }

    private void ShowUp(bool b)
    {
        // Toggle the Dialogue Box
        // Restart on Open 
        // Rewind on Close
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
        // Show full message then close
        yield return StartCoroutine(textFX.ShowDialogue(toSay));
        ShowUp(diag);
        yield return new WaitUntil(() => !diag);
    }
}
