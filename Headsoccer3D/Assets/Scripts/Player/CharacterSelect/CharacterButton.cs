using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterButton : MonoBehaviour, IMenuItem
{
    public int characterID;

    [Header("Character Info")]
    public string characterName;
    public string defaultName;
    public Sprite selectedImage;
    public Sprite deselectedImage;
    public string sceneName;

    private Image image;
    private bool hovered;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnHoverEnter(int playerIndex)
    {
        MenuManager.Instance.SetPortraitInfo(
            playerIndex,
            selectedImage,
            characterName
        );
        if(characterID != -1 )
            PlayerInputHolder.Instance.playerList[playerIndex].selectedCharacterID = characterID;
    }

    public void OnHoverExit(int playerIndex)
    {
        hovered = false;

        MenuManager.Instance.SetPortraitInfo(
            playerIndex,
            deselectedImage,
            defaultName
        );

    }

    public void OnConfirm(int playerIndex)
    {
        //PlayerInputHolder.Instance.playerList[playerIndex].gameObject.SetActive(false);
        if (sceneName != "")
            MenuManager.Instance.LoadGameLevel(sceneName);


        Debug.Log($"Player {playerIndex} selected {characterName}");
    }







    /*    public int characterID;

        [Space(10)]
        [Header("Current Character Info")]
        public string characterName;
        public Sprite selectedImage;
        public string sceneName;

        // for cursor navigation
        [Space(10)]
        [Header("Adjacent Characters")]
        public RectTransform selectionAbove;
        public RectTransform selectionBelow;
        public RectTransform selectionLeft;
        public RectTransform selectionRight;*/

}
