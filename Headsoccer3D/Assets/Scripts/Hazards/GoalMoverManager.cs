using System.Collections;
using UnityEngine;

public class GoalMoverManager : MonoBehaviour
{
    [Header("Goal 1")]
    [SerializeField] GoalMovers mover1_A;
    [SerializeField] GoalMovers mover1_B;
    [SerializeField] GameObject objectToCarry_A;
    [SerializeField] GoalMovement goalMovement_A;

    [Header("Goal 2")]
    [SerializeField] GoalMovers mover2_A;
    [SerializeField] GoalMovers mover2_B;
    [SerializeField] GameObject objectToCarry_B;
    [SerializeField] GoalMovement goalMovement_B;

    [Space]
    [SerializeField] float xOffset;
    [SerializeField] float zOffset;

    [SerializeField] float headHeight;
    [SerializeField] float overshootHeight;
    [SerializeField] float liftDuration;

    Vector3 originalGroundPosition_A;
    Vector3 originalGroundRotation_A;
    Vector3 originalGroundPosition_B;
    Vector3 originalGroundRotation_B;
    bool canStartGoal = true;


    public int arrivedCount;
    int landedCount;
    [SerializeField] Bell bell;

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) TriggerSequence();
    }*/

    public void TriggerSequence()
    {
        if (!canStartGoal) return;

        canStartGoal = false;
        originalGroundPosition_A = objectToCarry_A.transform.position;
        originalGroundRotation_A = objectToCarry_A.transform.eulerAngles;
        originalGroundPosition_B = objectToCarry_B.transform.position;
        originalGroundRotation_B = objectToCarry_B.transform.eulerAngles;

        arrivedCount = 0;
        landedCount = 0;

        Vector3 centerA = originalGroundPosition_A;
        mover1_A.StartPickUp(centerA + new Vector3(-xOffset, 0f, -zOffset), this);
        mover1_B.StartPickUp(centerA + new Vector3(-xOffset, 0f, zOffset), this);

        Vector3 centerB = originalGroundPosition_B;
        mover2_A.StartPickUp(centerB + new Vector3(xOffset, 0f, -zOffset), this);
        mover2_B.StartPickUp(centerB + new Vector3(xOffset, 0f, zOffset), this);
    }


    public void OnBothArrived()
    {
        StartCoroutine(LiftSequence(objectToCarry_A, mover1_A, mover1_B,
                                    originalGroundPosition_A, originalGroundRotation_A,
                                    goalMovement_A));
        StartCoroutine(LiftSequence(objectToCarry_B, mover2_A, mover2_B,
                                    originalGroundPosition_B, originalGroundRotation_B,
                                    goalMovement_B));
    }

    IEnumerator LiftSequence(
        GameObject obj,
        GoalMovers moverA, GoalMovers moverB,
        Vector3 groundPos, Vector3 groundRot,
        GoalMovement goalMovement)
    {
        moverA.SetState(GoalMovers.MoveStates.PickUpPost);
        moverB.SetState(GoalMovers.MoveStates.PickUpPost);

        Vector3 start = obj.transform.position;
        Vector3 end = new Vector3(
            (moverA.transform.position.x + moverB.transform.position.x) / 2f,
            groundPos.y + headHeight,
            (moverA.transform.position.z + moverB.transform.position.z) / 2f
        );
        Vector3 mid = (start + end) / 2f + Vector3.up * overshootHeight;

        float t = 0f;
        while (t < liftDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / liftDuration);
            obj.transform.position =
                (1 - p) * (1 - p) * start +
                2 * (1 - p) * p * mid +
                p * p * end;
            yield return null;
        }
        obj.transform.position = end;

        moverA.SetState(GoalMovers.MoveStates.RunWithPost);
        moverB.SetState(GoalMovers.MoveStates.RunWithPost);
        moverA.transform.SetParent(obj.transform);
        moverB.transform.SetParent(obj.transform);

        moverA.transform.localRotation = Quaternion.Euler(-180, 90, 90);
        moverB.transform.localRotation = Quaternion.Euler(-180, 90, 90);

        moverA.animator.SetFloat("Velocity", 5);
        moverB.animator.SetFloat("Velocity", 5);

        goalMovement.GoalRun(this, groundPos, groundRot);
    }


    public void DetachMovers(GoalMovement caller)
    {
        mover1_A.animator.SetFloat("Velocity", 0);
        mover1_B.animator.SetFloat("Velocity", 0);
        mover2_A.animator.SetFloat("Velocity", 0);
        mover2_B.animator.SetFloat("Velocity", 0);


        if (caller == goalMovement_A)
        {
            mover1_A.transform.SetParent(null);
            mover1_B.transform.SetParent(null);
            mover1_A.SetState(GoalMovers.MoveStates.Leave);
            mover1_B.SetState(GoalMovers.MoveStates.Leave);
        }
        else if (caller == goalMovement_B)
        {
            mover2_A.transform.SetParent(null);
            mover2_B.transform.SetParent(null);
            mover2_A.SetState(GoalMovers.MoveStates.Leave);
            mover2_B.SetState(GoalMovers.MoveStates.Leave);
        }
    }


    public void OnGoalLanded(GoalMovement caller)
    {
        if (caller == goalMovement_A)
            objectToCarry_A.transform.position = originalGroundPosition_A;
        else if (caller == goalMovement_B)
            objectToCarry_B.transform.position = originalGroundPosition_B;

        landedCount++;
        if (landedCount >= 2)
        {
            canStartGoal = true;

            landedCount = 0;

            mover1_A.Leave();
            mover1_B.Leave();
            mover2_A.Leave();
            mover2_B.Leave();
        }
        bell.ResetBell();
    }
}