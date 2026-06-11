using UnityEngine;

public class QuitMenu : MonoBehaviour, IMenuItem
{
    public enum ButtonType
    {
        None,
        OpenMenu,
        Cancel,
        Quit,
        BackToCharacters,
        HowToPlay
    }

    public ButtonType type;

    [Space(10)]

    [SerializeField] GameObject[] objectsToTurnOff;
    //[SerializeField] GameObject howToPlayPlayer;
    //[SerializeField] Transform curserHolder;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject characterMenu;
    [SerializeField] GameObject mapMenu;
    [SerializeField] GameObject HowToPlayCamera;
    [SerializeField] GameObject HowToPlayCanvas_controller;
    [SerializeField] GameObject HowToPlayCanvas_arcade;
    [SerializeField] GameObject CharacterSelectBackground;
    [SerializeField] GameObject CharacterSelectPreview;


    public void OnConfirm(int playerIndex)
    {
        switch (type)
        {
            case ButtonType.OpenMenu:
                pauseMenu.SetActive(true);
                MenuManager.Instance.isPaused = true;
                break;

            case ButtonType.Cancel:
                pauseMenu.SetActive(false);
                MenuManager.Instance.isPaused = false;
                break;

            case ButtonType.Quit:
                Application.Quit();
                break;

            case ButtonType.BackToCharacters:
                characterMenu.SetActive(true);
                mapMenu.SetActive(false);

                // Reset confirm/error prompts and screen state
                MenuManager.Instance.pressConfirmPrompt.SetActive(false);
                MenuManager.Instance.wrongPlayerCountPrompt.SetActive(false);
                MenuManager.Instance.SetCurrentScreen(MenuManager.MenuScreen.CharacterSelect);

                foreach (Transform t in MenuManager.Instance.cursorHolder_character)
                {
                    t.gameObject.SetActive(true);
                    t.GetComponent<PlayerCursor>().OnCancel();
                }
                foreach (Transform t in MenuManager.Instance.cursorHolder_map)
                {
                    t.gameObject.SetActive(false);
                }
                break;

            case ButtonType.HowToPlay:
                foreach(GameObject g in objectsToTurnOff)
                {
                    g.SetActive(false);
                }
                foreach(Transform t in MenuManager.Instance.cursorHolder_character)
                {
                    t.gameObject.SetActive(false);
                }
                HowToPlayCamera.SetActive(true);

                if (MenuManager.Instance.joinManager.playerSlots[playerIndex].ControllerId.Contains("EP1"))
                    HowToPlayCanvas_arcade.SetActive(true);
                else
                    HowToPlayCanvas_controller.SetActive(true);
                //CharacterSelectBackground.SetActive(false);
                //CharacterSelectPreview.SetActive(false);
                Debug.Log(playerIndex);
                MenuManager.Instance.SetHowToPlayer(playerIndex);
                MenuManager.Instance.OpenHowToPlay();

                break;
        }
    }

    public void OnHoverEnter(int playerIndex)
    {
    }

    public void OnHoverExit(int playerIndex)
    {
    }
}
