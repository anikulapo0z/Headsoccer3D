using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WordSpawner : MonoBehaviour
{
    [Header("Letter Folder (Resources)")]
    [SerializeField] string letterFolder = "Letters";

    [Header("Layout")]
    [SerializeField] Transform startPosition;
    [SerializeField] float letterSpacing = 1f;
    [SerializeField] float lineSpacing = 2f;
    [SerializeField] float letterScale;
    [SerializeField] Vector3 letterRotation;

    private int currentLine = 0;

    private Dictionary<char, GameObject> letterPrefabs = new Dictionary<char, GameObject>();


    void Awake()
    {
        LoadLetters();
    }

    void LoadLetters()
    {
        GameObject[] letters = Resources.LoadAll<GameObject>(letterFolder);

        foreach (GameObject letter in letters)
        {
            if (letter.name.Length == 1)
            {
                char c = letter.name.ToLower()[0];
                letterPrefabs[c] = letter;
            }
        }
    }


    public void SpawnWord(string text, float timer)
    {
        List<GameObject> spawnedLetters = new List<GameObject>();

        Vector3 lineCenter = startPosition.position + new Vector3(0, -currentLine * lineSpacing, 0);

        int characterCount = text.Length;
        float totalWidth = (characterCount - 1) * letterSpacing;
        float startX = -totalWidth / 2f;

        int index = 0;

        foreach (char rawChar in text)
        {
            char c = char.ToLower(rawChar);

            if (c == ' ')
            {
                index++;
                continue;
            }

            if (letterPrefabs.ContainsKey(c))
            {
                float xOffset = startX + (index * letterSpacing);

                Vector3 spawnPos = lineCenter + new Vector3(xOffset, 0, 0);

                GameObject letterObj = Instantiate(letterPrefabs[c], spawnPos, Quaternion.Euler(letterRotation), transform);
                letterObj.transform.localScale *= letterScale;

                spawnedLetters.Add(letterObj);
            }

            index++;
        }

        currentLine++;

        if (timer != -1)
        {
            StartCoroutine(StartTimer(timer, spawnedLetters));
        }
    }

    IEnumerator StartTimer(float time, List<GameObject> letters)
    {
        yield return new WaitForSeconds(time);

        HandleTimedLetters(letters);
    }


    void HandleTimedLetters(List<GameObject> letters)
    {
        foreach (GameObject letter in letters)
        {
            if (letter != null)
            {
                letter.GetComponent<Rigidbody>().isKinematic = false;
                //Debug.LogError("kjsdfbsjlbhfsldfsd");
            }
        }
    }
}
