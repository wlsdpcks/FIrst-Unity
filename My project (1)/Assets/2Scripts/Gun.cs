using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public Transform fpsCam;

    public enum Type { Pistol, Rifle, Heavy, Sniper };
    public Type type;

    public float range = 20;
    public float impactForce = 150;

    public int fireRate = 1;
    public float nextTimeToFire = 0;
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public ParticleSystem muzzleFlush;
    public GameObject impactEffect;

    InputAction shoot;
    public Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        shoot = new InputAction("Shoot", binding: "<mouse>/leftButton");
        shoot.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        bool isShooting = shoot.ReadValue<float>() == 1;
        anim.SetBool("isShooting", isShooting);
        if (isShooting && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Fire();
        }
    }
    void Fire()
    {
        RaycastHit hit;
        muzzleFlush.Play();
        if(Physics.Raycast(fpsCam.position, fpsCam.forward, out hit, range))
        {
            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }
        }

        Quaternion impactRotation = Quaternion.LookRotation(hit.normal);
        GameObject impact = Instantiate(impactEffect, hit.point, impactRotation);
        impact.transform.parent = hit.transform;
        Destroy(impact, 4);

    }

}
