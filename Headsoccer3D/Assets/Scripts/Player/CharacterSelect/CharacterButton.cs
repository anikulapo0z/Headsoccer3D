using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public int characterID;

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
    public RectTransform selectionRight;

}
