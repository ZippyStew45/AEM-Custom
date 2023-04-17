using Optimization.Caching;
using System.Linq;
using UnityEngine;
using Anarchy;
using Anarchy.Commands.Chat;
using Anarchy.UI;
using System;
using System.Diagnostics;

public class ObjControll : MonoBehaviour
{
    [Header("PickUp Settings")]
    [SerializeField] Transform HoldArea = Camera.main.transform;
    private static GameObject HeldOBJ;
    private static Rigidbody HeldOBJRB;


    [Header("Physics Paramater")]
    [SerializeField] private float PickUpRange = 25f;
    [SerializeField] private float PickUpForce = 150f;
    [SerializeField] private float HeldRange = 18f;

    [Header("Object Colors")]
    public static Color PlacedObjColor = Color.grey;
    public static Color UnPlacedObjColor = Color.red;

    private void Awake()
    {

    }

    private void Update()
    {
        if (!PhotonNetwork.player.Builder) //builder check
            return;
        if (HeldRange <= 15) HeldRange = 12;
        if (HeldRange >= 35) HeldRange = 25;

        if (EMInputManager.IsInputDown(EMInputManager.EMInputs.Builder_Place))
        {
            if (HeldOBJ == null)
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, PickUpRange))
                {
                    if (hit.transform.gameObject.renderer.material.color != UnPlacedObjColor)
                        return;
                    PickUpOBJ(hit.transform.gameObject);
                }
            }
            else
            {
                DropOBJ();
            }
        }
        if (EMInputManager.IsInputDown(EMInputManager.EMInputs.Builder_delete))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, PickUpRange))
            {
                if (hit.transform.gameObject.renderer.material.color == UnPlacedObjColor || hit.transform.gameObject.renderer.material.color == PlacedObjColor)
                  DeleteOBJ(hit);
            }
        }
        if (EMInputManager.IsInputDown(EMInputManager.EMInputs.Builder_OBJ_away))
        {
            HeldRange += 1f;
        }
        if (EMInputManager.IsInputDown(EMInputManager.EMInputs.Builder_OBJ_close))
        {
            HeldRange -= 1f;
        }
        if (HeldOBJ != null)
        {
            MoveOBJ();
            //Chat.Add(HeldOBJ.name);
        }
    }

    void MoveOBJ()
    {
        if (Vector3.Distance(HeldOBJ.transform.position, (HoldArea.position + HoldArea.transform.forward * HeldRange)) > 0.1f)
        {
            Vector3 moveDirection = ((HoldArea.position + HoldArea.transform.forward * HeldRange) - HeldOBJ.transform.position);
            HeldOBJRB.AddForce(moveDirection * PickUpForce);
        }
    }

    public static void PickUpOBJ(GameObject obj)
    {
        if (obj.GetComponent<Rigidbody>())
        {
            HeldOBJRB = obj.GetComponent<Rigidbody>();
            HeldOBJRB.useGravity = false;
            HeldOBJRB.drag = 10;
            HeldOBJRB.constraints = RigidbodyConstraints.FreezeRotation;

            //HeldOBJRB.transform.parent = HoldArea;
            HeldOBJ = obj;
        }
    }

    void DropOBJ()
    {
        FengGameManagerMKII.FGM.BasePV.RPC("SpawnPrimitiveRPC", PhotonTargets.AllBuffered, HeldOBJ.name, HeldOBJ.transform.position, HeldOBJ.transform.rotation);
        Destroy(HeldOBJ.gameObject);
        /*HeldOBJRB.useGravity = true;
        HeldOBJRB.drag = 1;
        HeldOBJRB.constraints = RigidbodyConstraints.FreezeAll;

        HeldOBJ.transform.parent = null;
        HeldOBJ = null;*/

    }

    void DeleteOBJ(RaycastHit hit)
    {
        //Destroy(hit.transform.gameObject);
        FengGameManagerMKII.FGM.BasePV.RPC("DeletePrimitiveRPC", PhotonTargets.AllBuffered, hit.transform.gameObject.name);
    }
}