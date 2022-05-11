using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private Animator anim;

    [Header("Move Settings")]
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float turnSpeed = 15f;
    [SerializeField] float patrolRadius = 6f;
    [SerializeField] float patrolWaitTime = 2f;
    [SerializeField] float chaseSpeed = 4f;
    [SerializeField] float searchSpeed = 3f;
    [Header("Attack Settings")]
    [SerializeField] int damage = 2;
    [SerializeField] float attackRate = 2f;
    [SerializeField] float attackRange = 1.5f;


    private bool isSearched = false;
    private bool isAttacking = false;

    enum State
    {
        Idle,
        Search,
        Chase,
        Attack
    }
    [SerializeField] private State currentState = State.Idle;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        StateCheck();
        StateExecute();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,chaseRange);
    }

    private void StateCheck()
    {
        float distanceToTarget = Vector3.Distance(player.position, transform.position);
        if (distanceToTarget <= chaseRange && distanceToTarget > attackRange)
        {
            currentState = State.Chase;
        }
        else if (distanceToTarget <= attackRange)
        {
            currentState=State.Attack;
        }
        else
        {
            currentState = State.Search;
        }
        
    }

    private void StateExecute()
    {
        switch (currentState)
        {
            case State.Idle:
                break;
            case State.Search:
                if (!isSearched && agent.remainingDistance <= 0.1f ||
                    !agent.hasPath && !isSearched)
                {
                    Vector3 agentTarget = new Vector3(agent.destination.x, transform.position.y, agent.destination.z);
                    agent.enabled = false;
                    transform.position = agentTarget;//Yukarýdaki koþul saðlanmasý için ek güvenlik daha sonra tekrar bak
                    agent.enabled = true;

                    Invoke("Search", patrolWaitTime);
                    anim.SetBool("Walk",false);
                    isSearched = true;
                }
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
        }
    }
    private void Search()
    {
        agent.isStopped = false;
        agent.speed = searchSpeed;
        isSearched = false;
        anim.SetBool("Walk",true);
        agent.SetDestination(GetRandomPosition());
    }
    private void Attack()
    {
        if (player == null)
        {
            return;
        }
        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
        anim.SetBool("Walk", false);
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        LookTheTarget(player.position);
    }

    private void Chase()
    {
        if (player == null)
        {
            return;
        }
        agent.isStopped=false;
        agent.speed = chaseSpeed;
        anim.SetBool("Walk", true);
        agent.SetDestination(player.position);
    }
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackRate);
        anim.SetTrigger("Attack");
        yield return new WaitUntil(IsAttackAnimationFinished);
        isAttacking=false;
    }
    private bool IsAttackAnimationFinished()
    {
        if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")
            && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void LookTheTarget(Vector3 target)
    {
        Vector3 lookpos = new Vector3(target.x,transform.position.y,target.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookpos - transform.position),
            turnSpeed * Time.deltaTime); //Enemy Oyuncuya bakmasý için oluþturulan Metot.
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1);
        return hit.position;
    }
    public int GetDamage()
    {
        return damage;
    }
}
