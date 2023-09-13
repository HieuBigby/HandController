using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimationManager : MonoBehaviour
{
    public Animator handAnimator;
    public AnimationClip testAnimation;

    [Button]
    public void PlaySequenceAnimation()
    {
        IEnumerator PlayAnimationRoutine()
        {
            handAnimator.gameObject.SetActive(true);
            handAnimator.Play(testAnimation.name, 1);
            handAnimator.SetFloat("Speed", 0);
            handAnimator.Play("HandShow", 0);
            yield return new WaitForSeconds(0.5f);
            handAnimator.SetFloat("Speed", 1);
            yield return new WaitForSeconds(testAnimation.length - 0.1f);
            handAnimator.SetFloat("Speed", 0);
            handAnimator.Play("HandHide", 0);
            yield return new WaitForSeconds(0.5f);
            handAnimator.SetFloat("Speed", 1);
            handAnimator.gameObject.SetActive(false);
        }
        StartCoroutine(PlayAnimationRoutine()); 
    }
}
