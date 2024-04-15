using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAnim : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
       if (Input.GetKeyDown(KeyCode.Q)){
              animator.SetBool("Look", true);
         }
         else if (Input.GetKeyDown(KeyCode.E)){
              animator.SetTrigger("ATK");
         }
         
    }
}
