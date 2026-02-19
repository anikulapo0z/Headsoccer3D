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
    public bool holdingBall = false; //set from SoccerBall script
    private bool teamPossession = true;
    public CPURole thisCPURole;
    public Transform ball;
    [SerializeField] private CPUEnemy myTeammate;
    [HideInInspector] public Vector3 runningDestination;

    //Internal
    [Space(10)]
    [Header("Internal")]
    [SerializeField] private Raumdeuter thomasMuller;
    private NavMeshAgent agent;
    private Vector2 gridPos;
    [SerializeField] private HorizontalSpace horizontalSpace;
    [SerializeField] private VerticalSpace verticalSpace;
    private bool pauseForAMoment = false;
    private bool movingToRecieve = false;

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
    IEnumerator assessPositionEveryFewFrames()
    {
        yield return new WaitForSeconds(1.0647f);
        do
        {
            assessPosition();
            yield return new WaitForSeconds(0.6812f);
        } while (true);
    }

    public void onRestart()
    {
        assessPosition();
    }

    private void Update()
    {
        //rotation only if not holding ball
        if(!holdingBall)
            transform.rotation = Quaternion.LookRotation((new Vector3
                    (ball.position.x, transform.position.y, ball.position.z) - transform.position).normalized 
                                , Vector3.up);

        //------------------------------------------------------------------------------------------------
        teamPossession = holdingBall || myTeammate.holdingBall;

        if (pauseForAMoment) return;


        //go get ball
        if(movingToRecieve)
        {
            runningDestination = ball.position;
            agent.SetDestination(runningDestination);
            if (agent.remainingDistance < 0.1486f)
            {
                movingToRecieve = false;
            }
            return;
        }

        //sets running destination
        if (thisCPURole == CPURole.Attacker)
            CPUAttackingBehaviour();
        if (thisCPURole == CPURole.Defender)
            CPUDefenseBehaviour();

        agent.SetDestination(runningDestination);

    }

    //Behaviour Tree
    void CPUAttackingBehaviour()
    {
        if(holdingBall)
        {
            if (agent.remainingDistance < 0.62f)
            {
                moveToNewFreeSpace();
            }
        }
        else
        {
            runningDestination = ball.position;
        }
    }

    void CPUDefenseBehaviour()
    {

        if (holdingBall)
        {
            if (agent.remainingDistance < 0.22f)
            {
                myTeammate.tryPass();
            }
        }
        else
        {
            // if you are close to the goal post, shield closer to the post
            runningDestination = Vector3.Lerp(defendingPost.transform.position, ball.position,
               horizontalSpace == HorizontalSpace.Attacking && verticalSpace == VerticalSpace.Central
               ? 0.8861f : 0.513694f);
        }
    }

    //kicking and team play----------------------------------------------------------------------
    private void kickBallTowards(Vector3 _target, bool _switchRoles = false, float _forceMult = 1.0f)
    {
        Debug.Log(gameObject.name + "to kick the ball");

        StartCoroutine(larpAsJudeBelligoalAgainstBarca());

        Rigidbody _brb = ball.GetComponent<Rigidbody>();

        Vector3 _dir = (_target - ball.position).normalized;

        _brb.linearVelocity = Vector3.zero;
        _dir.y = Random.Range(0.234f, 0.76674f) / _forceMult; //more force should not affect y
        _brb.AddForce(_dir * 5.8467f * _forceMult, ForceMode.VelocityChange);

        if (_switchRoles)
        {
            StartCoroutine(larpAsJudeBelligoalAgainstBarca());
            //and litelly switch roles
            myTeammate.thisCPURole = CPURole.Attacker;
            thisCPURole = CPURole.Defender;
            myTeammate.moveToRecievePass();
        }

    }

    private bool tryShoot(bool _directGoal = false)
    {
        //behind the player
        if (Vector3.Dot((goalPost.transform.position - ball.position), (ball.position - transform.position)) < 0f)
        {
            Debug.Log("GOAL BEHIND PLAYER");
            return false;
        }

        Vector3 _nearestPost = goalPost.ClosestPointOnBounds(ball.transform.position);
        //if goal is open
        if (thomasMuller.isGridSPaceOccupied(0, 1) || _directGoal)
        {
            //with force
            kickBallTowards(_nearestPost, false, 2.0127f);
            ball.GetComponent<SoccerBall>().resetBallParent();
            return true;
        }

        //near some distance
        if ((ball.position - goalPost.transform.position).sqrMagnitude > 49.274f)
        {
            Debug.Log("ITS OFODIGUSSGUJ FARRRRRRRRRR");
            return false;
        }
        
        //otherwise, find the angle
        else
        {
            //see players
            float[] _angleToPlayers = new float[realPlayers.Length];
            for (int i = 0; i < realPlayers.Length; i++)
            {
                if (realPlayers[i] == null)
                    continue;

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
                    if (_angle > 0.2f && _angleToPlayers[j] > 0.3f && _angle - _angleToPlayers[j] > 0.39354f)
                    {
                        //angle found
                        kickBallTowards(goalAngles[i].position);
                        ball.GetComponent<SoccerBall>().resetBallParent();
                        return true;
                    }
                }
            }

            //no shooting chance
            //random between shoot wildly or not shoot
            if (Random.value < 0.7f) // 70% chance
            {
                kickBallTowards(_nearestPost);
                ball.GetComponent<SoccerBall>().resetBallParent();
                return true;
            }

            return false;
        }
    }
    private void tryPass()
    {
        //get current position
        //random decision
        Vector3 _dirToTeammate = Random.value > 0.5f ? myTeammate.transform.position : myTeammate.runningDestination;

        //dont pass if teammate is in defensive line
        int _XLine = (int)thomasMuller.convertToSpaceGrid(myTeammate.transform.position.x, myTeammate.transform.position.z).x;

        if (_XLine == 2)
        {
            tryShoot();
        }

        //its behind
        if (Vector3.Dot((_dirToTeammate - ball.position), (ball.position - transform.position)) < 0.1f)
        {
            yeetTheBallCloseToOtherCPU();
            return;
        }

        kickBallTowards(_dirToTeammate, true);
    }
    private void yeetTheBallCloseToOtherCPU()
    {
        //random decision
        Vector3 _dirToTeammate = Random.value > 0.5f ? myTeammate.transform.position : myTeammate.runningDestination;
        kickBallTowards(_dirToTeammate, true);
    }

    private void passToFreeSpace()
    {
        Vector3 _dirToSpace = thomasMuller.getPointOnFreeSpace(transform, myTeammate.transform.position);
        //dont pass towards goal
        Vector2 _zone = thomasMuller.convertToSpaceGrid(_dirToSpace.x, _dirToSpace.z);
        if (_zone.x == 2 && _zone.y == 1)
            return;

        kickBallTowards(_dirToSpace, true);
    }
    public void moveToRecievePass(bool sprintToBall = false)
    {
        movingToRecieve = true;
        runningDestination = ball.position;
        agent.speed *= 2;
        StartCoroutine(stopBeingGattuso());
    }

    //Space and Movement-------------------------------------------------------------------
    //asks Thomas Muller, "Muller-dono, watashiwa on which space?"
    private void assessPosition()
    {
        if (pauseForAMoment) return;

        if(teamPossession)
        {
            thisCPURole = holdingBall ? CPURole.Attacker : CPURole.Defender;
            myTeammate.thisCPURole = holdingBall ? CPURole.Defender : CPURole.Attacker;
            return;
        }
        

        gridPos = thomasMuller.convertToSpaceGrid(transform.position.x, transform.position.z);
        horizontalSpace = (HorizontalSpace)(int)gridPos.x;
        verticalSpace = (VerticalSpace)(int)gridPos.y;

        //and check distance and set role
        //only one player will do this part of the code
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
    //declares Thomas Muller, "Sire Muller, thou wish is my command", or something
    private void moveToNewFreeSpace()
    {
        runningDestination = thomasMuller.getPointOnFreeSpace(transform, myTeammate.runningDestination);
        agent.SetDestination(runningDestination);
    }

    //dont chase the ball anymore and think of CPURole
    IEnumerator stopBeingGattuso()
    {
        yield return new WaitForSeconds(1.1966f);
        movingToRecieve = false;
        agent.speed /= 2;
        assessPosition();
        moveToNewFreeSpace();
    }

    IEnumerator larpAsJudeBelligoalAgainstBarca()
    {
        pauseForAMoment = true;
        yield return new WaitForSeconds(0.5647f);
        pauseForAMoment = false;
        assessPosition();
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            tryShoot(); //one touch finish
        }
    }
}
