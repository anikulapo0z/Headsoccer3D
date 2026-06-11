using UnityEngine;
using UnityEngine.UI;

public class GameModeButton : MonoBehaviour, IMenuItem
{
    public enum ButtonType
    {
        CycleGameMode
    }

    [Header("Type")]
    public ButtonType type;

    [Header("UI")]
    [SerializeField] private Image modeImage;

    [Header("Sprites")]
    [SerializeField] private Sprite classicSprite;
    [SerializeField] private Sprite ffaSprite;
    [SerializeField] private Sprite randomBallSprite;
    [SerializeField] private Sprite stageHazardsSprite;
    [SerializeField] private Sprite comboSprite;

    private void Start()
    {
        ApplyVisual();
    }

    // =========================
    // CURSOR INPUT ENTRY POINT
    // =========================
    public void OnConfirm(int playerIndex)
    {
        Debug.Log("GAME MODE BUTTON HIT");

        switch (type)
        {
            case ButtonType.CycleGameMode:

                CycleMode();
                ApplyVisual();

                Debug.Log("[GameModeButton] Mode changed to: " + MenuManager.Instance.GetGameMode());
                break;
        }
    }

    public void OnHoverEnter(int playerIndex) { }

    public void OnHoverExit(int playerIndex) { }

    // =========================
    // CORE LOGIC
    // =========================
    private void CycleMode()
    {
        var current = MenuManager.Instance.GetGameMode();

        MenuManager.GameMode next;

        switch (current)
        {
            case MenuManager.GameMode.Classic:
                next = MenuManager.GameMode.FFA;
                break;

            case MenuManager.GameMode.FFA:
                next = MenuManager.GameMode.RandomBall;
                break;

            case MenuManager.GameMode.RandomBall:
                next = MenuManager.GameMode.StageHazards;
                break;

            case MenuManager.GameMode.StageHazards:
                next = MenuManager.GameMode.RandomBallAndStageHazards;
                break;

            default:
                next = MenuManager.GameMode.Classic;
                break;
        }

        MenuManager.Instance.SetGameMode(next);
    }

    // =========================
    // VISUAL UPDATE
    // =========================
    private void ApplyVisual()
    {
        if (MenuManager.Instance == null)
        {
            Debug.LogError("[GameModeButton] MenuManager.Instance is NULL");
            return;
        }

        if (modeImage == null)
        {
            Debug.LogError("[GameModeButton] modeImage is NOT assigned");
            return;
        }

        var mode = MenuManager.Instance.GetGameMode();

        switch (mode)
        {
            case MenuManager.GameMode.Classic:
                modeImage.sprite = classicSprite;
                break;

            case MenuManager.GameMode.FFA:
                modeImage.sprite = ffaSprite;
                break;

            case MenuManager.GameMode.RandomBall:
                modeImage.sprite = randomBallSprite;
                break;

            case MenuManager.GameMode.StageHazards:
                modeImage.sprite = stageHazardsSprite;
                break;

            case MenuManager.GameMode.RandomBallAndStageHazards:
                modeImage.sprite = comboSprite;
                break;
        }
    }
}