using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterPortrait : MonoBehaviour
{
    [SerializeField] Image portrait;
    [SerializeField] TMP_Text characterName;

    public void SetPortraitFields(Sprite newPortrait, string newCharacterName)
    {
        GetComponent<Image>().sprite = newPortrait;
        characterName.text = newCharacterName.ToUpper();
    }

}
