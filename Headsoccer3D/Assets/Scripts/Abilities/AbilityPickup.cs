using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    [HideInInspector]
    public AbilityThrower thrower;

    public void PickUp()
    {
        thrower.ItemPickedUp();
    }


}
