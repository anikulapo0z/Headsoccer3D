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
        }
    }
}
