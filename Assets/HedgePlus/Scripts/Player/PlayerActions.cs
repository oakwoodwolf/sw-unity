using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    PlayerController player;
    public PlayerAnimator animator { get; set; }
    static ActionBase actionBase = new ActionBase(); //Base class for all the actions. This will be used for setting the Player and Actions components of each action without every action having its own.
    public ActionDefault defaultState;
    public ActionJump jumpState;
    public ActionHoming homingState;
    public ActionSpindash spinDashState;
    public ActionHurt hurtState;
    ActionBase objectState = new ActionBase(); //Empty action, used for pulleys and ziplines and such
    public static ActionBase currentState; //The current state being updated
    public int StateIndex { get; set; } //Changes the current animation state depending on Sonic's state
    //public List<Transform> homingTargets = new List<Transform>();
    public Transform ClosestTarget;

    private void Start()
    {
        player = GetComponent<PlayerController>();
        animator = GetComponent<PlayerAnimator>();
        actionBase.player = player;
        actionBase.actions = this;
        ChangeState(0);
    }

    private void Update()
    {
        currentState.UpdateState();
        homingState.BackgroundUpdate();
        //Debug.DrawLine(transform.position, ClosestTarget.position);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    public void ChangeState(int state)
    {
        ///Here we simply set currentState to the state we would like to transition to, then call Initialize.
        if (currentState is ActionHoming)
            homingState.OnExit();
        switch (state)
        {
            case 0:
                currentState = defaultState;
                break;
            case 1:
                currentState = jumpState;
                break;
            case 2:
                currentState = homingState;
                break;
            case 3:
                currentState = hurtState;
                break;
            case 4:
                currentState = spinDashState;
                break;
            case 5:
                currentState = objectState;
                break;
        }
        currentState.InitializeState(actionBase);
        StateIndex = state; //This is used for changing states in the animator.
    }

    /// <summary>
    /// Gets the closest object tagged "HomingTarget"
    /// </summary>
    /// <returns>Closest Object</returns>
    public Transform GetClosestTarget(LayerMask layer, float Radius, float FOV)
    {
        ///First we use a spherecast to get every object with the given layer in range. Then we go through the
        ///available targets from the spherecast to find which is the closest to Sonic.
        RaycastHit[] TargetsInRange = Physics.SphereCastAll(transform.position, Radius, transform.forward, Radius, layer);
        Transform closestTarget = null;
        float distance = 1f;
        foreach (RaycastHit t in TargetsInRange)
        {
            Transform target = t.transform;
            Vector3 Direction = target.position - transform.position;
            bool Facing = Vector3.Dot(Direction.normalized, transform.forward) > FOV; //Make sure Sonic is facing the target
            float TargetDistance = (Direction.sqrMagnitude / Radius) / Radius;
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(target.position); //Get the target's screen position
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; //Make sure the target is on screen
            if (TargetDistance < distance && Facing && onScreen)
            {
                if (!Physics.Linecast(transform.position, target.position, homingState.BlockingLayers))
                {
                    closestTarget = target;
                    distance = TargetDistance;
                }
            }
        }

        return closestTarget;
    }

    public void UpdateTargets()
    {
        ClosestTarget = null;
        ClosestTarget = GetClosestTarget(homingState.TargetLayer, homingState.MaxHomingDistance, homingState.FieldOfView);
    }
}
#region Action Classes

#endregion
