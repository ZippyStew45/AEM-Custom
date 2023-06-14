﻿using Anarchy;
using Anarchy.Inputs;
using System.Net.Sockets;
using UnityEngine;

public class TITAN_CONTROLLER : MonoBehaviour
{
    public Camera currentCamera;
    private PhotonView view;

    public float currentDirection;
    public float targetDirection;
    public bool isAttackDown;
    public bool isAttackIIDown;
    public bool isHorse;
    public bool isJumpDown;
    public bool isSuicide;
    public bool isWALKDown;
    public bool sit;
    public bool grabbackl;
    public bool grabbackr;
    public bool grabfrontl;
    public bool grabfrontr;
    public bool grabnapel;
    public bool grabnaper;
    public bool bite;
    public bool bitel;
    public bool biter;
    public bool chopl;
    public bool chopr;
    public bool choptl;
    public bool choptr;
    public bool cover;

    public bool faceSlap;
    public bool neckSlap;
    public bool kick;
    public bool stomp;

    public static bool HorseAutoRun = false;
    public static bool Mounted = false;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    private void Start()
    {
        currentCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            base.enabled = false;
        }
    }

    private void Update()
    {
        if (view != null && !view.IsMine)
        {
            return;
        }
        Mounted = isHorse;
        if (isHorse)
        {
            if (InputManager.IsInputHorse((int)InputHorse.HorseAutoRun))
            {
                HorseAutoRun = !HorseAutoRun;
            }

            int Ordonate;
            if (InputManager.IsInputHorseHolding((int)InputHorse.HorseForward) || HorseAutoRun)
            {
                Ordonate = 1;
            }
            else
            {
                if (InputManager.IsInputHorseHolding((int)InputHorse.HorseBackward))
                {
                    Ordonate = -1;
                }
                else
                {
                    Ordonate = 0;
                }
            }
            int Abscissa;
            if (InputManager.IsInputHorseHolding((int)InputHorse.HorseLeft))
            {
                Abscissa = -1;
            }
            else
            {
                if (InputManager.IsInputHorseHolding((int)InputHorse.HorseRight))
                {
                    Abscissa = 1;
                }
                else
                {
                    Abscissa = 0;
                }
            }
            if (Abscissa != 0 || Ordonate != 0)
            {
                float CameraAngle = currentCamera.transform.rotation.eulerAngles.y;
                float Tan = Mathf.Atan2((float)Ordonate, (float)Abscissa) * 57.29578f;
                Tan = -Tan + 90f;
                float direction = CameraAngle + Tan;
                targetDirection = direction;
            }
            else
            {
                targetDirection = -874f;
            }
            isAttackDown = false;
            isAttackIIDown = false;
            if (targetDirection != -874f)
            {
                currentDirection = targetDirection;
            }
            float num5 = currentCamera.transform.rotation.eulerAngles.y - currentDirection;
            if (num5 >= 180f)
            {
                num5 -= 360f;
            }
            if (InputManager.IsInputHorseHolding((int)InputHorse.HorseJump))
            {
                isAttackDown = true;
            }
            isWALKDown = InputManager.IsInputHorseHolding((int)InputHorse.HorseWalk);
        }
        else
        {
            int Ordonate;
            if (InputManager.IsInputTitanHolding((int)InputTitan.TitanForward))
            {
                Ordonate = 1;
            }
            else if (InputManager.IsInputTitanHolding((int)InputTitan.TitanBackward))
            {
                Ordonate = -1;
            }
            else
            {
                Ordonate = 0;
            }
            int Abscissa;
            if (InputManager.IsInputTitanHolding((int)InputTitan.TitanLeft))
            {
                Abscissa = -1;
            }
            else if (InputManager.IsInputTitanHolding((int)InputTitan.TitanRight))
            {
                Abscissa = 1;
            }
            else
            {
                Abscissa = 0;
            }
            if (Abscissa != 0 || Ordonate != 0)
            {
                float CameraAngle = IN_GAME_MAIN_CAMERA.MainCamera.transform.rotation.eulerAngles.y;
                float Tan = Mathf.Atan2((float)Ordonate, (float)Abscissa) * 57.29578f;
                Tan = -Tan + 90f;
                float Direction = CameraAngle + Tan;
                targetDirection = Direction;
            }
            else
            {
                targetDirection = -874f;
            }
            isAttackDown = false;
            isJumpDown = false;
            isAttackIIDown = false;
            isSuicide = false;
            grabbackl = false;
            grabbackr = false;
            grabfrontl = false;
            grabfrontr = false;
            grabnapel = false;
            grabnaper = false;
            choptl = false;
            chopr = false;
            chopl = false;
            choptr = false;
            bite = false;
            bitel = false;
            biter = false;
            cover = false;
            sit = false;
            neckSlap = false;
            faceSlap = false;
            kick = false;
            stomp = false;
            if (targetDirection != -874f)
            {
                currentDirection = targetDirection;
            }
            float num5 = currentCamera.transform.rotation.eulerAngles.y - currentDirection;
            if (num5 >= 180f)
            {
                num5 -= 360f;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanPunch))
            {
                isAttackDown = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanSlam))
            {
                isAttackIIDown = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanJump))
            {
                isJumpDown = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanKill))
            {
                isSuicide = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanCoverNape))
            {
                cover = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanSit))
            {
                sit = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanGrabFront) && num5 >= 0f)
            {
                grabfrontr = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanGrabFront) && num5 < 0f)
            {
                grabfrontl = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanGrabBack) && num5 >= 0f)
            {
                grabbackr = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanGrabBack) && num5 < 0f)
            {
                grabbackl = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanGrabNape) && num5 >= 0f)
            {
                grabnaper = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanGrabNape) && num5 < 0f)
            {
                grabnapel = true;
            }

            //Slap
            if(InputManager.DisableDirectionalSlap.Value == false)
            {
                if (InputManager.InvertTitanSlapInput.Value)
                {
                    if (InputManager.IsInputTitan((int)InputTitan.TitanSlap) && num5 >= 0f)
                    {
                        choptr = true;
                    }
                    if (InputManager.IsInputTitan((int)InputTitan.TitanSlap) && num5 < 0f)
                    {
                        choptl = true;
                    }
                    if (InputManager.IsInputTitan((int)InputTitan.TitanSlapLow) && num5 >= 0f)
                    {
                        chopr = true;
                    }
                    if (InputManager.IsInputTitan((int)InputTitan.TitanSlapLow) && num5 < 0f)
                    {
                        chopl = true;
                    }
                }
                else
                {
                    if (InputManager.IsInputTitan((int)InputTitan.TitanSlap) && num5 < 0f)
                    {
                        choptr = true;
                    }
                    if (InputManager.IsInputTitan((int)InputTitan.TitanSlap) && num5 >= 0f)
                    {
                        choptl = true;
                    }

                    if (InputManager.IsInputTitan((int)InputTitan.TitanSlapLow) && num5 < 0f)
                    {
                        chopr = true;
                    }
                    if (InputManager.IsInputTitan((int)InputTitan.TitanSlapLow) && num5 >= 0f)
                    {
                        chopl = true;
                    }
                }
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanLeftSlap))
            {
                choptl = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanRightSlap))
            {
                choptr = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanLeftSlapLow))
            {
                chopl = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanRightSlapLow))
            {
                chopr = true;
            }

            //Bite
            if (InputManager.DisableDirectionalBite.Value == false)
            {
                if (InputManager.InvertTitanBiteInput.Value)
                {
                    if (InputManager.IsInputTitan((int)InputTitan.TitanBite) && num5 > 7.5f)
                    {
                        biter = true;
                    }
                    if (InputManager.IsInputTitan((int)InputTitan.TitanBite) && num5 < -7.5f)
                    {
                        bitel = true;
                    }
                }
                else
                {
                    if (InputManager.IsInputTitan((int)InputTitan.TitanBite) && num5 < 7.5f)
                    {
                        biter = true;
                    }
                    if (InputManager.IsInputTitan((int)InputTitan.TitanBite) && num5 > -7.5f)
                    {
                        bitel = true;
                    }
                }
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanLeftBite))
            {
                bitel = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanRightBite))
            {
                biter = true;
            }

            if (InputManager.IsInputTitan((int)InputTitan.TitanBite) && ((num5 >= -7.5f && num5 <= 7.5f) || InputManager.DisableDirectionalBite.Value))
            {
                bite = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanSlapFace))
            {
                faceSlap = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanSlapNeck))
            {
                neckSlap = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanStomp))
            {
                stomp = true;
            }
            if (InputManager.IsInputTitan((int)InputTitan.TitanKick))
            {
                kick = true;
            }
            isWALKDown = InputManager.IsInputTitanHolding((int)InputTitan.TitanWalk);
        }
    }
}