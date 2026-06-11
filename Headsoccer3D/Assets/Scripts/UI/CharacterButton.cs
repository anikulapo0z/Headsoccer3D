using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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


    [Header("Selection Visuals")]
    [SerializeField] private GameObject[] playerSelectionSprites; // size = 4

    private List<int> hoveringPlayers = new List<int>(); 


    // for map
    [Space(5)]
    [Header("Maps")]
    [SerializeField] Sprite backGroundImage;
    [SerializeField] MapSelectBackground selectBackground;
    public bool ffa_supported = false;



    public enum ButtonType
    {
        None,
        character,
        map
    }

    public ButtonType type;


    void Awake()
    {
        image = GetComponent<Image>();
        DisableAllSelections();
    }

    public void OnHoverEnter(int playerIndex)
    {
        Debug.Log($"Hover ENTER: {gameObject.name} by player {playerIndex}");
        hoveringPlayers.Remove(playerIndex); 
        hoveringPlayers.Add(playerIndex);    // last = highest priority
        UpdateSelectionVisuals();

        switch (type)
        {
            case ButtonType.map:
                Debug.Log(selectBackground);
                Debug.Log(backGroundImage);
                selectBackground.SetBackground(backGroundImage);
                return;

            case ButtonType.character:

                Debug.Log(MenuManager.Instance + " : " + playerIndex + " : " + selectedImage + " : " + characterName);
                MenuManager.Instance.SetPortraitInfo(
                    playerIndex,
                    selectedImage,
                    characterName
                );/*
        if(characterID != -1 )
            PlayerInputHolder.Instance.playerList[playerIndex].selectedCharacterID = characterID;*/

                var player = MenuManager.Instance.joinManager.playerSlots[playerIndex];

                if (player != null)
                {
                    player.selectedCharacterID = characterID;
                }
                break;


        }




    }

    public void OnHoverExit(int playerIndex)
    {
        hovered = false;

        hoveringPlayers.Remove(playerIndex);
        UpdateSelectionVisuals();

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


        //Debug.Log($"Player {playerIndex} selected {characterName}");
    }


    private void UpdateSelectionVisuals()
    {
        DisableAllSelections();

        if (hoveringPlayers.Count == 0)
            return;

        int lastPlayer = hoveringPlayers[hoveringPlayers.Count - 1];

        if (lastPlayer >= 0 && lastPlayer < playerSelectionSprites.Length)
        {
            playerSelectionSprites[lastPlayer].SetActive(true);
        }
    }

    private void DisableAllSelections()
    {
        for (int i = 0; i < playerSelectionSprites.Length; i++)
        {
            playerSelectionSprites[i].SetActive(false);
        }
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
