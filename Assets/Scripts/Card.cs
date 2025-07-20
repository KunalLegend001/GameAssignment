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

    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
#else
        HandleMouseInput();
#endif
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(worldPoint);
            if (hit != null && hit.transform == this.transform)
            {
                TryFlipCard();
            }
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Collider2D hit = Physics2D.OverlapPoint(worldPoint);
            if (hit != null && hit.transform == this.transform)
            {
                TryFlipCard();
            }
        }
    }

    void TryFlipCard()
    {
        if (!isFlipped)
        {
            FlipCard();
            AudioManager.Instance.PlayCardFlip();
            gameManager.OnCardClicked(this);
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
        yield return new WaitForSeconds(0.75f); // Mid-flip
        sr.sprite = backSprite;
        NormalizeSpriteSize();
        yield return new WaitForSeconds(0.25f); // Finish animation
        isFlipped = false;
    }

    IEnumerator ChangeSpriteAfterDelay()
    {
        yield return new WaitForSeconds(0.85f); // Mid-flip delay
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
