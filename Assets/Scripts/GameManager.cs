using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Card Settings")]
    public GameObject cardPrefab;
    public Sprite[] frontSprites;
    public Sprite backSprite;
    public int columns = 2;
    public int rows = 2;

    [Header("UI Elements")]
    public TMPro.TextMeshProUGUI turnsText;
    public TMPro.TextMeshProUGUI matchesText;
    public GameObject winPanel;

    private List<Card> flippedCards = new List<Card>();
    private int score = 0;

    [Header("Progress Tracking")]
    public int turns = 0;
    public int matches = 0;

    void Start()
    {
        LoadProgress();
        GenerateCards();
        UpdateUI();
        winPanel.SetActive(false);
    }

    void GenerateCards()
    {
        List<int> ids = new List<int>();

        for (int i = 0; i < (rows * columns) / 2; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        ids.Shuffle();

        float spacingX = 2f;
        float spacingY = 2.5f;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int index = i * columns + j;
                GameObject cardObj = Instantiate(cardPrefab, new Vector3(j * spacingX, -i * spacingY, 0), Quaternion.identity);
                Card card = cardObj.GetComponent<Card>();
                card.cardId = ids[index];
                card.frontSprite = frontSprites[ids[index]];
                card.backSprite = backSprite;
            }
        }
    }

    public void OnCardClicked(Card card)
    {
        if (!flippedCards.Contains(card))
        {
            flippedCards.Add(card);
            turns++;
            UpdateUI();

            if (flippedCards.Count == 2)
            {
                StartCoroutine(CheckMatch());
            }
        }
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f);

        if (flippedCards[0].cardId == flippedCards[1].cardId)
        {
            matches++;
            UpdateUI();
            AudioManager.Instance.PlayCardMatch();

            yield return new WaitForSeconds(0.3f);
            Destroy(flippedCards[0].gameObject);
            Destroy(flippedCards[1].gameObject);

            CheckWinCondition();
        }
        else
        {
            AudioManager.Instance.PlayCardMismatch();
            flippedCards[0].FlipBack();
            flippedCards[1].FlipBack();
        }

        flippedCards.Clear();
    }

    void UpdateUI()
    {
        turnsText.text = "Turns: " + turns;
        matchesText.text = "Matches: " + matches;
        SaveProgress();
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt("turns", turns);
        PlayerPrefs.SetInt("matches", matches);
        PlayerPrefs.Save();
    }

    void LoadProgress()
    {
        turns = PlayerPrefs.GetInt("turns", 0);
        matches = PlayerPrefs.GetInt("matches", 0);
    }

    void CheckWinCondition()
    {
        int totalPairs = (rows * columns) / 2;
        if (matches >= totalPairs)
        {
            winPanel.SetActive(true);
            AudioManager.Instance.StopMusic(); // Optional
        }
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("turns");
        PlayerPrefs.DeleteKey("matches");
        turns = 0;
        matches = 0;
        UpdateUI();
    }
}
