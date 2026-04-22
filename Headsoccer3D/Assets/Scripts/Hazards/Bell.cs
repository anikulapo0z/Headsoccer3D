using UnityEngine;
using System.Collections;
using DG.Tweening;

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

    bool isBreaking = false;

    Tween t1;


    private void Start()
    {
        currentHitUntilBreak = totalHitsUntilBreak;
        lastHitTime = Time.time;
    }

    public void BellGetHit()
    {
        if (Time.time - lastHitTime > timeBetweenHits)
        {
            //audioSource.Play();
            lastHitTime = Time.time;
            if (currentHitUntilBreak % 2 == 1)
            {
                if (breakIndex <= bellCracks.Length -1)
                {
                    bellCracks[breakIndex].gameObject.SetActive(true);
                    breakIndex++;
                }
            }
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

        yield return new WaitForSeconds(5);

        goalMoverManager.TriggerSequence();

        yield return new WaitForSeconds(15);

        foreach(Transform child in crackedBell.transform)
        {
            child.DOScale(0.001f, 2);
        }
    }


    public void ResetBell()
    {
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
}
/*bellStand.transform.DOMove(
originalPos +
new Vector3(
    -originalPos.x,
    -originalPos.y - 5,
    -originalPos.z), 5
)*/