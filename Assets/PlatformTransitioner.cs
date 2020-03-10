using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlatformTransitioner : MonoBehaviour
{
    public RuntimeAnimatorController DownAnimation;
    public RuntimeAnimatorController UpAnimation;
    private bool doingDown;
    public UnityEvent[] afterDown;
    private int currentI;
    private bool doingUp;
    public UnityEvent afterUp { get; set; }
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void DoUpAnimation()
    {
        animator.runtimeAnimatorController = UpAnimation;
        StartCoroutine(DoAfterUpAnimation(UpAnimation, afterUp));
    }
    public void DoDownAnimation()
    {
        animator.runtimeAnimatorController = DownAnimation;
        StartCoroutine(DoAfterDownAnimation(DownAnimation, afterDown[currentI]));
        currentI++;
    }

    IEnumerator DoAfterDownAnimation(RuntimeAnimatorController controller, UnityEvent uevent){
        yield return new WaitForSeconds(controller.animationClips[0].length);
        uevent?.Invoke();
        //animator.runtimeAnimatorController = null;
        DoUpAnimation();
    }

    IEnumerator DoAfterUpAnimation(RuntimeAnimatorController controller, UnityEvent uevent)
    {
        yield return new WaitForSeconds(controller.animationClips[0].length);
        uevent?.Invoke();
        //animator.runtimeAnimatorController = null;
    }
}
