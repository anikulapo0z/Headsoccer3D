using System.Collections;
using UnityEngine;

public class GoalMovers : MonoBehaviour
{
    public enum MoveStates
    {
        Idle,
        GoToGoalPost,
        PickUpPost,
        RunWithPost,
        PutDownPost,
        Leave
    }

    public MoveStates currentState = MoveStates.Idle;
    [SerializeField] float moveSpeed;
    [SerializeField] float destinationRadius;

    public Vector3 pickUpPosition;
    public Vector3 originalPosition;
    GoalMoverManager manager;
    CharacterController controller;

    public Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalPosition = transform.position;

    }

    public void SetState(MoveStates state) => currentState = state;

    public void StartPickUp(Vector3 destination, GoalMoverManager mgr)
    {
        manager = mgr;
        pickUpPosition = destination;
        currentState = MoveStates.GoToGoalPost;
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        animator.SetFloat("Velocity", 5);

        while (currentState == MoveStates.GoToGoalPost)
        {
            Vector3 direction = pickUpPosition - transform.position;
            controller.Move(direction.normalized * moveSpeed * Time.deltaTime);

            transform.LookAt(pickUpPosition);
            

            if (direction.magnitude < destinationRadius)
            {
                currentState = MoveStates.Idle;
                manager.arrivedCount++;
                if (manager.arrivedCount >= 4)
                    manager.OnBothArrived();
            }
            yield return null;
        }
        animator.SetFloat("Velocity", 0);
    }

    public void Leave()
    {
        StartCoroutine(ReturnSequence());
    }

    IEnumerator ReturnSequence()
    {
        animator.SetFloat("Velocity", 5);

        while (currentState == MoveStates.Leave)
        {
            Vector3 direction = originalPosition - transform.position;
            controller.Move(direction.normalized * moveSpeed * Time.deltaTime);
            transform.LookAt(originalPosition);


            if (direction.magnitude < destinationRadius)
            {
                currentState = MoveStates.Idle;
            }
            yield return null;
        }
        animator.SetFloat("Velocity", 0);

    }

}