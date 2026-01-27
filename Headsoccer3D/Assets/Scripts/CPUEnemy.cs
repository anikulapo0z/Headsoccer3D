using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

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
    private bool teamPossession = false;
    public Transform ball;
    [SerializeField] private CPUEnemy[] myTeammates;
    public Vector3 runningDestination;

    //Internal
    [Space(10)]
    [Header("Internal")]
    public float distanceToBall = 0;
    private bool previousPossession = false;
    private Raumdeuter thomasMuller;
    private NavMeshAgent agent;
    private Vector2 gridPos;
    [SerializeField] private HorizontalSpace horizontalSpace;
    [SerializeField] private VerticalSpace verticalSpace;
    private float haveBallTimer = 0;
    private CPUEnemy ballHolder = null;

    private void Start()
    {
        myTeammates = GameObject.FindObjectsByType<CPUEnemy>(FindObjectsSortMode.None);
        ball = GameObject.FindWithTag("Ball").transform;
        thomasMuller = FindFirstObjectByType<Raumdeuter>();
        agent = GetComponent<NavMeshAgent>();
        onRestart();
    }

    public void onRestart()
    {
        assessPosition();
    }

    private void Update()
    {
        haveBallTimer -= Time.deltaTime;
        if (haveBallTimer < 0) iHaveTheBall = false;

        //first, update the team possession
        teamPossession = false;
        for (int i = 0; i < myTeammates.Length; i++)
        {
            if (myTeammates[i].iHaveTheBall)
            {
                teamPossession = true;
                ballHolder = myTeammates[i];
            }
        }
        distanceToBall = (ball.transform.position - transform.position).sqrMagnitude;
        //if you dont have the ball and the ball is within some range
        ////break all rules and chase it
        if (iHaveTheBall == false && (ball.transform.position - transform.position).sqrMagnitude < 3)
        {
            Debug.Log(gameObject.name + " is breaking all rules and chasing ball");
            runningDestination = ball.position;
            agent.SetDestination(runningDestination);
            return;
        }

        //WITH THE BALL POSSESSION, THE BALL HOLDER WILL DRIBBLE or LOOK FOR PASS OR SHOOTING
        //AND NOT BALL HOLDER WILL RUN TO FREE SPACE AND CALL FOR THROUGH PASS OR REGULAR PASS
        if (teamPossession)
        {
            //reaching close to my spot
            if (agent.remainingDistance < 0.1f)
            {
                //regardless of have or not have ball, dribble 
                Debug.Log(gameObject.name + " knows team possession and moving to space");
                runningDestination = thomasMuller.getPointOnFreeSpace(transform);
                agent.SetDestination(runningDestination);
                assessPosition();
            }
            
            if (iHaveTheBall)
            {
                if (haveBallTimer < 0)
                {
                    if (tryShoot())
                    {
                        Debug.Log(gameObject.name + " just shot the ball.");
                        assessPosition();
                        //I have lost the ball
                    }
                    else
                    {
                        //through pass or simple pass
                        for (int i = 0; i < myTeammates.Length; i++)
                        {
                            if (myTeammates[i] != this)
                            {
                                Debug.Log(gameObject.name + " attempting pass.");

                                kickBallTowards(Random.value > 0.5f ?
                                              myTeammates[i].runningDestination 
                                            : myTeammates[i].transform.position);
                                assessPosition();
                            }
                        }
                    }
                    
                }
            }
            else
            {

                // ask for pass (either immediately or once close enough)
                if (agent.remainingDistance < 0.15f)
                {
                    assessPosition();
                    if (ballHolder != null)
                    {
                        Debug.Log(gameObject.name + " has reached free face and is asking for pass.");
                        ballHolder.passCall(transform);
                    }
                }
            }
            return;
        }

        //IF WE REACH HERE, THIS IS NON-POSSESSION

        //if we are in defending or midfiled half of our enemy team, press the ball
        //otherwise shield the goal 
        Debug.Log(gameObject.name + " is defending since team doesnt have possession.");
        runningDestination = horizontalSpace != HorizontalSpace.Attacking ? ball.position
                                            : defendingPost.transform.position +
                                            ((defendingPost.transform.position - ball.position).normalized * 0.14f);
        agent.SetDestination(runningDestination);
        if (previousPossession != teamPossession)
        {
            Debug.Log(gameObject.name + " recognizes possession change.");
            assessPosition();
        }

        previousPossession = teamPossession;
    }



    public void passCall(Transform _receiver)
    {
        //oi mate, barking at wrong tree
        if (!iHaveTheBall) return;

        //I shot the ball
        if (tryShoot()) return;

        Vector3 _dir = (_receiver.position - ball.position);
        float _dist = _dir.magnitude; //well sqrMag is more cost efficient but we need this here for physics

        //yaycast to see if pass lane is clear
        RaycastHit _hit;
        if (Physics.Raycast(ball.position, _dir.normalized, out _hit, _dist * 1.05f))
        {
            //only pass if the reciever is in sight
            if (_hit.transform == _receiver)
            {
                kickBallTowards(_receiver.position);
            }
        }
    }

    //asks Thomas Muller, "Muller-dono, watashiwa on which space?"
    private void assessPosition()
    {
        gridPos = thomasMuller.convertToSpaceGrid(transform.position.x, transform.position.z);
        horizontalSpace = (HorizontalSpace)(int)gridPos.x;
        verticalSpace = (VerticalSpace)(int)gridPos.y;
        checkDistanceFromBallAndSetRole();
    }

    private void checkDistanceFromBallAndSetRole()
    {
        if (index > 1) return;
        float _closestDistance = Mathf.Infinity;
        float _dist = 0;
        int _closestDude = 0;
        for (int i = 0;i < myTeammates.Length;i++)
        {
            //all are defensive minded
            myTeammates[i].thisCPURole = CPURole.Defender;
            _dist = (ball.transform.position - myTeammates[i].transform.position).sqrMagnitude;
            //find the dude that is closest
            if(_dist < _closestDistance)
            {
                _closestDistance = _dist;
                _closestDude = i;
            }
        }
        //closest dude is the attacker (chasing the ball/pressing)
        myTeammates[_closestDude].thisCPURole = CPURole.Attacker;
    }

    private bool tryShoot()
    {
        Vector3 _nearestPost = goalPost.ClosestPointOnBounds(ball.transform.position);
        //if goal is open
        if(thomasMuller.isGridSPaceOccupied(0,1))
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
            for (int i = 0;i < goalAngles.Length; i++)
            {
                //if the angle to the goal is outside of players threshold
                _angle = Vector3.Dot(ball.transform.position, goalAngles[i].position);
                for (int j = 0; j < _angleToPlayers.Length; j++)
                {
                    //criteris one, goal post must not be at the side
                    //criteria two, player infront
                    //criteria three, outside threshold of the player
                    if(_angle > 0.2f && _angleToPlayers[i] > 0.3f && _angle - _angleToPlayers[i] > 0.3f)
                    {
                        //angle found
                        kickBallTowards(goalAngles[i].position);
                        return true;
                    }
                }
            }

            //no shooting chance
            //random between shoot wildly or not shoot
            if(Random.value > 0.7f) // 30% chance
            {
                kickBallTowards(_nearestPost);
                return true;
            }

            return false;
        }
    }

    private void kickBallTowards(Vector3 _target)
    {
        Rigidbody _brb = ball.GetComponent<Rigidbody>();

        Vector3 _dir = (_target - ball.position).normalized;

        _brb.linearVelocity = Vector3.zero;
        _brb.AddForce(_dir * 7.8467f, ForceMode.VelocityChange);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            iHaveTheBall = true;
            haveBallTimer = 2.4391f;

            //shoot check on recieve ball
            if(tryShoot())
            {
                return;
            }

            //tell others they dont have the ball
            for (int i = 0; i < myTeammates.Length; i++)
            {
                if (myTeammates[i] != this)
                {
                    Debug.Log(gameObject.name + " has the ball now and the other person no longer has ball");
                    myTeammates[i].thisCPULoseBall();
                }
            }
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
    }
}
