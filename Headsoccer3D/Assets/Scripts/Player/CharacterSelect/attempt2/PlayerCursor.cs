using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(RectTransform))]
public class PlayerCursor : MonoBehaviour, IPlayerControllable
{
    [Header("Cursor Settings")]
    public int playerIndex;
    public float moveSpeed = 1200f;

    RectTransform cursorRect;
    RectTransform canvasRect;
    Canvas canvas;
    GraphicRaycaster raycaster;
    EventSystem eventSystem;

    [SerializeField] IMenuItem currentItem;

    [SerializeField] Sprite defaultSpriteCursor;
    [SerializeField] Sprite selectedSpriteCursor;

    [SerializeField] AudioSource uiSelectSound;
    [SerializeField] AudioSource uiDeselectSound;




    Vector2 moveInput;
    public bool isLocked = false;



    void Awake()
    {
        cursorRect = GetComponent<RectTransform>();

        canvas = GetComponentInParent<Canvas>();

        canvasRect = canvas.GetComponent<RectTransform>();
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;

        if (raycaster == null)
            Debug.LogError("canvas missing GraphicRaycaster");

        if (TryGetComponent<Graphic>(out var g))
            g.raycastTarget = false;
    }

    void FixedUpdate()
    {

        if (isLocked) return;
        if (moveInput.sqrMagnitude > 0.1f)
        {
            MoveCursor(moveInput);
            RaycastForMenuItem();
        }
    }


    public void OnMove(Vector2 dir)
    {
        if (!gameObject.activeSelf)
            return;
        moveInput = dir;
    }

    void MoveCursor(Vector2 dir)
    {
        if (!gameObject.activeSelf)
            return;
        Vector2 delta = dir * moveSpeed * Time.unscaledDeltaTime;
        cursorRect.anchoredPosition += delta;

        ClampToCanvas();
    }

    void ClampToCanvas()
    {
        Vector2 canvasSize = canvasRect.rect.size;
        Vector2 cursorSize = cursorRect.rect.size;

        Vector2 min = -canvasSize * 0.5f + cursorSize * 0.5f;
        Vector2 max = canvasSize * 0.5f - cursorSize * 0.5f;

        Vector2 pos = cursorRect.anchoredPosition;
        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);

        cursorRect.anchoredPosition = pos;
    }

    void RaycastForMenuItem()
    {
        if (!gameObject.activeSelf)
            return;
        if (raycaster == null || eventSystem == null)
            return;

        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = RectTransformUtility.WorldToScreenPoint(
                canvas.worldCamera,
                cursorRect.position
            )
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        IMenuItem hitItem = null;

        if (results.Count == 0 && currentItem != null)
        {
            currentItem?.OnHoverExit(playerIndex);
            currentItem = null;
        }

        foreach (var r in results)
        {
            hitItem = r.gameObject.GetComponent<IMenuItem>();
            if (hitItem != null)
                break;
        }

        if (hitItem != currentItem)
        {
            currentItem?.OnHoverExit(playerIndex);
            currentItem = hitItem;
            currentItem?.OnHoverEnter(playerIndex);
        }
    }

    public void OnConfirm()
    {
        //Debug.Log(gameObject);
        if (!gameObject.activeSelf)
            return;
        if (currentItem != null)
        {
            //currentItem.OnConfirm(playerIndex);
            MonoBehaviour mono = currentItem as MonoBehaviour;

            if (mono.GetComponent<QuitMenu>() != null)
            {

                currentItem.OnConfirm(playerIndex);
                if (uiSelectSound.clip != null)
                {
                    uiSelectSound.Play();
                }
                else
                {
                    Debug.Log("Confirm SFX clip is not assigned.");
                }
            }
            if (MenuManager.Instance.isPaused)
                return;

            //MenuManager.Instance.CheckPlayerConfirm(isLocked);
            //isLocked = true;
            //GetComponent<Image>().sprite = selectedSpriteCursor;
            //currentItem.OnConfirm(playerIndex);
            if (mono.GetComponent<DisconnectPlayerButton>() != null)
            {

                currentItem.OnConfirm(playerIndex);
                if (uiSelectSound.clip != null)
                {
                    uiSelectSound.Play();
                }
                else
                {
                    Debug.Log("Confirm SFX clip is not assigned.");
                }
            }


            if (mono.GetComponent<CharacterButton>() != null)
            {
                MenuManager.Instance.CheckPlayerConfirm(isLocked);
                isLocked = true;
                GetComponent<Image>().sprite = selectedSpriteCursor;
                currentItem.OnConfirm(playerIndex);
                if (uiSelectSound.clip != null)
                {
                    uiSelectSound.Play();
                }
                else
                {
                    Debug.Log("Confirm SFX clip is not assigned.");
                }
            }


        }
    }


    public void OnCancel()
    {
        if (!gameObject.activeSelf)
            return;
        MenuManager.Instance.PlayerCancel(isLocked);
        isLocked = false;
        GetComponent<Image>().sprite = defaultSpriteCursor;

        currentItem?.OnHoverExit(playerIndex);
        currentItem = null;
        //currentItem?.OnHoverExit(playerIndex);
        //currentItem = null;

        if (uiSelectSound.clip != null)
        {
            uiSelectSound.Play();
        }
        else
        {
            Debug.Log("Confirm SFX clip is not assigned.");
        }
    }

    public void OnJump() { }
    public void OnKick(bool val) { }
    public void OnSprint(bool val) { }
    public void OnJoin() { }
    public void OnAbility() { }
}