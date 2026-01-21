using UnityEngine;

public class CharacterButton : MonoBehaviour
{
    // attached to each character selection
    public int characterID;

    // for cursor navigation
    public RectTransform selectionAbove;
    public RectTransform selectionBelow;
    public RectTransform selectionLeft;
    public RectTransform selectionRight;

}
