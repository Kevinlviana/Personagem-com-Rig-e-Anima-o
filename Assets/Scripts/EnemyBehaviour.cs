using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] Rigidbody enemyRb;
    [SerializeField] Animator animator;
    float speed = 1;
    float attackDistance = 1.5f;

    private void Update()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) < attackDistance)
        {
            animator.SetBool("IsAttacking", true);
        }
        else
        {
            animator.SetBool("IsAttacking", false);
        }
    }

    private void FixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Attack")) return;

        if (playerTransform != null)
        {
            Vector3 direction = playerTransform.position - transform.position;
            direction.y = 0;

            transform.forward = direction.normalized;
            enemyRb.linearVelocity = transform.forward * speed;
        }
    }
}
