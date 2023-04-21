using Anarchy.Commands.Chat;
using AoTTG.EMAdditions;
using Optimization.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AoTTG.Anarchy.Commands.Chat
{
    internal class SpawnPrimitiveCommand : ChatCommand
    {
        public SpawnPrimitiveCommand() : base("obj", false, true, true)
        {

        }
        
        public override bool Execute(string[] args)
        {
            if (!PhotonNetwork.player.Builder)
                return false;


            chatMessage = "Game Object Spawned: " + args[0];

            /*Vector3 ppos = PhotonPlayer.MyHero().transform.position;
            Quaternion prot = PhotonPlayer.MyHero().transform.rotation;

            FengGameManagerMKII.FGM.BasePV.RPC("SpawnPrimitiveRPC", PhotonTargets.AllBuffered, args[0], ppos, prot);*/


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
            switch (args[0].ToLower())
            {
                case "cube":
                    Primitive = PrimitiveType.Cube;
                    break;
                case "sphere":
                    Primitive = PrimitiveType.Sphere;
                    break;
                case "capsule":
                    Primitive = PrimitiveType.Capsule;
                    break;
                case "cylinder":
                    Primitive = PrimitiveType.Cylinder;
                    break;
                case "quad":
                    Primitive = PrimitiveType.Quad;
                    break;
                case "plane":
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
            ObjControll.PickUpOBJ(SpawnObj);
            return true;
        }
    }
}
