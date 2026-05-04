using UnityEngine;
using System.Collections;

public class AbilityTrigger : MonoBehaviour
{
    public enum AbilityTypes
    {
        None,
        EmpoweredKick,
        MultiBall,
        Earthquake
    }
    public AbilityTypes ability = AbilityTypes.None;
    [SerializeField] private AudioSource pickUpSfx;
    [SerializeField] private Animator animator;
 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerAbility pa = other.GetComponent<PlayerAbility>();
            if (pa != null && pa.currentAbility == AbilityTypes.None)
            {
                pa.SetAbility(ability);

                // Trigger the animation
                if (animator != null)
                {
                    animator.SetTrigger("PickedUp");
                    Debug.Log("Animation trigger set");
                }

                // Disable collider and start destroy coroutine
                GetComponent<Collider>().enabled = false;
                Destroy(gameObject);

                //StartCoroutine(DestroyAfterDelay());
            }
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}