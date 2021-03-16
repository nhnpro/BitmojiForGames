//CharacterControl.cs v0.2
//universal character control script for both Console&Mobile
//character control setups: movement, jump, 3 actions, use of prop 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterControl : MonoBehaviour
{
    //component var
    private CharacterController c_controller;
    private Animator c_anim;
    //movement var
    float walkSpeed = 1;
    float runSpeed = 3;
    float rotSpeed = 80;
    float rot = 0;
    private Vector3 moveDir = Vector3.zero;
    //jump var
    float force = 0;
    float move_vertical = 0;
    float move_horizontal = 1;
    float jumpForce = -30;
    float gravity = 30;
    //KeyboardAction var
    float tab_v = 0;
    float new_additional = 0;
    //timer var
    float timer = 0;
    float jump_time = 0.2f;
    bool timerActive = false;
    //temp var only for throwing obj sample
    private Rigidbody p_throwRb;
    private Vector3 p_origLocPos;
    private Vector3 p_origLocRot;

    [Header("Public References")]
    public Transform p_throw;
    public Transform p_hand;
    public GameObject p_throwObj;

    void Start()
    {
        c_controller = GetComponent<CharacterController>();
        c_anim = GetComponent<Animator>();
        timer = 0;
        timerActive = false;
        move_vertical = 0;
        move_horizontal = 1;
        moveDir = new Vector3(0, 0, 0);
    }
    void Update()
    {
        UpdateTimer();
        KeyboardActions();
        Jumps();
        Movements();
    }
    void UpdateTimer()
    {
      if(timerActive)
      {
        timer += Time.deltaTime;
      }else
      {
        timer = 0;
      }
    }
    void Movements()
    {
      if (Input.GetKey(KeyCode.W)) //only move forward in this setup
      {
          c_anim.SetFloat("movement_states", 0.5f);
          c_anim.SetFloat("jump_speed", 0);
          moveDir = new Vector3(0, move_vertical, move_horizontal);
          if (Input.GetKey(KeyCode.LeftShift))
          {
              c_anim.SetFloat("movement_states", 1);
              c_anim.SetFloat("jump_speed", 1);
              moveDir = moveDir * runSpeed;
          }
          if(Input.GetKeyUp(KeyCode.LeftShift))
          {
              c_anim.SetFloat("movement_states", 0.5f);
              c_anim.SetFloat("jump_speed", 0.5f);
              moveDir = moveDir * walkSpeed;
          }
          if(Input.GetKey(KeyCode.D))//rotate only when moving
          {
            rot += rotSpeed * Time.deltaTime;
          }
          if(Input.GetKey(KeyCode.A))
          {
            rot -= rotSpeed * Time.deltaTime;
          }
      }
      if (Input.GetKeyUp(KeyCode.W))
      {
          moveDir.x = 0;
          moveDir.z = 0;
          c_anim.SetFloat("movement_states", 0);
          c_anim.SetFloat("jump_speed", 0);
      }
      transform.eulerAngles = new Vector3(0, rot, 0);
      moveDir = transform.TransformDirection(moveDir);
      c_controller.Move(moveDir * Time.deltaTime);
    }
    void Jumps()
    {
      if (Input.GetKey(KeyCode.Space)) //space pressed, can re-jump in mid-air in this setup
      {
          timerActive = true;//start timer or keep it open
          if(timer <= jump_time)
          {
              force = jumpForce;
              c_anim.SetBool("in_air", true);//
              c_anim.SetBool("jump_on", true);
          }else//start falling after preset jump-up time, when Space still pressed
          {
              c_anim.SetBool("jump_on", false);
              if(c_controller.isGrounded)
              {
                force = 0;
                c_anim.SetBool("in_air", false);//
              }else
              {
                force = gravity;
                c_anim.SetBool("in_air", true);//
              }
          }
      }
      if (Input.GetKeyUp(KeyCode.Space)) //space released
      {
          timerActive = false;//stop and reset timer
          c_anim.SetBool("jump_on", false);
          if(c_controller.isGrounded)
          {
            move_vertical = 0;
            force = 0;
            c_anim.SetBool("in_air", false);
          }else
          {
            force = gravity;
            c_anim.SetBool("in_air", true);
          }
      }
      if (!Input.GetKey(KeyCode.Space))//space not pressed
      {
        timerActive = false;
        if (c_controller.isGrounded)
        {
          force = 0;
          move_vertical = 0;
          c_anim.SetBool("in_air", false);
        }else
        {
          force = gravity;
        }
      }
      move_vertical -= force * Time.deltaTime;
      moveDir = new Vector3(0, move_vertical, 0);
    }
    void KeyboardActions()
    {
        //Num1 pressed, released
        if (Input.GetKey(KeyCode.Alpha1))
        {
            c_anim.SetBool("a01_on", true);
        }
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            c_anim.SetBool("a01_on", false);
        }
        //Num2 pressed, released
        if (Input.GetKey(KeyCode.Alpha2))
        {
            c_anim.SetBool("a02_on", true);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            c_anim.SetBool("a02_on", false);
        }
        //Num3 pressed, released
        if (Input.GetKey(KeyCode.Alpha3))
        {
            c_anim.SetBool("a03_on", true);
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            c_anim.SetBool("a03_on", false);
        }
        //tab pressed, released
        if (Input.GetKey(KeyCode.Tab))
        {
            tab_v = 1;
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            tab_v = 0;
        }
        new_additional = tab_v;//add more keys & values as needed
        c_anim.SetFloat("additional", new_additional);
    }

    //demo for prop setups
    void ThrowAction_temp()
    {
      p_throwRb = p_throw.GetComponent<Rigidbody>();
      p_throwObj.gameObject.SetActive(true);
      p_throwRb.isKinematic = false;
      p_throwRb.transform.parent = null;
      p_throwRb.AddForce(c_controller.transform.forward * 25 + c_controller.transform.up * 5, ForceMode.Impulse);
    }
    void ThrowReset_temp()
    {
      p_origLocRot = new Vector3(90.0f, 0.0f, 0.0f);
      p_origLocPos = new Vector3(0.0707f, -0.0607f, -0.0605f);
      p_throwRb = p_throw.GetComponent<Rigidbody>();
      p_throwObj.gameObject.SetActive(false);
      p_throwRb.isKinematic = true;
      p_throw.parent = p_hand;
      p_throw.localEulerAngles = p_origLocRot;
      p_throw.localPosition = p_origLocPos;
    }
}
