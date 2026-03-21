using UnityEngine;

public class AbilityTrigger : MonoBehaviour
{

    public enum AbilityTypes
    {
        None,
        EmpoweredKick,
        MultiBall
    }
    public AbilityTypes ability = AbilityTypes.None;
    [SerializeField] private AudioSource pickUpSfx;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerAbility pa = other.GetComponent<PlayerAbility>();
            if (pa != null && pa.currentAbility == AbilityTypes.None)
            {
                pa.SetAbility(ability);
                Destroy(gameObject);
            }
            if (pickUpSfx.resource != null)
            {
                pickUpSfx.Play();
            }
            else if (pickUpSfx.resource == null)
            {
                Debug.Log("PickUpSfx AudioSource has no clip assigned.");
            }
        }
    }
}
