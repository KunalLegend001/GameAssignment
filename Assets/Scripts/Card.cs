using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardId;
    public Sprite frontSprite;
    public Sprite backSprite;

    private SpriteRenderer sr;
    public Transform spriteTransform;
    private bool isFlipped = false;

    private GameManager gameManager;
    private Animator animator;

    void Start()
    {
        sr = spriteTransform.GetComponent<SpriteRenderer>();
        animator = spriteTransform.GetComponent<Animator>();
        sr.sprite = backSprite;
        gameManager = FindObjectOfType<GameManager>();
        NormalizeSpriteSize();
    }

    public void OnMouseDown()
    {
        if (!isFlipped)
        {
            FlipCard();
            gameManager.OnCardClicked(this); // new method to track turns/matches
        }
    }

    public void FlipCard()
    {
        animator.SetTrigger("Flip"); // Trigger the animation
        StartCoroutine(ChangeSpriteAfterDelay());
        isFlipped = true;
    }

    public void FlipBack()
    {
        StartCoroutine(FlipBackRoutine());
    }
    IEnumerator FlipBackRoutine()
    {
        animator.SetTrigger("Flip"); // Reuse the same animation

        yield return new WaitForSeconds(1f); // Mid-flip
        sr.sprite = backSprite;
        NormalizeSpriteSize();

        yield return new WaitForSeconds(0.25f); // Finish animation
        isFlipped = false;
    }


    IEnumerator ChangeSpriteAfterDelay()
    {
        yield return new WaitForSeconds(1f); // mid-flip point
        sr.sprite = frontSprite;
        NormalizeSpriteSize();
    }

    void NormalizeSpriteSize()
    {
        if (sr.sprite == null) return;

        Vector2 size = sr.sprite.bounds.size;
        float targetSize = 1.5f;

        float scaleFactor = targetSize / Mathf.Max(size.x, size.y);
        spriteTransform.localScale = Vector3.one * scaleFactor;
    }
}
