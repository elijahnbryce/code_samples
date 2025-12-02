using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sticker : MonoBehaviour
{
    [SerializeField] private Transform mask;
    [SerializeField] private Transform front, back, shadow;
    [SerializeField] private float duration = 2f;
    private Tween maskT, backT;
    private Sequence stickerSequence;

    public void Set()
    {
        float targY = front.position.y;
        float maskOffset = Mathf.Abs(mask.position.y - back.position.y);
        float maskTarg = mask.position.y + (targY - (mask.position.y + maskOffset));
        float backTarg = back.position.y + (targY - (back.position.y + maskOffset)) * 2;

        SetSprites();

        // Create a sequence for this sticker's animations
        stickerSequence = DOTween.Sequence();

        maskT = stickerTween(mask, maskTarg);
        backT = stickerTween(back, backTarg);

        // Add tweens to the sequence
        stickerSequence.Join(maskT);
        stickerSequence.Join(backT);

        StickAnim();
    }

    private Tween stickerTween(Transform t, float d)
    {
        Tween new_t = t.DOMoveY(d, duration)
            .SetEase(Ease.InOutCubic)
            .Pause()
            .SetAutoKill(false)
            .OnRewind(() => gameObject.SetActive(false))
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        return new_t;
    }

    public void SetSprites()
    {
        Sprite s = front.GetComponent<SpriteRenderer>().sprite;
        back.GetComponent<SpriteRenderer>().sprite = s;
        shadow.GetComponent<SpriteRenderer>().sprite = s;
    }

    public void StickerAnim(bool down)
    {
        if (!down)
        {
            stickerSequence?.Play();
        }
        else
        {
            stickerSequence?.PlayBackwards();
        }
    }

    public void StickAnim()
    {
        shadow.gameObject.SetActive(true);
        stickerSequence?.Restart();
    }

    public void PeelAnim()
    {
        stickerSequence?.PlayBackwards();
    }

    private void OnDestroy()
    {
        // Clean up the sequence when the sticker is destroyed
        stickerSequence?.Kill();
    }
}