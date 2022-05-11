using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class AttackController : MonoBehaviour
{
    [SerializeField] Weapon currentWeapon;
    private Transform mainCamera;
    private bool isAttacking = false;
    private Animator animator;
    private void Awake()
    {

        mainCamera = GameObject.FindWithTag("CameraPoint").transform;

        animator = mainCamera.GetChild(0).GetComponent<Animator>();
        /* Bir Gameobjectin çocuðunun Componentlerine eriþmek için böyle bir yöntem denedim ve oldu yukardaki de alternatif yol
        playerChild = player.transform.GetChild(0).GetChild(0).gameObject;
        animator = mainCamera.GetChild(0).GetComponent<Animator>();*/
        if (currentWeapon != null)
        {
            SpawnWeapon();

        }
    }
    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    private void Attack()
    {
        if (Mouse.current.leftButton.isPressed && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private void SpawnWeapon()
    {
        if (currentWeapon == null)
        {
            return;
        }
        currentWeapon.SpawnNewWeapon(mainCamera.transform.GetChild(0).GetChild(0),animator);
    }

    public void EquipWeapon(Weapon weapontype)
    {
        if (currentWeapon != null)
        {
            currentWeapon.Drop();
        }
        currentWeapon = weapontype;
        SpawnWeapon();
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(currentWeapon.GetAttackRate);
        isAttacking=false;
    }
    public int GetDamage()
    {
        if (currentWeapon != null)
        {
            return currentWeapon.GetDamage;
        }
        return 0;
    }
}
