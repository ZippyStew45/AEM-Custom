using Optimization.Caching;
using UnityEngine;
using System.Threading;

public class CheckHitGround : MonoBehaviour
{
    public bool isGrounded;
    private Collider Other;

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.layer)
        {
            case Layers.GroundN:
            case Layers.EnemyAABBN:
                isGrounded = true;
                break;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        Other = other;
        Thread simulationThreadenter = new Thread(new ThreadStart(OnTriggerStayThread));
        simulationThreadenter.Start();
        /*switch (other.gameObject.layer)
        {
            case Layers.GroundN:
            case Layers.EnemyAABBN:
                this.isGrounded = true;
                break;
        }*/
    }

    void OnTriggerStayThread()
    {
        // Perform your computationally intensive task here
        switch (Other.gameObject.layer)
        {
            case Layers.GroundN:
            case Layers.EnemyAABBN:
                this.isGrounded = true;
                break;
        }
    }

}