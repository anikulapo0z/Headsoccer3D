using TMPro;
using UnityEngine;

public class PlayerCountSwitch : MonoBehaviour
{
    [SerializeField] TMP_Text statusText;
    [SerializeField] bool is1v1 = true;
    public bool force2v2;

    public void SwitchPlayerCount()
    {
        if (force2v2) return;
        is1v1 = MenuManager.Instance.ToggleTeamSizes();
        if (is1v1)
            statusText.text = "1v1";
        else statusText.text = "2v2";

        // if 1v1 then play 1v1 anim
    }
    public void Set2v2()
    {
        is1v1 = false;
        statusText.text = "2v2";
        // is1v1 then play anim to set 2v2
    }

}
