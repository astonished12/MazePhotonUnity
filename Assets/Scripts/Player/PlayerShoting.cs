﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoting : MonoBehaviour {

    private float fireRate = 0.5f;
    private float cooldown = 0f;
    private AudioSource playerAudio;                                   // Reference to the AudioSource component.
    public AudioClip shotClip;
    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
    }

    void Update()
    {

        cooldown -= Time.deltaTime;
        if (Input.GetButton("Fire1"))
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (cooldown > 0)
        {
            return;
        }

        Debug.Log("Fire our gun");
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;
        hitInfo = FindClosetHitInfo(ray);
        if (hitInfo.collider != null)
        {
            Debug.Log("we hit " + hitInfo.transform.name);
            Health h = hitInfo.transform.GetComponent<Health>();
            playerAudio.clip = shotClip;
            playerAudio.Play();
            h.GetComponent<PhotonView>().RPC("TakeDamage",PhotonTargets.All,10);
        }

        cooldown = fireRate;
    }

    private RaycastHit FindClosetHitInfo(Ray ray)
    {
        RaycastHit[] raycasts = Physics.RaycastAll(ray);
        float distance = 0f;
        RaycastHit closet = new RaycastHit();

        foreach (RaycastHit hit in raycasts)
        {
            Debug.Log(hit.collider.gameObject.name+" "+hit.distance);
            if (closet.collider == null||(hit.distance < distance)){
                closet = hit;
                distance = hit.distance;
                
            }
        }

        return closet;

    }
}
