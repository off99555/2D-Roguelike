using UnityEngine;

public class Enemy : MovingObject
{
    private Animator animator;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;
    public int playerDamage;
    private bool skipMove;
    private Transform target;

    protected override int FacingDirectionOnStart
    {
        get { return -1; }
    }


    // Use this for initialization
    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;
        base.Start();
    }

    protected override void OnCantMove<T>(T component)
    {
        var hitPlayer = component as Player;
        hitPlayer.LoseFood(playerDamage);
        animator.SetTrigger("enemyAttack");
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }

    protected override void AttemptMove<T>(Vector2 displacement)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(displacement);
        skipMove = true;
    }

    public void MoveEnemy()
    {
        var movement = new Vector2();
        Vector2 displacement = target.position - transform.position;
        if (Mathf.Abs(displacement[0]) > Mathf.Abs(displacement[1])) // if X distance is larger than Y distance
        {
            movement[0] = Mathf.Sign(displacement[0]); // move horizontally
        }
        else
        {
            movement[1] = Mathf.Sign(displacement[1]); // move vertically
        }
//        Debug.Log("Enemy Movement: " + movement);
        AttemptMove<Player>(movement);
    }
}