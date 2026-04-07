using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPortrait : MonoBehaviour
{
    [SerializeField] Image portrait;
    [SerializeField] Image border;
    [SerializeField] TMP_Text characterName;

    [Header("States")]
    [SerializeField] Sprite notJoinedSprite;
    [SerializeField] Sprite notJoinedBorder;
    [SerializeField] Sprite[] joinedBorder;
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

        border.sprite = joinedBorder[playerIndex];

        disconnectButton.SetPlayerVar(assignedPlayerIndex);
    }

    public void SetNotJoined()
    {
        assignedPlayerIndex = -1;

        portrait.sprite = notJoinedSprite;
        border.sprite = notJoinedBorder;
        characterName.text = notJoinedText;
    }

    public void OnDisconnectButton()
    {
        if (assignedPlayerIndex != -1)
        {
            MenuManager.Instance.DisconnectPlayer(assignedPlayerIndex);
            Destroy(playerCursor);
            border.sprite = notJoinedBorder;

        }
    }
}