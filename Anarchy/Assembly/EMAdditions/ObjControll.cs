using Optimization.Caching;
using System.Linq;
using UnityEngine;
using Anarchy;
using Anarchy.Commands.Chat;
using Anarchy.UI;

public class ObjControll : MonoBehaviour
{
    [Header("PickUp Settings")]
    [SerializeField] Transform HoldArea = Camera.main.transform;
    private GameObject HeldOBJ;
    private Rigidbody HeldOBJRB;


    [Header("Physics Paramater")]
    [SerializeField] private float PickUpRange = 250f;
    [SerializeField] private float PickUpForce = 150f;

    private void Awake()
    {

    }

    private void Update()
    {
        if (!PhotonNetwork.player.Builder) //builder check
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (HeldOBJ == null)
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, PickUpRange))
                {
                    if (hit.transform.gameObject.renderer.material.color != Color.red)
                        return;
                    PickUpOBJ(hit.transform.gameObject);
                }
            }
            else
            {
                DropOBJ();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, PickUpRange))
            {
                DeleteOBJ(hit);
            }
        }
        if (HeldOBJ != null)
        {
            MoveOBJ();
            Chat.Add(HeldOBJ.name);
        }
    }

    void MoveOBJ()
    {
        if (Vector3.Distance(HeldOBJ.transform.position, (HoldArea.position + HoldArea.transform.forward * 18f)) > 0.1f)
        {
            Vector3 moveDirection = ((HoldArea.position + HoldArea.transform.forward * 18f) - HeldOBJ.transform.position);
            HeldOBJRB.AddForce(moveDirection * PickUpForce);
        }
    }

    void PickUpOBJ(GameObject obj)
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
        Destroy(hit.transform.gameObject);
        //FengGameManagerMKII.FGM.BasePV.RPC("DeletePrimitiveRPC", PhotonTargets.AllBuffered, hit.transform.gameObject);
    }
}