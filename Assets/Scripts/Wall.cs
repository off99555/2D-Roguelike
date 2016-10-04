using UnityEngine;

public class Wall : MonoBehaviour
{
    public AudioClip chopSound1;
    public AudioClip chopSound2;
    public Sprite dmgSprite;
    public int hp = 4;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int loss)
    {
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
        hp -= loss;
        spriteRenderer.sprite = dmgSprite;
        if (hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}