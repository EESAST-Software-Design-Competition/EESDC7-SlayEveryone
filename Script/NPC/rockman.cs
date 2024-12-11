using Unity.VisualScripting;
using UnityEngine;

public class Rockman : MonoBehaviour
{
    public float moveSpeed = 3f; 
    public float detectionRange = 10f; 
    public float attackRange = 2f; 
    public float stopChaseRange = 15f; 
    public Transform player; 
    public Animator animator; 
    public AudioSource audioSource; 
    private bool isChasing = false; 
    private bool checkMove = false; 
    private bool checkAttack = false; 

    private void Update()
    {
        checkMove=false;
        checkAttack=false;

        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            isChasing = true;
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                Attack();
            }
            else
            {
                Chase();
            }
        }
        else
        {

            isChasing = false;
        }

        if (GetComponent<Rigidbody>().velocity.magnitude != 0)
        {
            checkMove = true;
        }
        else
        {
            checkMove = false;
        }
    }

    private void FixedUpdate()
    {
        if (isChasing)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 targetDirection = (player.position - transform.position).normalized;

        targetDirection.y = 0;

        GetComponent<Rigidbody>().velocity = targetDirection * moveSpeed;
    }

    private void Chase()
    {
        if (animator != null)
        {
            animator.SetBool("checkmove", checkMove);
            
        }

        if (audioSource != null)
        {
            // audioSource.PlayOneShot(chaseSound);
        }
    }

    private void Attack()
    {
        if (animator != null)
        {
            // ²¥·Å¹¥»÷¶¯»­
            animator.SetBool("checkattack",checkAttack);
        }

        if (audioSource != null)
        {
            // audioSource.PlayOneShot(attackSound);
        }

        checkAttack = true;
    }
}