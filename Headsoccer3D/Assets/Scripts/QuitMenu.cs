using UnityEngine;

public class QuitMenu : MonoBehaviour, IMenuItem
{
    public enum ButtonType
    {
        None,
        OpenMenu,
        Cancel,
        Quit,
        BackToCharacters
    }

    public ButtonType type;

    [Space(10)]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject characterMenu;
    [SerializeField] GameObject mapMenu;


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
