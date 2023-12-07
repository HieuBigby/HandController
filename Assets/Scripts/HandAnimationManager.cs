using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HandAnimationManager : MonoBehaviour
{
    public AnimationClip[] testSequence;
    public Animator handAnimator;

    public float blendTime = 0.3f;
    public float hideDelay = 3f;
    public float inbetweenOffset = 0.5f;
    public float normalizedTransitionDuration;
    public float normalizedTimeOffset;
    public float normalizedTransitionTime;


    public void ResetAnimation()
    {
        handAnimator.Rebind();
        handAnimator.speed = 1f;
        StopAllCoroutines();
    }

    public void PlaySequenceAnimation(AnimationClip[] clipSequence, Action onComplete = null)
    {
        IEnumerator PlayAnimationRoutine()
        {
            handAnimator.Play("HandShow", 0);
            for (int i = 0; i < clipSequence.Length; i++)
            {
                if(i == 0) handAnimator.Play(clipSequence[i].name, 1);
                yield return new WaitForSeconds(clipSequence[i].length - inbetweenOffset);
                if (i < clipSequence.Length - 1)
                {
                    handAnimator.CrossFade(clipSequence[i + 1].name, normalizedTransitionDuration,
                        1, normalizedTimeOffset, normalizedTransitionTime);
                }
            }
            yield return new WaitForSeconds(hideDelay);
            handAnimator.Play("HandHide", 0);
            yield return new WaitForSeconds(0.5f);
            onComplete?.Invoke();
        }
        StartCoroutine(PlayAnimationRoutine()); 
    }

    [Button]
    public void TestSequence()
    {
        PlaySequenceAnimation(testSequence);
    }
}
