using UnityEngine;
using System;
using System.ComponentModel;
using Anarchy;
using ExitGames.Client.Photon;
using System.Security.Cryptography;
using Optimization.Caching;
using Anarchy.Network.Discord.SDK;
using Anarchy.Configuration;

public class WaterVolume : MonoBehaviour
{
    private Transform baseT;
    public float maxDrownTime = 30f;
    private float drowntime = 30f;
    public float maxHorseDrownTime = 20f;
    private float horseDrowntime = 20f;
    private bool normalFog;
    private Color normalFogColor;
    private float normalFogDensity;
    private float normalFogEndDistance;
    private FogMode normalFogMode;
    private float normalFogStartDistance;
    private float swimSpeed = 4f;
    private Collider[] hitColliders;
    private bool cameraIsUnderWater = false;
    private bool cameraNewlyUnderWater = true;
    private float timeSinceLastBounce = 0.06f;
    private String debugString;
    private HERO myHero;
    private float timeHeroStay = 0f;

    public void UpdateNormalFog(string option, string value, PhotonMessageInfo info)
    {
        switch (option)
        {
            case "start":
                normalFogStartDistance = float.Parse(value);
                break;
            case "end":
                normalFogEndDistance = float.Parse(value);
                break;
            case "density":
                normalFogDensity = float.Parse(value);
                break;
            case "color":
                normalFogColor = value.HexToColor();
                break;
            case "enabled":
                var result = Convert.ToBoolean(int.Parse(value));
                normalFog = result;
                break;
            case "mode":
                switch (value)
                {
                    case "ExponentialSquared":
                        normalFogMode = FogMode.ExponentialSquared;
                        break;
                    case "Exponential":
                        normalFogMode = FogMode.Exponential;
                        break;
                    case "Linear":
                        normalFogMode = FogMode.Linear;
                        break;
                }
                break;
        }
    }

    void Start()
    {
        normalFog = RenderSettings.fog;
        normalFogColor = RenderSettings.fogColor;
        normalFogDensity = RenderSettings.fogDensity;
        normalFogEndDistance = RenderSettings.fogEndDistance;
        normalFogMode = RenderSettings.fogMode;
        normalFogStartDistance = RenderSettings.fogStartDistance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody)
        {
            GameObject gameObject = other.gameObject;
            gameObject = gameObject.transform.root.gameObject;
            HERO thisHero = other.GetComponent<HERO>();
            if (thisHero != null)
            {
                if (gameObject.GetPhotonView() != null && gameObject.GetPhotonView().IsMine)
                {
                    myHero = thisHero;
                    drowntime = maxDrownTime;

                    baseT = IN_GAME_MAIN_CAMERA.MainCamera.transform;


                    #region Bounce on water surface
                    if (gameObject.rigidbody.velocity.magnitude > 40f && timeSinceLastBounce > 0.333f) //bounce only if player speed over 40 and at least 0.333 seconds have passed since last bounce
                    {
                        //calculate horizontal speed
                        float horiSpeed = Mathf.Pow((gameObject.rigidbody.velocity.x * gameObject.rigidbody.velocity.x) + (gameObject.rigidbody.velocity.z * gameObject.rigidbody.velocity.z), 0.5f);
                        //float debugVertSpeedOnImpact = myHero.baseR.velocity.y;
                        float vertSpeed = Mathf.Abs(gameObject.rigidbody.velocity.y);
                        
                        //    check for 'shallow' angle of impact               check if falling
                        if (horiSpeed > 1.1547f * vertSpeed && gameObject.rigidbody.velocity.y < 0f)
                        {
                            //alter player speed
                            gameObject.rigidbody.velocity = new Vector3 (gameObject.rigidbody.velocity.x * 0.6f, (vertSpeed * 0.08f) + horiSpeed * 0.05f + 2f, gameObject.rigidbody.velocity.z * 0.6f);
                            timeSinceLastBounce = 0f;

                            //float debugVertSpeedAfterAdjust = myHero.baseR.velocity.y;

                            /*debugString = "Time of Water Impact: " + Time.timeSinceLevelLoad + "  |  " +
                                "horiSpeed on impact: " + horiSpeed.ToString() + "  |  " +
                                "velocity.y on impact: " + debugVertSpeedOnImpact.ToString() + "  |  " +
                                "Bounce Speed: " + (vertSpeed * 10.4f - 10f).ToString() + "  |  " +
                                "vel.y after bounce: " + debugVertSpeedAfterAdjust.ToString()
                                ;*/

                            /*Debug.Log(
                                "Time of Water Impact: " + Time.timeSinceLevelLoad + "  |  " + 
                                "horiSpeed on impact: " + horiSpeed.ToString() + "  |  " + 
                                "velocity.y on impact: " + debugVertSpeedOnImpact.ToString() + "  |  " + 
                                "Bounce Speed: " + (vertSpeed * 10.4f - 10f).ToString() + "  |  " + 
                                "velocity.y after bounce: " + debugVertSpeedAfterAdjust.ToString()
                                );*/
                        }
                    }
                    #endregion
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (timeSinceLastBounce < 0.35f)
        {
            timeSinceLastBounce += Time.deltaTime;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody)
        {
            HERO thisHero = other.GetComponent<HERO>();
            if (thisHero != null)
            {
                timeHeroStay = 0f;
            }
        }
    }

    void Update()
    {
        cameraIsUnderWater = false;
        hitColliders = Physics.OverlapSphere(IN_GAME_MAIN_CAMERA.BaseT.position, 0.01f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<WaterVolume>() != null)
            {
                cameraIsUnderWater = true;
            }

        }
        if (cameraIsUnderWater && cameraNewlyUnderWater)
        {
            setFogToWater();

            cameraNewlyUnderWater = false;
        }
        else if (cameraIsUnderWater == false && cameraNewlyUnderWater == false)
        {
            setFogToNormal();

            cameraNewlyUnderWater = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody)
        {
            HERO thisHero = other.GetComponent<HERO>();
            if (thisHero != null)
            {
                if (timeHeroStay < 0.1f)
                {
                    timeHeroStay += Time.deltaTime;
                }
                if (timeHeroStay > 0.06f)
                {
                    other.attachedRigidbody.AddForce(-1f * other.attachedRigidbody.velocity.normalized * (other.attachedRigidbody.velocity.sqrMagnitude / (thisHero.speed * 0.8f)));

                    bool headIsUnderWater = false;
                    hitColliders = Physics.OverlapSphere(thisHero.Head.position + Vector3.up * 0.1f, 0.05f);
                    foreach (var hitCollider in hitColliders)
                    {
                        if (hitCollider.GetComponent<WaterVolume>() != null)
                        {
                            headIsUnderWater = true;
                        }
                    }
                    bool shoulderIsUnderWater = false;
                    hitColliders = Physics.OverlapSphere(thisHero.Head.position + Vector3.down * 0.3f, 0.05f);
                    foreach (var hitCollider in hitColliders)
                    {
                        if (hitCollider.GetComponent<WaterVolume>() != null)
                        {
                            shoulderIsUnderWater = true;
                        }
                    }


                    if (shoulderIsUnderWater)
                    {
                        other.attachedRigidbody.AddForce(0f, 15f, 0f); //cancel and overcome gravity, player should float to surface and keep head above
                    }

                    if (headIsUnderWater)
                    {
                        GameObject gameObject = other.gameObject;
                        gameObject = gameObject.transform.root.gameObject;
                        if (gameObject.GetPhotonView() != null && gameObject.GetPhotonView().IsMine)
                        {
                            drowntime -= Time.deltaTime;
                            if (drowntime <= 0f)
                            {
                                thisHero.MarkDie();
                                thisHero.BasePV.RPC("netDie2", PhotonTargets.All, new object[] { -1, "Drowning" });
                            }
                        }
                    }
                    else
                    {
                        drowntime = maxDrownTime;
                    }


                    #region Swimming Controller
                    float num;
                    if (InputManager.IsInput[InputCode.Up])
                    {
                        num = 1f;
                    }
                    else if (InputManager.IsInput[InputCode.Down])
                    {
                        num = -1f;
                    }
                    else
                    {
                        num = 0f;
                    }
                    float num2;
                    if (InputManager.IsInput[InputCode.Left])
                    {
                        num2 = -1f;
                    }
                    else if (InputManager.IsInput[InputCode.Right])
                    {
                        num2 = 1f;
                    }
                    else
                    {
                        num2 = 0f;
                    }

                    float tempSwimSpeed = swimSpeed;
                    if (num != 0f && num2 != 0f)
                    {
                        tempSwimSpeed *= 0.707107f;
                    }
                    float upDown = Vector3.Dot(baseT.Forward(), Vector3.up);
                    if (num > 0f)
                    {
                        other.attachedRigidbody.AddForce(baseT.Forward() * tempSwimSpeed + 6f * new Vector3(0f, upDown, 0f));
                    }
                    if (num < 0f)
                    {
                        other.attachedRigidbody.AddForce(-(baseT.Forward() * tempSwimSpeed + 6f * new Vector3(0f, upDown, 0f)));
                    }
                    if (num2 > 0f)
                    {
                        other.attachedRigidbody.AddForce(baseT.right * tempSwimSpeed);
                    }
                    if (num2 < 0f)
                    {
                        other.attachedRigidbody.AddForce(-baseT.right * tempSwimSpeed);
                    }
                    #endregion


                    return;
                }
            }
            else if (other.attachedRigidbody.GetComponent<Horse>() != null)
            {
                Horse thisHorse = other.GetComponent<Horse>();
                HERO thisHero1 = thisHorse.Owner;
                GameObject gameObject = thisHero1.gameObject;
                gameObject = gameObject.transform.root.gameObject;
                if (gameObject.GetPhotonView() != null && gameObject.GetPhotonView().IsMine)
                {
                    bool headIsUnderWater = false;
                    hitColliders = Physics.OverlapSphere(thisHorse.transform.position + thisHorse.transform.forward * 1.5f + Vector3.up * 2.3f, 0.05f);
                    foreach (var hitCollider in hitColliders)
                    {
                        if (hitCollider.GetComponent<WaterVolume>() != null)
                        {
                            headIsUnderWater = true;
                        }
                    }
                    if (headIsUnderWater)
                    {
                        horseDrowntime -= Time.deltaTime;
                        if (horseDrowntime <= 0f)
                        {
                            thisHorse.gameObject.GetComponent<TITAN_CONTROLLER>().enabled = false;
                            thisHero1.Dismount();
                            UnityEngine.Object.Destroy(thisHorse.gameObject, 0.01f);
                        }
                    }
                    else
                    {
                        horseDrowntime = maxHorseDrownTime;
                    }
                }
            }
            else if (other.attachedRigidbody.GetComponent<TITAN>() != null)
            {

            }
            else
            {
                other.attachedRigidbody.AddForce(-1f * other.attachedRigidbody.velocity.normalized * (other.attachedRigidbody.velocity.sqrMagnitude / 10f));
            }
        }
    }

    private void setFogToWater()
    {
        RenderSettings.fog = true;
        float fogColorR = CacheGameObject.Find("mainLight").GetComponent<Light>().color.r;
        float fogColorG = CacheGameObject.Find("mainLight").GetComponent<Light>().color.g;
        float fogColorB = CacheGameObject.Find("mainLight").GetComponent<Light>().color.b;
        float tempHighestColor = Mathf.Max(fogColorR, fogColorG);
        tempHighestColor = Mathf.Max(tempHighestColor, fogColorB);
        if (tempHighestColor < 1f && tempHighestColor > 0f)
        {
            tempHighestColor = 1f / tempHighestColor;
            fogColorR *= tempHighestColor;
            fogColorG *= tempHighestColor;
            fogColorB *= tempHighestColor;
        }
        else if (tempHighestColor == 0f)
        {
            fogColorR = 1f;
            fogColorG = 1f;
            fogColorB = 1f;
        }
        fogColorR = (fogColorR + RenderSettings.ambientLight.r) * 0.5f * 0.3f;
        fogColorG = (fogColorG + RenderSettings.ambientLight.g) * 0.5f * 0.4f;
        fogColorB = (fogColorB + RenderSettings.ambientLight.b) * 0.5f * 0.6f;
        RenderSettings.fogColor = new Color(fogColorR, fogColorG, fogColorB);

        RenderSettings.fogDensity = 0.015f;
        RenderSettings.fogMode = FogMode.Exponential;
    }

    private void setFogToNormal()
    {
        RenderSettings.fog = normalFog;
        RenderSettings.fogColor = normalFogColor;
        RenderSettings.fogDensity = normalFogDensity;
        RenderSettings.fogEndDistance = normalFogEndDistance;
        RenderSettings.fogMode = normalFogMode;
        RenderSettings.fogStartDistance = normalFogStartDistance;
    }
}