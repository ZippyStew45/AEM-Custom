using Optimization.Caching;
using System.Linq;
using UnityEngine;
using Anarchy;
using Anarchy.Commands.Chat;
using Anarchy.UI;
using System;
using System.Diagnostics;
using AoTTG.EMAdditions;

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

    private void Awake()
    {

    }

    //base.gameObject.GetComponent<PunkRockTag>() != null

    private void Update()
    {
        if (!PhotonNetwork.player.Builder) //builder check
            return;
        if (HeldRange <= 15) HeldRange = 15;
        if (HeldRange >= 35) HeldRange = 35;

        if (EMInputManager.IsInputDown(EMInputManager.EMInputs.Builder_Place))
        {
            if (HeldOBJ == null)
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, PickUpRange))
                {
                    //if (hit.transform.gameObject.renderer.material.color != UnPlacedObjColor)
                    if (hit.transform.gameObject.GetComponent<BuilderTag>() == null)
                        return;
                    RespawnOBJ(hit);
                    DeleteOBJ(hit);
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
                if (hit.transform.gameObject.GetComponent<BuilderTag>() == null)
                    return;
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

    void RespawnOBJ(RaycastHit hit)
    {
        Photon.MonoBehaviour hero = null;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Stop)
        {

            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                if (FengGameManagerMKII.Heroes.Count > 0)
                {
                    hero = FengGameManagerMKII.Heroes[0];
                }
            }
            else if (PhotonNetwork.player.IsTitan)
            {
                hero = PhotonNetwork.player.GetTitan();
            }
            else
            {
                hero = PhotonNetwork.player.GetHero();
            }
        }
        PrimitiveType Primitive;
        switch (hit.transform.gameObject.name.ToLower())
        {
            case string s when s.StartsWith("cube"):
                Primitive = PrimitiveType.Cube;
                break;
            case string s when s.StartsWith("sphere"):
                Primitive = PrimitiveType.Sphere;
                break;
            case string s when s.StartsWith("capsule"):
                Primitive = PrimitiveType.Capsule;
                break;
            case string s when s.StartsWith("cylinder"):
                Primitive = PrimitiveType.Cylinder;
                break;
            case string s when s.StartsWith("quad"):
                Primitive = PrimitiveType.Quad;
                break;
            case string s when s.StartsWith("plane"):
                Primitive = PrimitiveType.Plane;
                break;
            default:
                Primitive = PrimitiveType.Cube;
                break;
        }


        GameObject SpawnObj = GameObject.CreatePrimitive(Primitive);
        SpawnObj.name += " [" + FengGameManagerMKII.RandomString(25) + "]";
        SpawnObj.transform.position = hero.gameObject.transform.position + (hero.gameObject.transform.forward * 6f) + (Vector3.up * 3f);
        SpawnObj.transform.rotation = hero.gameObject.transform.rotation;
        SpawnObj.transform.localScale = new Vector3(10, 10, 10);
        SpawnObj.AddComponent<BuilderTag>();
        SpawnObj.renderer.material.color = Color.gray;
        SpawnObj.AddComponent<Rigidbody>();
        SpawnObj.GetComponent<Rigidbody>().useGravity = true;
        SpawnObj.GetComponent<Rigidbody>().mass = 10;
        SpawnObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        SpawnObj.layer = 1;

        PickUpOBJ(SpawnObj);
    }
}