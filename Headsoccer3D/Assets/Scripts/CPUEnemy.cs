using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public enum CPURole
{
    Attacker,
    Defender
};

public class CPUEnemy : MonoBehaviour
{
    [SerializeField] private int index;

    [Space(10)]
    [Header("Shooting")]
    public Collider goalPost;
    public Transform[] realPlayers;
    public Transform[] goalAngles;

    [Space(10)]
    [Header("Defence")]
    public Collider defendingPost;

    //Teamplay
    [Space(10)]
    [Header("Teamplay")]
    public CPURole thisCPURole;
    public bool iHaveTheBall = false;
    [SerializeField] private bool teamPossession = false;
    public Transform ball;
    [SerializeField] private CPUEnemy myTeammate;
    [HideInInspector] public Vector3 runningDestination;
    public bool iAmChasingBall = false;
    public bool teamIsChasingBall = false;

    //Internal
    [Space(10)]
    [Header("Internal")]
    [SerializeField] private bool ballAtFeet = false; 
    public float distanceToBall = 0;
    private bool previousPossession = false;
    [SerializeField] private Raumdeuter thomasMuller;
    private NavMeshAgent agent;
    private Vector2 gridPos;
    [SerializeField] private HorizontalSpace horizontalSpace;
    [SerializeField] private VerticalSpace verticalSpace;
    private float haveBallTimer = 0;
    //private CPUEnemy ballHolder = null;

    private void Start()
    {
        CPUEnemy[] _CPUs = FindObjectsByType<CPUEnemy>(FindObjectsSortMode.None);
        for (int i = 0; i < _CPUs.Length; i++)
        {
            if (_CPUs[i] != this)
            {
                myTeammate = _CPUs[i];
            }
        }
        ball = GameObject.FindWithTag("Ball").transform;
        agent = GetComponent<NavMeshAgent>();
        onRestart();
        StartCoroutine(assessPositionEveryFewFrames());
    }

    public void onRestart()
    {
        assessPosition();
    }

    private void Update()
    {
        //rotation
        if(!iHaveTheBall)
            transform.rotation = Quaternion.LookRotation((new Vector3
                    (ball.position.x, transform.position.y, ball.position.z) - transform.position).normalized 
                                , Vector3.up);

        //Crucial Infos--------------------------------------------------------------------------
        //ball at feet is at feet range
        //ballAtFeet = Vector3.Dot(transform.position, ball.position) > 0.34891f 
        //            && (ball.position - transform.position).sqrMagnitude < 0.49f;

        //how long to hold on to ball
        haveBallTimer -= Time.deltaTime;
        if (haveBallTimer < 0) iHaveTheBall = false;
        //and update having ball with ballAtFeet
        iHaveTheBall = iHaveTheBall || ballAtFeet;

        //update the team possession and ball chase
        teamPossession = iHaveTheBall || myTeammate.iHaveTheBall;
        teamIsChasingBall = iAmChasingBall || myTeammate.iAmChasingBall;

        //if you dont have the ball and the ball is within some range
        //break all rules and chase it
        //and team is notchasing the ball
        distanceToBall = (ball.transform.position - transform.position).sqrMagnitude;
        if (iHaveTheBall == false 
            && (ball.transform.position - transform.position).sqrMagnitude < 3 
            && !teamIsChasingBall
            && thisCPURole == CPURole.Attacker)
        {
            Debug.Log(gameObject.name + " is breaking all rules and chasing ball");
            runningDestination = ball.position;
            agent.SetDestination(runningDestination);
            iAmChasingBall = true; //set this to true
            return;
        }
        iAmChasingBall = false;

        //------------------------------------------------------------------------------------------------
        //if agent is near the ball, or reached the free space
        if (agent.remainingDistance < 0.05f)
            CPUAttackingBehaviour();

        //regardless of agentDistance, we press sonce we actively need to set ball chase
        if(!teamPossession)
            CPUDefenseBehaviour();

        //on possession change
        if (previousPossession != teamPossession)
        {
            Debug.Log(gameObject.name + " recognizes possession change.");
            assessPosition();
            moveToNewFreeSpace();
        }

        previousPossession = teamPossession;
    }

    //Behaviour Tree
    void CPUAttackingBehaviour()
    {
        //WITH THE BALL POSSESSION, THE BALL HOLDER WILL DRIBBLE or LOOK FOR PASS OR SHOOTING
        //AND NOT BALL HOLDER WILL RUN TO FREE SPACE AND CALL FOR THROUGH PASS OR REGULAR PASS
        if (iHaveTheBall)
        {
            //dribble kinda
            Debug.Log(gameObject.name + " moving to new space");
            assessPosition();
            moveToNewFreeSpace();

            if (haveBallTimer < 1f && ballAtFeet)
            {
                if (tryShoot())
                {
                    //if we shoot
                    Debug.Log(gameObject.name + " just shot the ball.");
                    assessPosition();
                    //I have lost the ball
                }
                else
                {
                    //through pass or simple pass
                    Debug.Log(gameObject.name + " attempting pass.");

                    kickBallTowards(Random.value > 0.5f ?
                                    myTeammate.runningDestination
                                : myTeammate.transform.position);
                    assessPosition();
                    moveToNewFreeSpace();
                }

            }
        }
        else
        {
            //if we reach destination
            // ask for pass
            if(myTeammate.passCall(transform))
            {
                //stay put for a while
                iHaveTheBall = true;
                haveBallTimer = 1.9234f;
            }
            else
            {
                assessPosition();
                moveToNewFreeSpace();
            }
        }
    }
    
    void CPUDefenseBehaviour()
    {
        //here the role of attacker and defencer is more set
        //attacker presses, defender shields goal
        if (thisCPURole == CPURole.Defender)
        {

            // if you are close to the goal post, shield closer to the post
            runningDestination = Vector3.Lerp(defendingPost.transform.position, ball.position,
               horizontalSpace == HorizontalSpace.Attacking && verticalSpace == VerticalSpace.Central
               ? 0.8861f : 0.513694f);
        }
        else
        {
            //if you are attacker, you press
            runningDestination = ball.position;
        }
        agent.SetDestination(runningDestination);
    }

    //kicking and team play----------------------------------------------------------------------
    private void kickBallTowards(Vector3 _target)
    {
        if (!ballAtFeet)
        {
            Debug.Log("Ball is not at feet. Cannot perform kick");
            return;
        }

        Rigidbody _brb = ball.GetComponent<Rigidbody>();

        Vector3 _dir = (_target - ball.position).normalized;

        _brb.linearVelocity = Vector3.zero;
        _brb.AddForce(_dir * 5.8467f, ForceMode.VelocityChange);
    }
    public bool passCall(Transform _receiver)
    {
        //oi mate, barking at wrong tree
        if (!ballAtFeet) return false;

        //I shot the ball
        if (tryShoot()) return false;

        Vector3 _dir = (_receiver.position - ball.position);
        float _dist = _dir.magnitude; //well sqrMag is more cost efficient but we need this here for physics

        //yaycast to see if pass lane is clear
        RaycastHit _hit;
        if (Physics.Raycast(ball.position, _dir.normalized, out _hit, _dist * 1.05f))
        {
            //only pass if the reciever is in sight
            if (_hit.transform == _receiver)
            {
                Debug.Log(gameObject.name + " is passing to " + _receiver.gameObject.name);
                kickBallTowards(_receiver.position);
            }
        }

        return true;
    }
    private bool tryShoot()
    {
        //near some distance
        if ((ball.position - goalPost.transform.position).sqrMagnitude > 45.274)
            return false;

        Vector3 _nearestPost = goalPost.ClosestPointOnBounds(ball.transform.position);
        //if goal is open
        if (thomasMuller.isGridSPaceOccupied(0, 1))
        {
            kickBallTowards(_nearestPost);
            return true;
        }
        //otherwise, find the angle
        else
        {
            //see players
            float[] _angleToPlayers = new float[realPlayers.Length];
            for (int i = 0; i < realPlayers.Length; i++)
            {
                _angleToPlayers[i] = Vector3.Dot(ball.transform.position, realPlayers[i].position);
            }

            //now we check angles to goal
            float _angle;
            float[] _angleToGoal = new float[goalAngles.Length];
            for (int i = 0; i < goalAngles.Length; i++)
            {
                //if the angle to the goal is outside of players threshold
                _angle = Vector3.Dot(ball.transform.position, goalAngles[i].position);
                for (int j = 0; j < _angleToPlayers.Length; j++)
                {
                    //criteris one, goal post must not be at the side
                    //criteria two, player infront
                    //criteria three, outside threshold of the player
                    if (_angle > 0.2f && _angleToPlayers[i] > 0.3f && _angle - _angleToPlayers[i] > 0.39354f)
                    {
                        //angle found
                        kickBallTowards(goalAngles[i].position);
                        return true;
                    }
                }
            }

            //no shooting chance
            //random between shoot wildly or not shoot
            if (Random.value > 0.7f) // 30% chance
            {
                kickBallTowards(_nearestPost);
                return true;
            }

            return false;
        }
    }
    //called when someone recieves the ball
    //or of ball touches real players but CPU doesnt get it back until a second
    public void thisCPULoseBall()
    {
        Debug.Log(gameObject.name + " no longer has the ball.");
        haveBallTimer = -1f;
        iHaveTheBall = false;
        assessPosition();
        moveToNewFreeSpace();
    }

    //Space and Movement-------------------------------------------------------------------
    //asks Thomas Muller, "Muller-dono, watashiwa on which space?"
    private void assessPosition()
    {
        gridPos = thomasMuller.convertToSpaceGrid(transform.position.x, transform.position.z);
        horizontalSpace = (HorizontalSpace)(int)gridPos.x;
        verticalSpace = (VerticalSpace)(int)gridPos.y;

        //and check distance and set role
        //only one player will do this part of the code
        //if (index > 1) return;

        /*
        if (teamIsChasingBall)
        {
            if (myTeammate.iAmChasingBall)
            {
                myTeammate.thisCPURole = CPURole.Attacker;
                thisCPURole = CPURole.Defender;
                return;
            }
            if (iAmChasingBall)
            {
                myTeammate.thisCPURole = CPURole.Defender;
                thisCPURole = CPURole.Attacker;
                return;
            }
        }*/

        //if we reach here, we do dist check
        bool amICloser = (ball.transform.position - myTeammate.transform.position).sqrMagnitude <
            (ball.transform.position - myTeammate.transform.position).sqrMagnitude;

        if (amICloser)
        {
            myTeammate.thisCPURole = CPURole.Defender;
            thisCPURole = CPURole.Attacker;
        }
        else
        {
            myTeammate.thisCPURole = CPURole.Attacker;
            thisCPURole = CPURole.Defender;
        }
    }
    private void moveToNewFreeSpace()
    {
        runningDestination = thomasMuller.getPointOnFreeSpace(transform);
        agent.SetDestination(runningDestination);
    }
    IEnumerator stopBeingMessi()
    {
        yield return new WaitForSeconds(1.0647f);
        ball.parent = null;
    }
    IEnumerator assessPositionEveryFewFrames()
    {
        yield return new WaitForSeconds(1.0647f);
        do
        {
            assessPosition();
            yield return new WaitForSeconds(0.6812f);
        } while (true);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            iHaveTheBall = true;
            ballAtFeet = true;
            haveBallTimer = 2.4391f;

            //kickBallTowards(myTeammate.transform.position);
            //return;

            //shoot check on recieve ball
            if(!tryShoot())
            {
                //if failed, dribble
                ball.parent = transform;
                moveToNewFreeSpace();
                StartCoroutine(stopBeingMessi());
            }
            //tell other they dont have the ball
            Debug.Log(gameObject.name + " has the ball now and the other person no longer has ball");
            myTeammate.thisCPULoseBall();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ball")
        {
            ballAtFeet = false;
        }
    }
}
