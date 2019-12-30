using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidPlayer : MonoBehaviour
{
    private HumanoidAnim anim;
    [SerializeField] private Humanoid humanoid;
    [SerializeField] private Camera cam;
    [SerializeField] private int rotDampening = 99;





    private void Start()
    {
        humanoid = GetComponent<Humanoid>();
        anim = GetComponent<HumanoidAnim>();
    }



    AttkType RequestedAttack = AttkType.none;
    RollDir rollDir = RollDir.None;
    void Update()
    {
        bool blocking = false;
        // Block with shield when block button clicked.
        if (Input.GetButton("Fire2"))
            blocking = true;

        if (Input.GetKeyDown(KeyCode.R) && humanoid.InCombat)
            humanoid.InCombat = false;
        else if (!humanoid.InCombat && (Input.GetKeyDown(KeyCode.R) || blocking || Input.GetMouseButtonDown(0)))
            humanoid.InCombat = true;


        Vector3 MoveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));





        RequestedAttack = AttkType.none;
        // Attack when attack button clicked. 
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            RequestedAttack = AttkType.pierce;        

        // Attack when attack button clicked. 
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            RequestedAttack = AttkType.crush;
        
        if (Input.GetButton("Fire1"))
            RequestedAttack = AttkType.slash;



       

        // Get if running & update humanoid move state.
        bool running = UpdateMoveState(MoveDir);

        // Rotate humanoid based onif in combat or not.
        RotateHumanoid(MoveDir);


        UpdateLastMoveDir(MoveDir);

        // Combat rolling when in combat.
        float toRollDir = 0;
        if (humanoid.InCombat && Input.GetKey(KeyCode.Space))
            toRollDir = DoCombatRoll();

        sneaking = false;
        if (Input.GetKey(KeyCode.LeftControl))              
            sneaking = true;
             
        // Update animator.
        anim.UpateAnimator(humanoid.Grounded, blocking, humanoid.InCombat, humanoid.CurSpeed, running, MoveDir.x, MoveDir.z, RequestedAttack, doCmbtRoll, (int)toRollDir, sneaking);
        doCmbtRoll = false;
    }
    bool sneaking = false;

    void UpdateLastMoveDir(Vector3 MoveDir)
    {
        if (MoveDir != Vector3.zero)
            lastMoveDir = MoveDir;
    }
    Vector3 lastMoveDir = Vector3.zero;

    private bool UpdateMoveState(Vector3 MoveDir)
    {
        // Update player move states (speed) based on button input.
        if (MoveDir != Vector3.zero && !anim.DoingAttk)
        {
            if (!humanoid.IsAttacking)
            {
                if (sneaking && humanoid.Grounded)
                {
                    humanoid.SetMoveState(Humanoid.moveEnum.Sneak);
                }                    
                else if (Input.GetButton("Run") && humanoid.Grounded || anim.DoingSlide)
                {                    
                    humanoid.SetMoveState(Humanoid.moveEnum.Run);
                    return true; // Return is running.
                }
                else if (humanoid.Grounded)
                {
                    humanoid.SetMoveState(Humanoid.moveEnum.Walk);
                }
                else if (!humanoid.Grounded)
                {
                    humanoid.SetMoveState(Humanoid.moveEnum.Jump);
                }
            }
        }
        else if (MoveDir == Vector3.zero || anim.DoingAttk)
            humanoid.SetMoveState(Humanoid.moveEnum.Idle);
        return false;
    }
   






    private void RotateHumanoid(Vector3 MoveDir)
    {    
        
        // Rotate the player differently based on if in combat or not.
        if (!anim.DoingCombatRoll && humanoid.Grounded && humanoid.InCombat  && MoveDir != Vector3.zero || anim.InAttkState)
        {
           
            if (anim.DoingAttk && anim.AttkRotationPenalty < 1) 
                humanoid.Rotate(cam.transform.forward, 900);
            else if(!anim.InAttkState && humanoid.InCombat && MoveDir != Vector3.zero)
                humanoid.Rotate(cam.transform.forward, rotDampening);
        }
           
        else if (!humanoid.InCombat && MoveDir != Vector3.zero)
            humanoid.RotateRelative(new Vector3(MoveDir.x, 0, MoveDir.z), cam.transform, rotDampening * 4);
    }








    private float DoCombatRoll()
    {
        // Calculate roll direction - add dir enums together to get new dir (e.g. fwd + lft = fwd&lft)
        if (lastMoveDir != Vector3.zero)
        {
            rollDir = RollDir.None;
            if (lastMoveDir.x > 0)
                rollDir += (int)RollDir.Right;
            else if (lastMoveDir.x < 0)
                rollDir += (int)RollDir.Left;
            if (lastMoveDir.z > 0)
                rollDir += (int)RollDir.Fwd;
            else if (lastMoveDir.z < 0)
                rollDir += (int)RollDir.Back;
        }
        // Do new combat roll if valid dir & not already doing one.
        doCmbtRoll = false;
        if (rollDir != RollDir.None && !anim.DoingCombatRoll) doCmbtRoll = true;       
        return (int)rollDir;
    }
    public enum RollDir { None = 0, Fwd = 1, Back = 10, Left = 3, Right = 6, FwdL = 4, FwdR = 7, BackL = 13, BackR = 16 }  
    private bool doCmbtRoll = false;









}