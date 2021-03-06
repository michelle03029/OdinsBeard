﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    // Set time sword is held after being swung
    public float holdTime = 1.5f;
    public float swordSpeed = 200.0f;

    public bool attacking;
    public bool holding;
    public bool pinwheeling;
    private bool fired;

    // Create distance, time, direction, attacking, and sword transform variables
    private float distance;
    private float timer;
    private Transform swordBox;
    private Transform pinwheelBox;

    private SpriteRenderer spriteRenderer;

    // Create variable to hold the rotation direction
    private Vector3 attackDir;

    // Create a variable to hold the collider of the sword
    private BoxCollider2D swordCollider;
    private BoxCollider2D pinwheelCollider;

    // Create variables to track the current sword and target angles
    private float swordAngle;
    private int targetAngle;

    // Create variables to hold the original position and rotation of the sword
    private Vector3 originalPos;
    private Quaternion originalRot;

    public GameObject Projectile;
    // Player Controller
    private PlayerController playerController;

    // Use this for initialization
    void Start ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Grab sword's hitbox (empty GameObject)
        swordBox = transform.Find("Sword Box");
        originalPos = swordBox.transform.localPosition;
        originalRot = swordBox.transform.rotation;

        pinwheelBox = transform.Find("Pinwheel Box");

        // Initialize the sword box's collider and disable it
        swordCollider = swordBox.GetComponent<BoxCollider2D>();
        swordCollider.enabled = false;

        // Initialize the pinwheel collider and disable it.
        pinwheelCollider = pinwheelBox.GetComponent<BoxCollider2D>();
        pinwheelCollider.enabled = false;

        // Iinitialize PlayerController
        playerController = GetComponent<PlayerController>();

        // Reset timer and variables
        timer = 0.0f;
        attacking = false;
        pinwheeling = false;
        holding = false;
        fired = false;

        // Initialize the pinwheel collider and disable it.
        pinwheelCollider = pinwheelBox.GetComponent<BoxCollider2D>();
        pinwheelCollider.enabled = false;

        playerController = GetComponent<PlayerController>();
        //Projectile = GameObject.Find("Projectile");
    }
	
	// Update is called once per frame
	void Update ()
    {
        // If the player presses Enter, start attacking
        // TODO: Add this button to the InputManager
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Set the collider to be enabled if it isn't already
            if (playerController.grounded && !attacking && !holding)
            {
                swordCollider.enabled = true;
                attacking = true;
                SoundManager.S.MakeEmptySlash();
            }
            if (attacking && playerController.hp == 3 && !fired)
            {
                GameObject clone = Instantiate(Projectile, transform.position, Quaternion.identity);
                fired = true;
            }
        }
        // Player is holding down Enter
        else if (Input.GetKey(KeyCode.Return))
        {
            if (!playerController.grounded)
            {
                swordCollider.enabled = false;
                attacking = false;

                pinwheelCollider.enabled = true;
                pinwheeling = true;
                ResetSword();
            }
            else
            {
                pinwheelCollider.enabled = false;
                pinwheeling = false;
            }
        }

        if (playerController.grounded) {
            pinwheelCollider.enabled = false;
            pinwheeling = false;
        }
        // Player is holding down Enter
        else if (Input.GetKey(KeyCode.Return))
        {
            if (!playerController.grounded)
            {
                swordCollider.enabled = false;
                attacking = false;

                pinwheelCollider.enabled = true;
                pinwheeling = true;

                SoundManager.S.MakeEmptySlash();
            }
            else
            {
                pinwheelCollider.enabled = false;
                pinwheeling = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {
            pinwheelCollider.enabled = false;
            pinwheeling = false;
        }


        if (attacking)
        {
            // Get the sword angle and check
            swordAngle = swordBox.rotation.eulerAngles.z;

            // Set target angle and attack direction depending on player position
            if (spriteRenderer.flipX)
            {
                targetAngle = 285;
                attackDir = -Vector3.forward;
                // This works fairly well and the game doesn't work without it for some reason
                if (swordAngle <= targetAngle)
                {
                    swordAngle = 360.0f - swordAngle;
                }
            }
            else
            {
                targetAngle = 75;
                attackDir = Vector3.forward;
            }

            // Rotate the sword and increase the base speed (giving "weight")
            swordBox.RotateAround(transform.position, attackDir,
                                   swordSpeed * Time.deltaTime);
            swordSpeed += 0.05f * swordSpeed;

            // Say the player is now holding the sword straight
            if ((spriteRenderer.flipX && (int)swordAngle <= targetAngle) ||
                (!spriteRenderer.flipX && (int)swordAngle >= targetAngle))
            {
                attacking = false;
                holding = true;
            }
        }

        // Hold the sword as long as directed
        if (holding)
        {
            // If the player has held the sword for long enough, reset the sword attributes
            if (timer >= holdTime)
            {
                ResetSword();
            }
            else
            {
                // Increase the timer
                timer += Time.deltaTime;
            }
        }

        if (pinwheeling && playerController.grounded) {
            playerController.animator.Rebind();
        }
    }

    // Resets the sword's properties
    private void ResetSword()
    {
        swordCollider.enabled = false;
        swordBox.transform.localPosition = originalPos;
        swordBox.transform.rotation = originalRot;
        swordSpeed = 200.0f;
        holding = false;
        timer = 0.0f;
        fired = false;
    }

    // Logic for Ignoring collision with tilemap
}
