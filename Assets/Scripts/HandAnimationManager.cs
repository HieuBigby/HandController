using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimationManager : MonoBehaviour
{
    public Animator handAnimator;
    public AnimationClip[] testAnimations;
    public float blendTime = 0.3f;
    public float hideDelay = 3f;
    public float inbetweenOffset = 0.5f;
    public float normalizedTransitionDuration;
    public float normalizedTimeOffset;
    public float normalizedTransitionTime;


    [Button]
    public void PlaySequenceAnimation()
    {
        IEnumerator PlayAnimationRoutine()
        {
            handAnimator.Play("HandShow", 0);
            for (int i = 0; i < testAnimations.Length; i++)
            {
                if(i == 0) handAnimator.Play(testAnimations[i].name, 1);
                yield return new WaitForSeconds(testAnimations[i].length - inbetweenOffset);
                if (i < testAnimations.Length - 1)
                {
                    handAnimator.CrossFade(testAnimations[i + 1].name, normalizedTransitionDuration,
                        1, normalizedTimeOffset, normalizedTransitionTime);
                }
            }
            yield return new WaitForSeconds(hideDelay);
            handAnimator.Play("HandHide", 0);
            yield return new WaitForSeconds(0.5f);
        }
        StartCoroutine(PlayAnimationRoutine()); 
    }
}
