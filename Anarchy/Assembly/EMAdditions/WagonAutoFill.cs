using UnityEngine;

public class WagonAutoFill : MonoBehaviour
{
    private Transform baseT;
    private float elapsedTime;
    private float stepTime = 1f;

    private void Awake()
    {
        baseT = transform;
    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        this.elapsedTime += Time.fixedDeltaTime;
        if (this.elapsedTime > this.stepTime)
        {
            this.elapsedTime -= this.stepTime;
            foreach (HERO hero in FengGameManagerMKII.Heroes)
            {
                if (hero != null && hero.IsLocal && Vector3.Distance(hero.baseT.position, baseT.position) < 8f)
                {
                    if (Input.GetKeyDown(KeyCode.Keypad7))
                    {
                        hero.baseT.parent = baseT.transform;
                    }
                    hero.AddGas(4f);
                }
            }
        }
    }
}