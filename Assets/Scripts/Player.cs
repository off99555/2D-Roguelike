using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    private Animator animator;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public Text foodText;
    public AudioClip gameOverSound;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public int wallDamage = 1;

    public int Food
    {
        get { return GameManager.instance.playerFoodPoints; }
        set { GameManager.instance.playerFoodPoints = value; }
    }

    protected override int FacingDirectionOnStart
    {
        get { return 1; }
    }

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        base.Start();
    }

    protected override void AttemptMove<T>(Vector2 displacement)
    {
        Food--;
        foodText.text = "Food: " + Food;
        base.AttemptMove<T>(displacement);
        RaycastHit2D hit;
        if (Move(displacement, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else
        {
            if (other.CompareTag("Food"))
            {
                Food += pointsPerFood;
                foodText.text = "+" + pointsPerFood + " Food: " + Food;
                SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            }
            else if (other.CompareTag("Soda"))
            {
                Food += pointsPerSoda;
                foodText.text = "+" + pointsPerFood + " Food: " + Food;
                SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            }
            other.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        var hitWall = component as Wall;
        hitWall.TakeDamage(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        Food -= loss;
        foodText.text = "-" + loss + " Food: " + Food;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (Food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.GameOver();
        }
    }

    private void Update()
    {
        if (GameManager.instance.playersTurn)
        {
            var horizontal = 0;
            var vertical = 0;
            horizontal = (int) Input.GetAxisRaw("Horizontal");
            vertical = (int) Input.GetAxisRaw("Vertical");
            if (horizontal != 0)
            {
                vertical = 0;
            }
            if (horizontal != 0 || vertical != 0)
            {
                AttemptMove<Wall>(new Vector2(horizontal, vertical));
            }
        }
    }
}