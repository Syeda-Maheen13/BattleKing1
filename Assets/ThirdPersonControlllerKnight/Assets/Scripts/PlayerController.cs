using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Animator
    [SerializeField] private Animator playerAnim;

    // Sword equip-unequip
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject swordOnShoulder;
    public bool isEquipping;
    public bool isEquipped;

    // Blocking Parameters
    public bool isBlocking;

    // Kick Parameters
    public bool isKicking;

    // Attack Parameters
    public bool isAttacking;
    private float timeSinceAttack;
    public int currentAttack = 0;

    // Movement Parameters
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f;
    private CharacterController controller;

    // Jump Parameters
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    private Vector3 velocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        timeSinceAttack += Time.deltaTime;

        Move();
        Jump();
        Attack();
        Equip();
        Block();
        Kick();
        Dodge();
    }


    // ---------- Movement ----------
    private void Move()
    {
        float h = Input.GetAxis("Horizontal");  // A/D keys
        float v = Input.GetAxis("Vertical");    // W/S keys

        Vector3 direction = new Vector3(h, 0f, v).normalized;

        if (direction.magnitude >= 0.1f)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float currentSpeed = isRunning ? runSpeed : walkSpeed;

            // Rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move
            Vector3 move = direction * currentSpeed;
            controller.Move(move * Time.deltaTime);

            // Animator (BlendTree)
            float speedValue = isRunning ? runSpeed : walkSpeed;
            playerAnim.SetFloat("Speed", speedValue);
        }
        else
        {
            playerAnim.SetFloat("Speed", 0f);
        }
    }

    // ---------- Jump ----------
    private void Jump()
    {
        // Check ground
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f; // stick to ground
            }

            // Set animator grounded
            playerAnim.SetBool("Grounded", true);

            // Jump only when pressing F (instead of Space)
            if (Input.GetKeyDown(KeyCode.F))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                playerAnim.SetTrigger("Jump");
            }
        }
        else
        {
            // In air state
            playerAnim.SetBool("Grounded", false);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Called from JumpLand animation event
    public void OnLand()
    {
        // Reset jump states after landing
        playerAnim.ResetTrigger("Jump");
        playerAnim.SetBool("Grounded", true);
    }

    // ---------- Equip / Unequip ----------
    private void Equip()
    {
        if (Input.GetKeyDown(KeyCode.R) && playerAnim.GetBool("Grounded"))
        {
            isEquipping = true;
            playerAnim.SetTrigger("Equip");
        }
    }

    public void ActiveWeapon()
    {
        if (!isEquipped)
        {
            sword.SetActive(true);
            swordOnShoulder.SetActive(false);
            isEquipped = !isEquipped;
        }
        else
        {
            sword.SetActive(false);
            swordOnShoulder.SetActive(true);
            isEquipped = !isEquipped;
        }
    }

    public void Equipped()
    {
        isEquipping = false;
    }

    // ---------- Block ----------
    private void Block()
    {
        if (Input.GetKey(KeyCode.Mouse1) && playerAnim.GetBool("Grounded"))
        {
            playerAnim.SetBool("Block", true);
            isBlocking = true;
        }
        else
        {
            playerAnim.SetBool("Block", false);
            isBlocking = false;
        }
    }

    // ---------- Kick ----------
    public void Kick()
    {
        if (Input.GetKey(KeyCode.K) && playerAnim.GetBool("Grounded"))  // 🔹 K button
        {
            playerAnim.SetBool("Kick", true);
            isKicking = true;
        }
        else
        {
            playerAnim.SetBool("Kick", false);
            isKicking = false;
        }
    }

    // ---------- Attack ----------
    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) && playerAnim.GetBool("Grounded") && timeSinceAttack > 0.8f)
        {
            if (!isEquipped)
                return;

            currentAttack++;
            isAttacking = true;

            if (currentAttack > 3)
                currentAttack = 1;

            if (timeSinceAttack > 1.0f)
                currentAttack = 1;

            playerAnim.SetTrigger("Attack" + currentAttack);

            timeSinceAttack = 0;
        }
    }

    // ---------- Dodge ----------
    private void Dodge()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerAnim.GetBool("Grounded") && !isAttacking && !isEquipping) // 🔹 Space button
        {
            playerAnim.SetTrigger("Dodge");
        }
    }

    // Empty method for Animation Events (prevents errors)
    public void OnFootstep()
    {
        // Leave empty OR play footstep sound later
    }

    public void ResetAttack()
    {
        isAttacking = false;
    }
}
