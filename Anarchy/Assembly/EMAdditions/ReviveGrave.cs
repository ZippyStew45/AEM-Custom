using Anarchy;
using Optimization;
using Optimization.Caching;
using UnityEngine;

public class ReviveGrave : Interactable
{
    public PhotonPlayer Player;
    private void Start()
    {
        gameObject.AddComponent<SelfDestroy>().CountDown = 60f;
        transform.FindChild("Text").GetComponent<TextMesh>().text = Player.UIName.ToHTMLFormat();
        FengGameManagerMKII.FGM.FlareColour(transform.position, Quaternion.Euler(-10, 0, 0), Color.gray.r, Color.gray.g, Color.gray.b, null);
    }

    public override void Interact()
    {
        FengGameManagerMKII.FGM.BasePV.RPC("spawnPlayerAtRPC", Player, transform.position.x, transform.position.y, transform.position.z);
        Destroy(gameObject);
    }
    private void LateUpdate()
    {
        if (!Player.Dead && gameObject.GetComponent<SelfDestroy>().CountDown <= 59.5f)
        {
            Destroy(gameObject);
        }
    }
}