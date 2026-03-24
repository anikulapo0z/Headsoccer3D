using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPortrait : MonoBehaviour
{
    [SerializeField] Image portrait;
    [SerializeField] TMP_Text characterName;

    [Header("States")]
    [SerializeField] Sprite notJoinedSprite;
    [SerializeField] string notJoinedText = "PRESS JOIN";

    public int assignedPlayerIndex = -1;

    public GameObject playerCursor;

    [SerializeField] DisconnectPlayerButton disconnectButton;

    public bool IsOccupied => assignedPlayerIndex != -1;
    public int AssignedPlayerIndex => assignedPlayerIndex;

    public void SetJoined(int playerIndex, Sprite defaultSprite, string name)
    {
        assignedPlayerIndex = playerIndex;

        portrait.sprite = defaultSprite;
        characterName.text = name.ToUpper();

        disconnectButton.SetPlayerVar(assignedPlayerIndex);
    }

    public void SetNotJoined()
    {
        assignedPlayerIndex = -1;

        portrait.sprite = notJoinedSprite;
        characterName.text = notJoinedText;
    }

    public void OnDisconnectButton()
    {
        if (assignedPlayerIndex != -1)
        {
            MenuManager.Instance.DisconnectPlayer(assignedPlayerIndex);
            Destroy(playerCursor);
        }
    }
}