using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NDR2ndTTB.UI;

namespace NDR2ndTTB
{
    public class UnitController : MonoBehaviour
    {
        [SerializeField]
        UnitStats stats;
        List<PathInfo> path;

        [Header("States")]
        [SerializeField] EStance currentStance;
        public bool run;
        public bool crouch;
        public bool prone;

        int pathIndex;
        float moveTimer;
        float rotateTimer;
        float speedActual;
     
        Vector3 startPosition;
        Vector3 targetPostion;

        bool initLerp;
        [SerializeField] bool moving;

        //[SerializeField] int actionPoint = 20;
        [SerializeField]
        private float walkSpeed = 2;
        [SerializeField] float crouchSpeed = 1;
        [SerializeField] float proneSpeed = 0.8f;
        [SerializeField] float runSpeed = 5f;
        private float rotateSpeed = 8;

        public Node GetNode { get { return GridBase.instance.GetNodeFromWorldPosition(transform.position); } }

        public EStance GetStance { get => currentStance; }
        public UnitStats Stats { get => stats;}

        [SerializeField]
        int x1, z1;

        Animator anim;
        [SerializeField]
        AudioSource sfxSource;
        [SerializeField]
        AudioClip[] footSteps;
        [SerializeField]
        AudioClip[] rogerClip;

        public void Init()
        {
            Vector3 worldPos = GridBase.instance.GetWorldCoordinatesFromNode(x1, 0, z1);
            transform.position = worldPos;
            GetNode.ChangeNodeStatus(false, GridBase.instance);
            anim = GetComponentInChildren<Animator>();
            anim.applyRootMotion = false;

            stats.CurrentHealth = stats.health;
            stats.Stamina = 100;

            EndTurn();
        }
        float counter = 4;
        private void Update()
        {
            anim.SetBool("crouching", crouch);
            anim.SetBool("prone", prone);


            if (moving)
            {
                MovingLogic();

                float v = 0.5f;
                if (run)
                    v = 1;

                anim.SetFloat("vertical", 1, 0.2f, Time.deltaTime);

            }
            else
            {
                anim.SetFloat("vertical", 0, 0.4f, Time.deltaTime);
            }
        }
        public void ConfirmOrder()
        {
            sfxSource.PlayOneShot(rogerClip[Random.Range(0, rogerClip.Length)]);
        }
        void MovingLogic()
        {
            if(!initLerp)
            {
                if(pathIndex == path.Count)
                {
                    moving = false;
                    return;
                }

                GetNode.ChangeNodeStatus(true, GridBase.instance);
                moveTimer = 0;
                rotateTimer = 0;
                startPosition = transform.position;
                targetPostion = path[pathIndex].targetPositon;
                float distance = Vector3.Distance(startPosition, targetPostion);

                float targetSpeed = walkSpeed;
                if (run)
                    targetSpeed = runSpeed;
                if (crouch)
                    targetSpeed = crouchSpeed;
                if (prone)
                    targetSpeed = proneSpeed;
                
                speedActual = targetSpeed / distance;
                initLerp = true;
            }

            moveTimer += Time.deltaTime * speedActual;
            counter += Time.deltaTime * speedActual;

            if(counter > 1.5f)
            {
                sfxSource.PlayOneShot(footSteps[Random.Range(0, footSteps.Length)]);
                counter = 0;
            }


            if(moveTimer > 1)
            {
                
                moveTimer = 1;
                initLerp = false;
                RemoveAP(path[pathIndex]);
                if (pathIndex < path.Count - 1)
                    pathIndex++;
                else
                    moving = false;
            }

            Vector3 newPos = Vector3.Lerp(startPosition, targetPostion, moveTimer);
            transform.position = newPos;
            rotateTimer += Time.deltaTime * rotateSpeed;

            Vector3 lookDir = targetPostion - startPosition;
            lookDir.y = 0;

            if (lookDir == Vector3.zero)
                lookDir = transform.forward;

            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateTimer);

            

        }

        void RemoveAP(PathInfo p)
        {
            stats.CurrentActionPoints -= p.ap;
            GetNode.ChangeNodeStatus(false, GridBase.instance);

            if(GameManager.instance.IsCurrentUnit(this))
            {
                UIManager.instance.UpdateAponCharacterPanel(stats.CurrentActionPoints);
            }
        }

        public void AddPath(List<PathInfo> p)
        {
            pathIndex = 1;
            path = p;
            moving = true;
        }

        public bool IsMoving()
        {
            return moving;
        }
        public int GetActionPoint()
        {
            return stats.CurrentActionPoints;
        }
        
        public void EndTurn()
        {
            stats.CurrentActionPoints = stats.GetBaseActionPoints();
            if (GameManager.instance.IsCurrentUnit(this))
            {
                UIManager.instance.UpdateAponCharacterPanel(stats.CurrentActionPoints);
            }
        }

        public void ChangeStance(EStance stance)
        {
            currentStance = stance;

            switch (stance)
            {
                case EStance.Normal:
                    run = false;
                    crouch = false;
                    prone = false;
                    break;
                case EStance.Run:
                    run = true;
                    crouch = false;
                    prone = false;
                    break;
                case EStance.Crouch:
                    run = false;
                    crouch = true;
                    prone = false;
                    break;
                case EStance.Prone:
                    run = false;
                    crouch = false;
                    prone = true;
                    break;
                default:
                    break;
            }
        }
    }

    public enum EStance
    {
        Normal, Run, Crouch, Prone,
    }

}