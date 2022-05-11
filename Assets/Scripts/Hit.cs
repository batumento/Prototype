using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent (typeof(Rigidbody))]
public class Hit : MonoBehaviour
{
    private Transform owner;
    private int damage;
    private Collider hitColider;
    private Rigidbody rb;
    private Animator anim;
    private void Awake()
    {
        owner = transform.root;
        hitColider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();

        hitColider.isTrigger = true;
        hitColider.enabled = false;
        rb.isKinematic = true;
        rb.useGravity = false;
    }
    private void Start()
    {
        if (owner.tag == "Player")
        {
           damage = owner.GetComponent<AttackController>().GetDamage();
            anim = GetComponentInParent<Transform>().GetComponentInParent<Animator>();
        }
        else if (owner.tag == "Enemy")
        {
            damage = owner.GetComponent<EnemyController>().GetDamage();
            anim = GetComponentInParent<Animator>();
        }
        else
        {
            enabled = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null && health.gameObject != owner.gameObject)
        {
            health.GiveDamage(damage);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.55f)//0.05f fazlalýk koyduk Ontrigger 1 kere çalýþýcak zaten ama biz ona þans tanýmýþýz gibi düþünebiliriz.
        {
            ControlTheCollider(true);
        }
        else
        {
            ControlTheCollider(false);
        }
    }
    private void ControlTheCollider(bool open)
    {
        hitColider.enabled = open;
    }
}
