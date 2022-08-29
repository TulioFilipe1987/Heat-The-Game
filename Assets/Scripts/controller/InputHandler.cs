using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{

    public class InputHandler : MonoBehaviour
    {

        float horizontal;
        float vertical;

        bool aimInput;
        bool spriteInput;
        bool shootInput;
        bool crounchInput;
        bool reloadInput;
        bool switchInput;
        bool pivotInput;

        bool isInit;

        float delta;

        public StateManager states;

        public Transform camHolder;

        void Start()
        {
            InitInGame();
        }

        public void InitInGame()
        {
            states.Init();
            isInit = true;
        }

        void FixedUpdate(){

            if (!isInit)
                return;

            delta = Time.fixedDeltaTime;
            GetInput_FixedUpdate();
            InGame_UpdateStates_FixedUpdate();

            states.FixedTick(delta);
            
        }

        void GetInput_FixedUpdate()
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal"); 
        }

        void InGame_UpdateStates_FixedUpdate()
        {
            states.inp.horizontal = horizontal;
            states.inp.vertical = vertical;

            states.inp.moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            Vector3 moveDir = camHolder.forward * vertical;
            moveDir += camHolder.right * horizontal;
            moveDir.Normalize();
            states.inp.moveDirection= moveDir;

        }

        void Update()
        {
            if (!isInit)
                return;

            delta = Time.deltaTime;

            states.Tick(delta);
        }


    }// class

    public enum GamePhase
    {

    }

}//namescpace
