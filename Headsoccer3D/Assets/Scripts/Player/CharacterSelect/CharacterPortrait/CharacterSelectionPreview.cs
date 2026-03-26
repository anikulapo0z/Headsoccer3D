using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionPreview : MonoBehaviour
{
    [SerializeField] CharacterPortrait[] portraits;
    public static CharacterSelectionPreview Instance;

    private void Start()
    {
        Instance = this;
    }

    public void SetPortraitInfo(int index, Sprite image, string name)
    {
        if (index < 0 || index >= portraits.Length)
            return;

        portraits[index].SetJoined(index, image, name);
    }
}
