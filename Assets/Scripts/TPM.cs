using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    public CharacterController ControladorDeAgente;
    public float velocidadDeMovimiento = 5f;
    public float Gravedad = 9.8f;
    public float fallVelocity;
    public float FuerzaDeSalto=0f;
    public float VelocidadDeRotacion = 20f;
    public bool Agachado=false;


    private Vector3 movePlayer;
    public Camera cam;
    private Vector3 camForward;
    private Vector3 camRigth;

    Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();   
    }

    // Update is called once per frame
    void Update()
    {
        float MovimientoHorizontalX = Input.GetAxis("Horizontal");
        float MovimientoHorizontalZ = Input.GetAxis("Vertical");
        Vector3 DireccionDeMovimiento = new Vector3(MovimientoHorizontalX, 0, MovimientoHorizontalZ);
        DireccionDeMovimiento.Normalize();

        camDirection();

        movePlayer = DireccionDeMovimiento.x * camRigth + DireccionDeMovimiento.z * camForward;

        movePlayer = movePlayer * velocidadDeMovimiento;


        //MOVIMIENTO
        if (DireccionDeMovimiento.magnitude <= 0.1)
        {
            animator.SetBool("Walking", false);
        }
        else
        {
            animator.SetBool("Walking", true);
        }
    


        ControladorDeAgente.transform.LookAt(ControladorDeAgente.transform.position + movePlayer);

        SetGravity();

        PlayerSkills();

        ControladorDeAgente.Move(movePlayer * Time.deltaTime);
        
    }

    private void camDirection()
    {
        camForward = cam.transform.forward;
        camRigth = cam.transform.right;
        camForward.y = 0;
        camRigth.y = 0;

        camForward = camForward.normalized;
        camRigth = camRigth.normalized;
    }

    private void SetGravity()
    {
        if(ControladorDeAgente.isGrounded )
        {
            fallVelocity = -Gravedad * Time.deltaTime;
            movePlayer.y = fallVelocity;
        }
        else
        {
            fallVelocity -= Gravedad *Time.deltaTime;
            movePlayer.y = fallVelocity;
        }
        animator.SetBool("IsGrounded",ControladorDeAgente.isGrounded);    
    }

    private void PlayerSkills()
    {
        if(ControladorDeAgente.isGrounded && Input.GetButtonDown("Jump") && Agachado==false)
        {
            fallVelocity = FuerzaDeSalto;
            movePlayer.y = fallVelocity;
            animator.SetTrigger("Jump");
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && Agachado == false)
        {
            animator.SetTrigger("Crouch");
            Agachado = true;
        }
        else if(Input.GetKeyDown(KeyCode.LeftControl) && Agachado == true)
        {
            animator.SetTrigger("Crouch");
            Agachado = false;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.SetTrigger("Attack");
        }
      
    }

}