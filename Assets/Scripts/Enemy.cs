using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{

    public int damageToPlayer;
    public float hp;
    public float maxHP;
    public HealthbarBehaviour healthbar;


    private Animator animator;
    private Transform target;
    private bool skipMove;
    private bool isBleeding = false;
    private GameObject player;
    
    

    // Music variables

    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;


    // Add this enemy to the current enemy list, set the hp to full (MaxHP), get a reference to the animator and set player variable as the target. Call MovingObject Start(). Adapted from code written by Matt Schell (2022).

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);

        animator = GetComponent<Animator>();

        target = GameObject.FindGameObjectWithTag("Player").transform;

        hp = maxHP;

        healthbar.Sethealth(hp, maxHP);

        player = GameObject.Find("Player");

        base.Start();

    }

    // Checks to see if its enemy's turn to move and then passes through directions to base class MovingObject function, also checks if bleeding and so decreases enemy health and removes enemy from list if killed. Adapted from code written by Matt Schell (2022).
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;

        if(isBleeding)
        {
            hp -= 1;

            healthbar.Sethealth(hp, maxHP);

            if (hp <= 0)
            {

                gameObject.SetActive(false);

                GameManager.instance.DeleteEnemyFromList(this);

            }
        }
    }

    // Called by GameManager when it is the enemy's turn to move. Adapted from code written by Matt Schell (2022).

    public void MoveEnemy()
    {
        if (GameManager.noMovement == false)
        {
            int xDir = 0;
            int yDir = 0;

            if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)

            {
                yDir = target.position.y > transform.position.y ? 1 : -1;

            }
            else
            {
                xDir = target.position.x > transform.position.x ? 1 : -1;
            }

            AttemptMove<Player>(xDir, yDir);
        }
    }


    // If enemy can't move into a grid position it will attempt to attack the player. Plays all relevant UI effects. Adapted from code written by Matt Schell (2022).

    protected override void onCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
 
        hitPlayer.LoseEnergy(damageToPlayer);

        animator.SetTrigger("enemyAttack");

        SoundManager.instance.RandomiseSfx(enemyAttack1, enemyAttack2);
    }


    // Function called from Player if enemy is attacked with weapon. Plays all relevant UI effects. Checks to see if game is won by defeating demonBoss.

    public void HitByPlayer (int loss)

    {
        hp -= loss;

        healthbar.Sethealth(hp, maxHP);

        if (hp<= 0)
        {

            gameObject.SetActive(false);

            GameManager.instance.DeleteEnemyFromList(this);

            player.GetComponent<Player>().EnergyOnKill();

            if (this.CompareTag("demonBoss"))
            {
                GameManager.instance.GameWon();

            }

        }

    }


    // Function called if player uses club weapon. Checks with RaycastHit if enemy will collide with anything with a collider, if it won't then move enemy in passed through direction.

    public void ClubAttack (char direction, int moveBy)
    {

        Vector2 checkingDir = transform.position;


        if  (direction == 'x')
            {

            float newX = (float)(checkingDir.x + moveBy);

            checkingDir.x = newX;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, checkingDir,1f);

            if (hit.collider == null)

            {
                transform.Translate(moveBy, 0.0f, 0.0f);
            }


            }

        if (direction == 'y')
        {
            float newY = (float)(checkingDir.y + moveBy);

            checkingDir.y = newY;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, checkingDir,1f);

            if (hit.collider == null)
            {
                transform.Translate(0.0f, moveBy, 0.0f);

            }
        }


     }

    // Function called if player uses axe weapon. Sets this enemy with bleed effect.

    public void AxeAttack()
    {

        isBleeding = true;

        healthbar.BleedEffect();

    }

    
}
