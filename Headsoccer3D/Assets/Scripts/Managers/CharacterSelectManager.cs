using System.Collections;
using TMPro;
using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager Instance;
    public int totalPlayerCount = 0;
    [SerializeField] int lockedPlayerCount = 0;
    [SerializeField] bool canMoveToNextScreen = false;

    [SerializeField] int currentCountDownTime;
    [SerializeField] int maxCountDownTime = 30;
    [SerializeField] TMP_Text countDownText;

    Coroutine countDownCoroutine;

    [SerializeField] GameObject mapSelectCanvas;
    [SerializeField] GameObject characterSelectCanvas;
    [SerializeField] GameObject pressConfirmPrompt;

    private void Start()
    {
        Instance = this;
    }

    public void StartCountDown()
    {
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
        MoveToNextScreen();
    }

    public void PlayerJoined(int count)
    {
        totalPlayerCount = count;
        canMoveToNextScreen = false;
        pressConfirmPrompt.SetActive(false);
    }

    public void CheckPlayerConfirm(bool isLocked)
    {
        if (!isLocked)
        {
            lockedPlayerCount++;
            if (lockedPlayerCount == totalPlayerCount)
            {
                canMoveToNextScreen = true;
                pressConfirmPrompt.SetActive(true);
            }
            return;
        }

        if(canMoveToNextScreen)
        {
            MoveToNextScreen();
            StopCoroutine(countDownCoroutine);
        }
    }
    public void PlayerCancel(bool isLocked)
    {
        if (isLocked)
        {
            if (canMoveToNextScreen)
            {
                canMoveToNextScreen = false;
                pressConfirmPrompt.SetActive(false);
            }
            lockedPlayerCount--;
        }
    }


    void MoveToNextScreen()
    {
        characterSelectCanvas.SetActive(false);
        mapSelectCanvas.SetActive(true);

        mapSelectCanvas.GetComponent<MapSelectionManager>().SetPlayers();
    }

}
