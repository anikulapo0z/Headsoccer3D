using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class EarthquakeObject : MonoBehaviour
{
    [HideInInspector]
    public float yKick;
    [HideInInspector]
    public float ballKickForce;
    [HideInInspector]
    public float playerKickForce;
    [HideInInspector]
    public GameObject controllingPlayer;

    [Space(20)]
    [SerializeField] private VisualEffect[] eqEffect;
    [SerializeField] private VisualEffect[] eqDustsEffect;

    [SerializeField] private Color DefaultFlatColor;
    [GradientUsage(true)]
    [SerializeField] private Gradient DefaultColor;

    [SerializeField] private Color BusMapFlatColor;
    [GradientUsage(true)]
    [SerializeField] private Gradient BusMapColor;

    [SerializeField] private Color TrainMapFlatColor;
    [GradientUsage(true)]
    [SerializeField] private Gradient TrainMapColor;

    [SerializeField] private Color BellMapFlatColor;
    [GradientUsage(true)]
    [SerializeField] private Gradient BellMapColor;

    [SerializeField] private Color BoathouseMapFlatColor;
    [GradientUsage(true)]
    [SerializeField] private Gradient BoathouseMapColor;
    private void Start()
    {
        string _sceneName = SceneManager.GetActiveScene().name;
        Color _col = DefaultFlatColor;
        Gradient _grad = DefaultColor;

        if (_sceneName.Contains("bus"))
        {
            _grad = BusMapColor;
            _col = BusMapFlatColor;
        }

        if (_sceneName.Contains("train"))
        {
            _grad = TrainMapColor;
            _col = TrainMapFlatColor;
        }

        if (_sceneName.Contains("LibertyBell"))
        {
            _grad = BellMapColor;
            _col = BellMapFlatColor;
        }

        if (_sceneName.Contains("BoatHouse"))
        {
            _grad = BoathouseMapColor;
            _col = BoathouseMapFlatColor;
        }

        for (int i = 0; i < eqEffect.Length; i++)
        {
            eqEffect[i].SetVector4("Color", _col);
        }
        for (int i = 0; i < eqDustsEffect.Length; i++)
        {
            eqDustsEffect[i].SetGradient("Color Gradient", _grad);
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        Vector3 kickDirection;
        kickDirection = (other.transform.position - transform.position);

        kickDirection.y = 0f;
        kickDirection.Normalize();

        Vector3 t = new Vector3(kickDirection.x * 15, kickDirection.y, kickDirection.z * 15);

        if (other.CompareTag("Ball") || other.CompareTag("FakeBall"))
        {
            other.GetComponent<SoccerBall>().LaunchAtDirection(t + (Vector3.up * yKick), ballKickForce);
        }

        PlayerController otherPlayer = other.GetComponent<PlayerController>();
        if (otherPlayer == null || otherPlayer == controllingPlayer.GetComponent<PlayerController>()) return;

        otherPlayer.GetHitFromPlayer(playerKickForce, t + (Vector3.up * yKick));

        if (other.CompareTag("Bell"))
        {
            other.GetComponent<BellGetHit>().BGetHit();
        }
    }

}
