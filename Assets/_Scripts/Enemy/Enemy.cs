using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference: https://youtu.be/sPiVz1k-fEs
// Reference: https://youtu.be/d002CljR-KU
// This video was used to understand and apply attack damage and how to play the necessary animations

public class Enemy : MonoBehaviour
{

    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int points = 10;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackRate = 0.6f;
    [SerializeField] private float range;
    [SerializeField] private float colliderRange;
    //[SerializeField] private int attackDamage = 1;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] SoundManager soundManager;
    private int currHealth;
    private float attackTime = Mathf.Infinity;
    private PlayerHealth playerHealth;
    private Animator enemyAnimator;


    public int EnemyPoints { get {return points;}}
    
    // Acts as a function container 
    public delegate void KilledEnemy(Enemy em);

    // A listener method, which is trigger when an enemy is killed by the player
    // Can hold any function that matches the KilledEnemy signature 
    public static KilledEnemy KilledEnemyEvent;

    // Start is called before the first frame update
    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        currHealth = maxHealth;
    }

    void Update()
    {
        attackTime += Time.deltaTime;

        // If: The player is in the line of sight
        if (PlayerDetected())
        {
            // Attack when cooldown has finished
            if (attackTime >= attackRate)
            {
                processAttack();
            }
        }

    }

    // Function: When called, deals damage to the Enemy
    public void TakeDamage(int damage)
    {
        currHealth -= damage;
        Debug.Log(currHealth);

        enemyAnimator.SetTrigger("Hurting");

        if (currHealth <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        enemyAnimator.SetBool("Dying", true);

        // Play audio
        soundManager.PlayClip("Enemy Death");

        // Set the enemy to a layer that the player does not collide with
        gameObject.layer = 12;

        // Disable the current script on the GameObject
        this.enabled = false;
        GetComponent<EnemyController>().enabled = false;

        // Send a call to the event
        SendKilledEnemyEvent();
    }

    private void SendKilledEnemyEvent()
    {
        if(KilledEnemyEvent != null)
        {
            KilledEnemyEvent(this);
        }
    }

    private void processAttack()
    {
       // Attack
       enemyAnimator.SetTrigger("Attacking");
    }

    // Determines if player is in the enemies line of sight 
    private bool PlayerDetected()
    {
        Vector3 boxSize = new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y,boxCollider.bounds.size.z);

        // Creates a boxcast based on enemy position (collider range and boxSize allows to customize the size of the raycast field through the inspector)
        RaycastHit2D detect = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderRange,
        boxSize, 0, Vector2.left, 0, playerLayer);

        if(detect.collider != null)
        {
           playerHealth = detect.transform.GetComponent<PlayerHealth>();
        }

        return detect.collider;
    }

    // Draws a wireframe on the box collider of the enemy
    private void OnDrawGizmos()
    {
        Vector3 boxSize = new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y,boxCollider.bounds.size.z);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderRange, boxSize);
    }

    public void HurtPlayer()
    {
        if(PlayerDetected())
        {
            // Decrease player health
            playerHealth.TakeDamage();
        }
    }
}
