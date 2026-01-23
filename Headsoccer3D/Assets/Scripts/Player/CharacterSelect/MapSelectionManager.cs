using System.Collections.Generic;
using UnityEngine;

public class MapSelectionManager : MonoBehaviour
{
    [SerializeField] RectTransform startingMapSelection;
    [SerializeField] List<GameObject> mapCursors = new List<GameObject>();
    public List<PlayerInputController> inputControllers = new List<PlayerInputController>();






    public void SetPlayers()
    {
        foreach (PlayerInputController i in inputControllers)
        {
            IPlayerControllable cursor = CreateCursor(i.PlayerIndex);
            print(i.name);
            i.SetControlledObject(cursor);
        }
    }

    IPlayerControllable CreateCursor(int playerIndex)
    {
        GameObject cursorObj = Instantiate(mapCursors[playerIndex], startingMapSelection);

        MapSelectionCursor cursor = cursorObj.GetComponent<MapSelectionCursor>();
        if (cursor == null)
        {
            Debug.LogError("Cursor prefab missing!");
            return null;
        }

        cursorObj.transform.SetParent(startingMapSelection, false);
        cursorObj.transform.localPosition = Vector3.zero;
        
        cursor.parent = startingMapSelection;

        return cursor;

    }
}
