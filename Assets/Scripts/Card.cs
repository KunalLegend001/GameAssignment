using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardId;
    public Sprite frontSprite;
    public Sprite backSprite;

    private SpriteRenderer sr;
    private bool isFlipped = false;

    private GameManager gameManager;

    public Transform spriteTransform;

    void Start()
    {
        sr = spriteTransform.GetComponent<SpriteRenderer>();
        sr.sprite = backSprite;
        gameManager = FindObjectOfType<GameManager>();
        NormalizeSpriteSize();
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
        NormalizeSpriteSize(); // In case aspect ratio differs
    }

    public void FlipBack()
    {
        isFlipped = false;
        sr.sprite = backSprite;
        NormalizeSpriteSize();
    }

    void NormalizeSpriteSize()
    {
        if (sr.sprite == null) return;

        Vector2 size = sr.sprite.bounds.size;
        float targetSize = 1.5f; // how big each card appears on screen (adjust to fit layout)

        float scaleFactor = targetSize / Mathf.Max(size.x, size.y);
        spriteTransform.localScale = Vector3.one * scaleFactor;
    }
}
