using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class StateManager : MonoBehaviour
    {

        public ControllerStates states;
        public ControllerStats stats;

        public InputVariables inp; // isso vai conectar a classe:INPUTVAIBALES

        

        [System.Serializable]
        public class InputVariables// movements
        {
            public float horizontal;
            public float vertical;
            public float moveAmount;
            public Vector3 moveDirection;
            public Vector3 aimPosition;
        }

        [System.Serializable]
        public class ControllerStates  // States
        {

            public bool onGround;
            public bool isAiming;
            public bool isCrounching;
            public bool isRunning;
            public bool isInteracting;

        }// controller states


        public Animator anim;
        public GameObject activeModel;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public Collider controllerCollider;


        List<Collider> ragdollColliders = new List<Collider>();
        List<Rigidbody> ragdollRigids = new List<Rigidbody>();
        [HideInInspector]
        public LayerMask ignoreLayers;
        [HideInInspector]
        public LayerMask ignoreForGround;

        public Transform mTransform;
        public CharStates curState;//
        public float delta;
       
        
        public void Init()
        {
            mTransform = this.transform;

            SetUpAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.isKinematic = false;
            rigid.drag = 4;
            rigid.angularDrag = 999;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            controllerCollider = GetComponent<Collider>();

            SetupRagdoll();
            ignoreLayers = ~(1 << 9); // ignorelayers
            ignoreForGround = ~(1 << 9 | 1 << 10);


        }

        void SetUpAnimator(){

            if(activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                activeModel = anim.gameObject;
            }

            if (anim == null)
                anim = activeModel.GetComponent<Animator>();


            anim.applyRootMotion = false;
        }


        void SetupRagdoll()
        {
            Rigidbody[] rigids = activeModel.GetComponentsInChildren<Rigidbody>();
          
            foreach(Rigidbody r in rigids)  // 
            {
                if(r == rigid)
                {
                    continue;
                }


                Collider c = r.gameObject.GetComponent<Collider>();
                c.isTrigger = true;
                ragdollRigids.Add(r);
                ragdollColliders.Add(c);
                r.isKinematic = true;
                r.gameObject.layer = 10;//layer 20

            }
        }

        public void FixedTick(float d)
        {
            delta = d;
            switch (curState)
            {
                case CharStates.normal:
                    states.onGround = OnGround();
                    if (states.isAiming)
                    {

                    }
                    else
                    {
                        MovementNormal();
                        RotationNormal();
                    }
                    break;
                case CharStates.onAir:
                    rigid.drag = 0;
                    states.onGround = OnGround();
                    break;
                case CharStates.cover:
                    break;
                case CharStates.vaulting:
                    break;
                default:
                    break;
                 
                 
            }

        }

        void MovementNormal()
        {

            if (inp.moveAmount > 0.05f)
                rigid.drag = 0;
            else
                rigid.drag = 4;

            float speed = stats.walkSpeed;
            if (states.isRunning)
                speed = stats.runSpeed;
            if (states.isCrounching)
                speed = stats.crounchSpeed;

            Vector3 dir = Vector3.zero;
            dir = mTransform.forward * (speed * inp.moveAmount);
            rigid.velocity = dir;


        }

        void RotationNormal(){

            Vector3 targetDir = inp.moveDirection;
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = mTransform.forward;

            Quaternion lookDir = Quaternion.LookRotation(targetDir);
            Quaternion targetRot = Quaternion.Slerp(mTransform.rotation, lookDir, stats.rotateSpeed * delta);
            mTransform.rotation = targetRot;
        
        }

        void HandleAnimationNormal()
        {
            float anim_v = inp.moveAmount;
            anim.SetFloat("Vertical", anim_v, 0.15f , delta);
        }
        void MovementAiming()
        {

        }
         
        public void Tick(float d){

            delta = d;

            switch (curState)
            {
                case CharStates.normal:
                    states.onGround = OnGround();
                    HandleAnimationNormal();
                    break;
                case CharStates.onAir:
                    states.onGround = OnGround();
                    break;
                case CharStates.cover:
                    break;
                case CharStates.vaulting:
                    break;
                default:
                    break;
            }
        
        }

        bool OnGround()
        {
            Vector3 origin = mTransform.position;
            origin.y += 0.6f;
            Vector3 dir = -Vector3.up;
            float dis = 0.7f;
            RaycastHit hit;
            if(Physics.Raycast(origin,dir,out hit ,dis,ignoreForGround))
            {

                Vector3 tp = hit.point;
                mTransform.position = tp;
                return true;
            }

            return false;

        }// onground
         


    }//class


    public enum CharStates
    {

        normal,onAir,cover,vaulting

    }



}// nmae sapce