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
    public int columns;
    public int rows;

    [Header("UI Elements")]
    public TMPro.TextMeshProUGUI turnsText;
    public TMPro.TextMeshProUGUI matchesText;
    public GameObject winPanel;

    private List<Card> flippedCards = new List<Card>();
    private List<GameObject> activeCards = new List<GameObject>(); // Track active cards

    private int turns = 0;
    private int matches = 0;

    void Start()
    {
        LoadProgress();
        GenerateCards();
        UpdateUI();
        winPanel.SetActive(false);
    }

    void GenerateCards()
    {
        int totalCards = rows * columns;

        if (totalCards % 2 != 0)
        {
            Debug.LogError("Total cards must be even!");
            return;
        }

        if (frontSprites.Length < totalCards / 2)
        {
            Debug.LogError($"Not enough frontSprites! Need at least {totalCards / 2}, but found {frontSprites.Length}.");
            return;
        }

        List<int> ids = new List<int>();
        for (int i = 0; i < totalCards / 2; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        ids.Shuffle();

        Transform spawnRoot = GameObject.Find("SpawnRoot")?.transform;
        if (spawnRoot == null)
        {
            Debug.LogError("SpawnRoot not found!");
            return;
        }

        float maxWidth = 8f;
        float maxHeight = 6f;

        float cardWidth = maxWidth / columns;
        float cardHeight = maxHeight / rows;

        float startX = -maxWidth / 2 + cardWidth / 2;
        float startY = maxHeight / 2 - cardHeight / 2;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int index = i * columns + j;
                GameObject cardObj = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity, spawnRoot);

                float posX = startX + j * cardWidth;
                float posY = startY - i * cardHeight;

                cardObj.transform.localPosition = new Vector3(posX, posY, 0);
                cardObj.transform.localScale = Vector3.one * 0.9f;

                Card card = cardObj.GetComponent<Card>();
                int id = ids[index];
                card.cardId = id;
                card.frontSprite = frontSprites[id];
                card.backSprite = backSprite;

                activeCards.Add(cardObj); // Track the card
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

            GameObject cardA = flippedCards[0].gameObject;
            GameObject cardB = flippedCards[1].gameObject;

            activeCards.Remove(cardA);
            activeCards.Remove(cardB);

            Destroy(cardA);
            Destroy(cardB);

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

    void CheckWinCondition()
    {
        if (activeCards.Count == 0)
        {
            winPanel.SetActive(true);
            AudioManager.Instance.StopMusic();
        }
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

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("turns");
        PlayerPrefs.DeleteKey("matches");
        turns = 0;
        matches = 0;
        UpdateUI();
    }
}
