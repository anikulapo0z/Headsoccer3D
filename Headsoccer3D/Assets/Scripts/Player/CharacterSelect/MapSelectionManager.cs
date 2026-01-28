using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectionManager : MonoBehaviour
{
    [SerializeField] RectTransform startingMapSelection;
    [SerializeField] List<GameObject> mapCursors = new List<GameObject>();
    public List<PlayerInputController> inputControllers = new List<PlayerInputController>();

    float currentCountDownTime;
    [SerializeField] float maxCountDownTime;
    Coroutine countDownCoroutine;
    [SerializeField] TMP_Text countDownText;
    public string selectedScene = "";
    public static MapSelectionManager Instance;
    [SerializeField] int lockedPlayerCount = 0;
    [SerializeField] int totalPlayerCount = 0;
    [SerializeField] string menuSceneName;


    bool canMoveToNextScene = false;

    private void Awake()
    {
        Instance = this;
    }

    public void SetPlayers()
    {
        foreach (PlayerInputController i in inputControllers)
        {
            IPlayerControllable cursor = CreateCursor(i.PlayerIndex);
            i.SetControlledObject(cursor);
        }
        StartCountDown();
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
        cursor.SetStartValue();

        return cursor;

    }

    public void StartCountDown()
    {
        totalPlayerCount = inputControllers.Count;
        currentCountDownTime = maxCountDownTime;
        countDownCoroutine = StartCoroutine(CountDownRoutine());
    }

    IEnumerator CountDownRoutine()
    {
        while (currentCountDownTime > 0)
        {
            currentCountDownTime--;
            countDownText.text = currentCountDownTime.ToString();
            yield return new WaitForSeconds(1);
        }
        StopCoroutine(countDownCoroutine);
        LoadScene(selectedScene);
    }

    void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
        SceneManager.UnloadSceneAsync(menuSceneName);
    }






    public void CheckPlayerConfirm(bool isLocked)
    {
        if (!isLocked)
        {
            lockedPlayerCount++;
            if (lockedPlayerCount == totalPlayerCount)
            {
                canMoveToNextScene = true;
                //pressConfirmPrompt.SetActive(true);
            }
            return;
        }

        if (canMoveToNextScene)
        {
            LoadScene(selectedScene);
            StopCoroutine(countDownCoroutine);
        }
    }

    public void PlayerCancel(bool isLocked)
    {
        if (isLocked)
        {
            if (canMoveToNextScene)
            {
                canMoveToNextScene = false;
                //pressConfirmPrompt.SetActive(false);
            }
            lockedPlayerCount--;
        }
    }

}
