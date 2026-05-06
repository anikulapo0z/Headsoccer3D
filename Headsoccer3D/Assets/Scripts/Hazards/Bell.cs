using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Runtime.CompilerServices;

public class Bell : MonoBehaviour
{

    [SerializeField] int totalHitsUntilBreak;
    [SerializeField] int currentHitUntilBreak;
    [SerializeField] float timeBetweenHits;
    [SerializeField] float lastHitTime;
    [SerializeField] float forceStrength;
    [SerializeField] AudioSource audioSource;

    [SerializeField] int breakIndex = 0;
    [SerializeField] GameObject[] bellCracks;
    [SerializeField] GameObject crackedBell;
    [SerializeField] GameObject wholeBell;
    [SerializeField] GameObject bellStand;

    [SerializeField] GoalMoverManager goalMoverManager;


    [Header("GOING crazy")]
    [SerializeField] Transform muralParent;
    [SerializeField] FreedomMapPedestrians freedomLovingIndependenceHallWatchingMoneyHavingTouristsWhoSomehowCanAffordTravellingInUSAInThisEconomy;
    [SerializeField] FreedomMapCrowd crowd1;
    [SerializeField] FreedomMapCrowd crowd2;
    [SerializeField] GameObject independenceHall;
    [SerializeField] GameObject independenceHallClockTower;
    [SerializeField] Transform clockTowerLookTransform;
    Vector3 clockTowerInitPosition;
    Quaternion clockTowerInitRotation;
    Animator iHallAnim;
    Animator iHallClockTowerAnim;
    [SerializeField] GameObject BellCrowd;


    bool isBreaking = false;

    Tween t1;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(BreakBell());
        }
    }

    private void Start()
    {
        currentHitUntilBreak = totalHitsUntilBreak;
        lastHitTime = Time.time;

        iHallAnim = independenceHall.GetComponent<Animator>();
        iHallClockTowerAnim = independenceHallClockTower.GetComponent<Animator>();

        clockTowerInitPosition = independenceHallClockTower.transform.localPosition;
        clockTowerInitRotation = independenceHallClockTower.transform.localRotation;

        //StartCoroutine(BreakBell());
    }

    public void BellGetHit()
    {
        if (Time.time - lastHitTime > timeBetweenHits)
        {
            //audioSource.Play();
            lastHitTime = Time.time;
            //if (currentHitUntilBreak % 2 == 1)
            //{
                if (breakIndex <= bellCracks.Length -1)
                {
                    bellCracks[breakIndex].gameObject.SetActive(true);
                    breakIndex++;
                }
            //}
            currentHitUntilBreak--;
            if(currentHitUntilBreak <= 0 && !isBreaking)
                StartCoroutine(BreakBell());

        }
    }

    IEnumerator BreakBell()
    {
        isBreaking = true;
        wholeBell.SetActive(false);
        crackedBell.SetActive(true);
        foreach (GameObject t in bellCracks)
        {
            t.SetActive(false);
        }
        foreach (Transform child in crackedBell.transform)
        {
            child.GetComponent<Rigidbody>().isKinematic = false;
            child.GetComponent<Rigidbody>().AddForce((transform.position - child.position).normalized * forceStrength, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(1);

        freedomLovingIndependenceHallWatchingMoneyHavingTouristsWhoSomehowCanAffordTravellingInUSAInThisEconomy.SetAnimation(1);
        crowd1.SetAnimation(1);
        crowd2.SetAnimation(1);
        iHallAnim.SetTrigger("Start Panic");
        iHallClockTowerAnim.SetTrigger("Start Panic");
        independenceHallClockTower.transform.DOLocalMove(clockTowerLookTransform.localPosition, 0.2367f);
        independenceHallClockTower.transform.DOLocalRotateQuaternion(clockTowerLookTransform.localRotation, 0.2367f);

        yield return new WaitForSeconds(1);

        StartCoroutine(triggerMurals(true));

        yield return new WaitForSeconds(3);

        goalMoverManager.TriggerSequence();
        iHallAnim.SetTrigger("Zoom Out");
        iHallClockTowerAnim.SetTrigger("Zoom Out");
        independenceHallClockTower.transform.DOLocalMove(clockTowerInitPosition, 0.2367f);
        independenceHallClockTower.transform.DOLocalRotateQuaternion(clockTowerInitRotation, 0.2367f);

        freedomLovingIndependenceHallWatchingMoneyHavingTouristsWhoSomehowCanAffordTravellingInUSAInThisEconomy.SetAnimation(0);
        crowd1.SetAnimation(0);
        crowd2.SetAnimation(0);
        yield return new WaitForSeconds(15);

        foreach(Transform child in crackedBell.transform)
        {
            child.DOScale(0.001f, 2);
        }
    }


    public void ResetBell()
    {
        //crowd control
        freedomLovingIndependenceHallWatchingMoneyHavingTouristsWhoSomehowCanAffordTravellingInUSAInThisEconomy.SetAnimation(2);
        crowd1.SetAnimation(2);
        crowd2.SetAnimation(2);
        iHallAnim.SetTrigger("Stop Panic");
        iHallClockTowerAnim.SetTrigger("Stop Panic");


        Vector3 originalPos = bellStand.transform.position;
        t1 = bellStand.transform.DOMove(
            originalPos + new Vector3(0, originalPos.y - 5, 0), 5)
            .OnComplete(() => SetBellObjects(originalPos));
    }

    void SetBellObjects(Vector3 pos)
    {
        t1.Kill();
        bellStand.transform.position = new Vector3(pos.x, pos.y + 10, pos.z);
        wholeBell.SetActive(true);
        foreach (Transform child in crackedBell.transform)
        {
            child.GetComponent<Rigidbody>().isKinematic = true;
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.Euler(0, 0, 0);
            child.transform.localScale = Vector3.one;
        }

        crackedBell.SetActive(false);
        breakIndex = 0;
        currentHitUntilBreak = totalHitsUntilBreak;
        isBreaking = false;



        bellStand.transform.DOMove(pos, 3).SetEase(Ease.Linear);
                

    }

    private IEnumerator triggerMurals(bool _goCrazy)
    {
        for (int i = 0; i < muralParent.childCount; i++)
        {
            if(_goCrazy)
                muralParent.GetChild(i).GetComponent<Animation>().Play();
            else
            {
                muralParent.GetChild(i).GetComponent<Animation>().Stop();
                muralParent.GetChild(i).GetComponent<Animation>().Rewind();
            }
            yield return new WaitForSeconds(0.1891f);
        }
    }
}


/*bellStand.transform.DOMove(
originalPos +
new Vector3(
    -originalPos.x,
    -originalPos.y - 5,
    -originalPos.z), 5
)*/