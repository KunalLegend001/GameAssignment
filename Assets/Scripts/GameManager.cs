using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Sprite[] frontSprites;
    public Sprite backSprite;

    public int columns = 2;
    public int rows = 2;

    private List<Card> flippedCards = new List<Card>();
    private int score = 0;

    void Start()
    {
        GenerateCards();
    }

    void GenerateCards()
    {
        List<int> ids = new List<int>();

        for (int i = 0; i < (rows * columns) / 2; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        ids.Shuffle(); // We'll create this extension below.

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

    public void CardFlipped(Card card)
    {
        if (!flippedCards.Contains(card))
        {
            flippedCards.Add(card);

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
            score += 10;
            // Play match sound
            matches++;
            UpdateUI();
        }
        else
        {
            flippedCards[0].FlipBack();
            flippedCards[1].FlipBack();
            // Play mismatch sound
        }

        flippedCards.Clear();
    }



    public int turns = 0;
    public int matches = 0;

    public TMPro.TextMeshProUGUI turnsText;
    public TMPro.TextMeshProUGUI matchesText;

    public void OnCardClicked(Card card)
    {
        if (!flippedCards.Contains(card))
        {
            flippedCards.Add(card);
            turns++;
            UpdateUI();

            if (flippedCards.Count == 2)
                StartCoroutine(CheckMatch());
        }
    }

    void UpdateUI()
    {
        turnsText.text = "Turns: " + turns;
        matchesText.text = "Matches: " + matches;
    }

}
