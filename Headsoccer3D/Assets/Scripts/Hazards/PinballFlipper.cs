using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class PinballFlipper : MonoBehaviour, IPlayerControllable
{

    Vector3 startRotation;
    [SerializeField] GameObject objectToRotate;
    [SerializeField] Vector3 endRotation;
    [SerializeField] float rotationSpeed;
    [SerializeField] float force;
    [SerializeField] float coolDown;
    float previousHitTime;

    [SerializeField] float activeHitWindow;
    Collider hitCollider;


    Tweener onTween;
    Tweener offTween;

    public List<GameObject> hitPlayers = new List<GameObject>();
    public List<Vector3> hitPoints = new List<Vector3>();
    [SerializeField] Transform fallbackKnockback;


    private void Start()
    {
        previousHitTime = Time.time;
        hitCollider = GetComponent<Collider>();
        startRotation = objectToRotate.transform.rotation.eulerAngles;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            FlipFlipper();
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            UnFlipFlipper();
        }
    }

    public void FlipFlipper()
    {
        if (Time.time - previousHitTime < coolDown)
            return;

        previousHitTime = Time.time;
        StartCoroutine(TurnOnThePain());
        offTween.Kill();
        onTween = objectToRotate.transform.DOLocalRotateQuaternion(Quaternion.Euler(endRotation), rotationSpeed);
    }

    public void UnFlipFlipper()
    {
        hitPlayers.Clear();
        onTween.Kill();
        offTween = objectToRotate.transform.DORotateQuaternion(Quaternion.Euler(startRotation), rotationSpeed);
    }

    IEnumerator TurnOnThePain()
    {
        hitCollider.enabled = true;
        yield return new WaitForSeconds(activeHitWindow);
        hitCollider.enabled = false;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && !hitPlayers.Contains(other.gameObject))
        {
            Debug.Log("sdfghjklkjhgfdfghjkl");
            hitPlayers.Add(other.gameObject);

            var collisionPoint = hitCollider.ClosestPoint(other.transform.position);
            hitPoints.Add(collisionPoint);
            

            if(other.transform.position - collisionPoint != Vector3.zero)
                other.gameObject.GetComponent<PlayerController>().GetHitFromPlayer(force, (other.transform.position - collisionPoint).normalized);
            else
                other.gameObject.GetComponent<PlayerController>().GetHitFromPlayer(force, (other.transform.position - fallbackKnockback.position).normalized);
        }
    }

    void OnDrawGizmos()
    {
        foreach(var p in hitPoints)
        {
            Gizmos.color = new Color(1, 1, 1, 1);

            Gizmos.DrawSphere(p, 0.05f);



        }
    }


    #region Player Inputs
    public void OnSprint(bool held){}

    public void OnJump(){}

    public void OnKick(bool held)
    {
        if (held) FlipFlipper();
        else UnFlipFlipper();
    }

    public void OnJoin(){}

    public void OnConfirm(){}

    public void OnCancel(){}

    public void OnAbility(){}

    public void OnMove(Vector2 input){}
    #endregion
}
