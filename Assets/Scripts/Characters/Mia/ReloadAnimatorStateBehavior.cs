using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadAnimationStateBehavior : StateMachineBehaviour
{
    private MiaScript miaScript;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        miaScript = animator.GetComponent<MiaScript>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("ReloadSpeed", miaScript.speedMultiplier);
    }
}
