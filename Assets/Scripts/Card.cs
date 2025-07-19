using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardId;
    public Sprite frontSprite;
    public Sprite backSprite;

    private SpriteRenderer sr;
    private bool isFlipped = false;

    private GameManager gameManager;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = backSprite;
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnMouseDown()
    {
        if (!isFlipped)
        {
            FlipCard();
            gameManager.CardFlipped(this);
        }
    }

    public void FlipCard()
    {
        isFlipped = true;
        sr.sprite = frontSprite;
    }

    public void FlipBack()
    {
        isFlipped = false;
        sr.sprite = backSprite;
    }

    public bool IsFlipped()
    {
        return isFlipped;
    }
}
