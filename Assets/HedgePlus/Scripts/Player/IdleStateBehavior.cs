using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStateBehavior : StateMachineBehaviour
{
    public enum IdleState { Default, Idle }
    public IdleState _state;
    public float Timeout;
    float time;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        time = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        time += Time.deltaTime;
        if (time >= Timeout)
        {
            switch (_state)
            {
                case IdleState.Default:
                    animator.SetInteger("IdleState", Random.Range(0, 4));
                    animator.SetBool("Idle", true);
                    break;
                case IdleState.Idle:
                    animator.SetBool("Idle", false);
                    break;
            }
        }
    }
}
