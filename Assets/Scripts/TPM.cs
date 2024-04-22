using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    private CharacterController characterController;
    public Transform camera;
    public float speed = 4;
    public float runSpeed = 8;
    private float gravity = -9.8f;

    // Declaro el animator
    private Animator animator;
    public AudioSource footstepAudio;
    private bool isCrouching = false;
    private bool isWalkingCrouched = false;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Defino que si el personaje no se esta moviendo su booleano sea falso y asi se inicie la animacion de idle
        animator.SetBool("Walking", false);

        // Input para el movimiento horizontal y vertical
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        Vector3 movement = Vector3.zero;
        float movementSpeed = 0;

        // Calculo la direccion del movimiento
        if (hor != 0 || ver != 0)
        {
            Vector3 forward = camera.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = camera.right;
            right.y = 0;
            right.Normalize();

            Vector3 direction = forward * ver + right * hor;
            movementSpeed = Mathf.Clamp01(direction.magnitude);
            direction.Normalize();

            // Determino la velocidad de movimiento
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                movement = direction * runSpeed * movementSpeed * Time.deltaTime;
                animator.SetBool("Walking", false);
                animator.SetBool("Run", true);
            }
            else
            {
                movement = direction * speed * movementSpeed * Time.deltaTime;
                animator.SetBool("Walking", true);
                animator.SetBool("Run", false);
            }

            // Aplico la rotacion
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.2f);

            // Reproduzco el sonido de los pasos
            if (!footstepAudio.isPlaying)
            {
                footstepAudio.Play();
                footstepAudio.loop = true;
            }
        }
        else
        {
            footstepAudio.Stop();
            footstepAudio.loop = false;
        }

        // Agrego la gravedad
        movement.y += gravity * Time.deltaTime;
        // Aplico el movimiento
        characterController.Move(movement);

        // Crouching input handling
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            animator.SetBool("Crouching", isCrouching);
        }

        // Jumping input handling
        if (characterController.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Jumping");
           
            movement.y = Mathf.Sqrt(-2 * gravity * 1.5f);
             animator.SetBool("Jump", false);
        }

        // Walking while crouched input handling
        if (isCrouching && Input.GetKey(KeyCode.W))
        {
            isWalkingCrouched = true;
            animator.SetBool("WalkingC", true);
        }
        else
        {
            isWalkingCrouched = false;
            animator.SetBool("WalkingC", false);
        }
    }
}
