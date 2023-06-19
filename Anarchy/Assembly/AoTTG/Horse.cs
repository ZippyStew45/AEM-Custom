//Temporarily horse rework is stopped.

using Anarchy.Configuration;
using AoTTG.EMAdditions.Sounds;
using Optimization.Caching;
using System.Linq;
using UnityEngine;

public class Horse : Photon.MonoBehaviour
{
    private readonly Vector3 heroOffsetVector = Vectors.up * 1.68f;
    private float awayTimer;
    private Animation baseA;
    private Rigidbody baseR;
    private Transform baseT;
    private TITAN_CONTROLLER controller;
    private ParticleSystem dustParticle;
    private Vector3 setPoint;
    public float speed = 45f;
    private float timeElapsed;
    public GameObject dust;
    public HERO Owner;
    public GameObject myHero;
    public HorseState State = HorseState.Idle;
    public bool Wagon;
    public GameObject wag;
    public bool FollowOverride;
    private float _idleTime = 0f;
    #region Added by Sysyfus for WaterVolume
    private Collider[] hitColliders;
    public float maxDrownTime = 15f;
    private float drownTime = 15f;
    private float xRotation = 0f;
    #endregion
    #region Added by Sysyfus for horse tilt for sloped ground
    private float frontElevation;
    private float rearElevation;
    #endregion

    public void SetSpeed(float s)
    {
        this.speed = s;
    }
    private void Awake()
    {
        baseA = GetComponent<Animation>();
        baseR = GetComponent<Rigidbody>();
        baseT = GetComponent<Transform>();
        dustParticle = dust.GetComponent<ParticleSystem>();
        myHero = BasePV.owner.GameObject;
    }

    private void CrossFade(string aniName, float time)
    {   
        baseA.CrossFade(aniName, time);
        if (!PhotonNetwork.connected)
        {
            return;
        }
        if (BasePV.IsMine)
        {
            BasePV.RPC("netCrossFade", PhotonTargets.Others, new object[]
            {
                aniName,
                time
            });
        }
    }

    private void Followed()
    {
        if (Owner == null)
        {
            return;
        }
        State = HorseState.Follow;
        float[] randoms = new float[2].Select(x => Random.Range(-6f, 6f)).ToArray();
        setPoint = Owner.baseT.position + Vectors.right * randoms[0] + Vectors.forward * randoms[1];
        setPoint.y = GetHeight(setPoint + Vectors.up * 5f);
        awayTimer = 0f;
    }

    private float GetHeight(Vector3 pt)
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(pt, -Vectors.up, out raycastHit, 1000f, Layers.Ground.value))
        {
            return raycastHit.point.y;
        }
        return 0f;
    }

    private void FixedUpdate()
    {
        if (Owner == null && BasePV.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
            return;
        }

        #region Added by Sysyfus for WaterVolume
        xRotation = 0f;

        #region Added by Sysyfus for horse tilt for sloped ground
        rearElevation = 0f;
        frontElevation = 0f;
        //Physics.Raycast(baseT.position - baseT.forward + Vector3.up, Vector3.down, out RaycastHit hitRear, 2f, Layers.EnemyGround.value);
        //Physics.Raycast(baseT.position + baseT.forward + Vector3.up, Vector3.down, out RaycastHit hitFront, 2f, Layers.EnemyGround.value);
        float tempF = 1f;
        if (Wagon)
        {
            tempF = 16f;
        }
        if (Physics.Raycast(baseT.position - (baseT.forward * tempF) + Vector3.up, Vector3.down, out RaycastHit hitRear, tempF * 2f, Layers.EnemyGround.value))
        {
            this.rearElevation = hitRear.distance;
        }
        if (Physics.Raycast(baseT.position + baseT.forward + Vector3.up, Vector3.down, out RaycastHit hitFront, 2f, Layers.EnemyGround.value))
        {
            this.frontElevation = hitFront.distance;
        }
        if (rearElevation != 0f && frontElevation != 0f)
        {
            //xRotation = Mathf.Rad2Deg * (Mathf.Atan(frontElevation - rearElevation));
            xRotation = Mathf.Rad2Deg * (Mathf.Atan((hitRear.point.y - hitFront.point.y) / tempF));
        }
        else if (rearElevation != 0f && frontElevation == 0f)
        {
            xRotation = Mathf.Rad2Deg * (Mathf.Atan((hitRear.point.y - baseT.position.y) / tempF));
        }
        else if (rearElevation == 0f && frontElevation != 0f)
        {
            xRotation = Mathf.Rad2Deg * (Mathf.Atan((baseT.position.y - hitFront.point.y) / tempF));
        }
        xRotation = Mathf.Clamp(xRotation, -45f, 45f);
        #endregion

        float floatDepth = 2.1f;
        floatDepth -= 0.5f * baseR.velocity.magnitude / this.speed;
        if (State == HorseState.Mounted)
        {
            floatDepth += 0.1f;
        }
        bool isInWater = false;
        hitColliders = Physics.OverlapSphere(baseT.position + (Vector3.up * floatDepth), 0.05f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<WaterVolume>() != null)
            {
                isInWater = true;
            }
        }

        if (isInWater)
        {
            if (!this.IsGrounded())
            {
                xRotation = -20f;
            }
            Vector3 waterResistance = new Vector3(-this.baseR.velocity.x, -this.baseR.velocity.y, -this.baseR.velocity.z);
            waterResistance.x *= 5.0f;
            waterResistance.z *= 5.0f;
            waterResistance.y *= 1.5f;
            waterResistance.y += 55f;
            if (this.Wagon)
            {
                waterResistance.y -= 10f;
                waterResistance.x *= 2.0f;
                waterResistance.z *= 2.0f;
            }
            baseR.AddForce(waterResistance * this.baseR.mass);
        }

        base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(xRotation, baseT.rotation.eulerAngles.y, 0f), Time.deltaTime * 2.5f);
        #endregion

        switch (State)
        {
            case HorseState.Mounted:
                {
                    if (this.Owner == null)
                    {
                        this.Unmounted();
                        return;
                    }

                    Owner.baseT.position = baseT.position + heroOffsetVector.y * baseT.up; //changed by Sysyfus to accomodate horse tilt
                    Owner.baseT.rotation = baseR.rotation;
                    Owner.baseR.velocity = baseR.velocity;

                    if (controller.targetDirection == -874f)
                    {
                        this.ToIdleAnimation();
                        if (baseR.velocity.magnitude > 15f)
                        {
                            if (!Owner.baseA.IsPlaying("horse_run"))
                            {
                                Owner.CrossFade("horse_run", 0.1f);
                            }
                        }
                        else if (!Owner.baseA.IsPlaying("horse_idle"))
                        {
                            Owner.CrossFade("horse_idle", 0.1f);
                        }
                    }
                    else
                    {
                        base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, this.controller.targetDirection, 0f), 100f * Time.deltaTime / (base.rigidbody.velocity.magnitude + 20f));
                        if (this.controller.isWALKDown)
                        {
                            #region Changed by Sysyfus for WaterVolume
                            if (this.IsGrounded())
                            {
                                baseR.AddForce(baseT.Forward() * this.speed * 0.6f, ForceMode.Acceleration);
                            }
                            else
                            {
                                baseR.AddForce(new Vector3(baseT.Forward().x, 0f, baseT.Forward().z) * this.speed * 0.6f, ForceMode.Acceleration);
                            }
                            #endregion
                            if (baseR.velocity.magnitude >= this.speed * 0.6f)
                            {
                                baseR.AddForce(-this.speed * 0.6f * baseR.velocity.normalized, ForceMode.Acceleration);
                            }
                        }
                        else
                        {
                            #region Changed by Sysyfus for WaterVolume
                            if (this.IsGrounded())
                            {
                                baseR.AddForce(baseT.Forward() * this.speed, ForceMode.Acceleration);
                            }
                            else
                            {
                                baseR.AddForce(new Vector3(baseT.Forward().x, 0f, baseT.Forward().z) * this.speed, ForceMode.Acceleration);
                            }
                            #endregion
                            if (baseR.velocity.magnitude >= this.speed)
                            {
                                baseR.AddForce(-this.speed * baseR.velocity.normalized, ForceMode.Acceleration);
                            }
                        }
                        if (baseR.velocity.magnitude > 8f)
                        {
                            if (!baseA.IsPlaying("horse_Run"))
                            {
                                this.CrossFade("horse_Run", 0.1f);
                            }
                            if (!this.Owner.baseA.IsPlaying("horse_run"))
                            {
                                this.Owner.CrossFade("horse_run", 0.1f);
                            }
                            if (!this.dustParticle.enableEmission)
                            {
                                this.dustParticle.enableEmission = true;
                                /*BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                                {
                                    true
                                });*/
                            }

                            if (BasePV.IsMine && Settings.HorseGallop && !AudioManager.AudioSource_horse_gallop.isPlaying && base.animation.IsPlaying("horse_Run") && AudioManager.List_string_of_loaded_sounds.Contains("horse_gallop"))
                                AudioManager.AudioSource_horse_gallop.Play();
                        }
                        else if (baseR.velocity.magnitude > 0.1f) //line changed by Sysyfus so horse will use idle animations
                        {
                            if (!baseA.IsPlaying("horse_WALK"))
                            {
                                this.CrossFade("horse_WALK", 0.1f);
                            }
                            if (!this.Owner.baseA.IsPlaying("horse_idle"))
                            {
                                this.Owner.baseA.CrossFade("horse_idle", 0.1f);
                            }
                            if (this.dustParticle.enableEmission)
                            {
                                this.dustParticle.enableEmission = false;
                                /*BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                                {
                                    false
                                });*/
                            }
                        }
                        else //this block added by Sysyfus so horse will use idle animations
                        {
                            ToIdleAnimation();
                        }
                    }

                    if ((this.controller.isAttackDown || this.controller.isAttackIIDown) && this.IsGrounded())
                    {
                        #region Changed by Sysyfus for WaterVolume
                        if (!isInWater)
                        {
                            baseR.AddForce(Vectors.up * 25f, ForceMode.VelocityChange);
                        }
                        else
                        {
                            baseR.AddForce(Vectors.up * 8.33f, ForceMode.VelocityChange);
                        }
                        #endregion
                    }
                }
                break;

            case HorseState.Follow:
                {
                    if (this.Owner == null)
                    {
                        this.Unmounted();
                        return;
                    }

                    if (baseR.velocity.magnitude > 8f)
                    {
                        if (!baseA.IsPlaying("horse_Run"))
                        {
                            this.CrossFade("horse_Run", 0.1f);
                        }
                        if (!this.dustParticle.enableEmission)
                        {
                            this.dustParticle.enableEmission = true;
                            /*BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                            {
                        true
                            });*/
                        }
                    }
                    else
                    {
                        if (!baseA.IsPlaying("horse_WALK"))
                        {
                            this.CrossFade("horse_WALK", 0.1f);
                        }
                        if (this.dustParticle.enableEmission)
                        {
                            this.dustParticle.enableEmission = false;
                            /*BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                            {
                                false
                            });*/
                        }
                    }

                    float num = -Mathf.DeltaAngle(FengMath.GetHorizontalAngle(baseT.position, this.setPoint), base.gameObject.transform.rotation.eulerAngles.y - 90f);
                    base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(xRotation, base.gameObject.transform.rotation.eulerAngles.y + num, 0f), 200f * Time.deltaTime / (baseR.velocity.magnitude + 20f)); //Changed by Sysyfus for WaterVolume
                    if (Vector3.Distance(this.setPoint, baseT.position) < 20f)
                    {
                        baseR.AddForce(baseT.Forward() * this.speed * 0.7f, ForceMode.Acceleration);
                        if (baseR.velocity.magnitude >= this.speed)
                        {
                            baseR.AddForce(-this.speed * 0.7f * baseR.velocity.normalized, ForceMode.Acceleration);
                        }
                    }
                    else
                    {
                        baseR.AddForce(base.transform.Forward() * this.speed, ForceMode.Acceleration);
                        if (baseR.velocity.magnitude >= this.speed)
                        {
                            baseR.AddForce(-this.speed * baseR.velocity.normalized, ForceMode.Acceleration);
                        }
                    }
                    this.timeElapsed += Time.deltaTime;
                    if (this.timeElapsed > 0.6f)
                    {
                        this.timeElapsed = 0f;
                        if (Vector3.Distance(this.Owner.baseT.position, this.setPoint) > 20f && (!this.Wagon || this.FollowOverride))
                        {
                            this.Followed();
                        }
                    }
                    if (Vector3.Distance(this.Owner.baseT.position, baseT.position) < 5f)
                    {
                        this.Unmounted();
                    }
                    if (Vector3.Distance(this.setPoint, baseT.position) < 5f)
                    {
                        this.Unmounted();
                    }
                    this.awayTimer += Time.deltaTime;
                    if (this.awayTimer > 6f)
                    {
                        this.awayTimer = 0f;
                        if (Physics.Linecast(baseT.position + Vectors.up, this.Owner.baseT.position + Vectors.up, Layers.Ground.value))
                        {
                            baseT.position = new Vector3(this.Owner.baseT.position.x, this.GetHeight(this.Owner.baseT.position + Vectors.up * 5f), this.Owner.baseT.position.z);
                        }
                    }
                }
                break;

            case HorseState.Idle:
                {
                    this.ToIdleAnimation();
                    if (this.Owner != null && Vector3.Distance(this.Owner.baseT.position, baseT.position) > 20f && (!this.Wagon || this.FollowOverride))
                    {
                        this.Followed();
                    }
                }
                break;
        }
        baseR.AddForce(new Vector3(0f, -50f * baseR.mass, 0f));

        #region Added by Sysyfus cuz kicking up dust while in air is dumb
        if (this.dustParticle.enableEmission && !this.IsGrounded())
        {
            this.dustParticle.enableEmission = false;
        }
        #endregion
    }

    [RPC]
    private void netCrossFade(string aniName, float time)
    {
        baseA.CrossFade(aniName, time);
    }

    [RPC]
    private void netPlayAnimation(string aniName)
    {
        baseA.Play(aniName);
    }

    [RPC]
    private void netPlayAnimationAt(string aniName, float normalizedTime)
    {
        baseA.Play(aniName);
        baseA[aniName].normalizedTime = normalizedTime;
    }

    private void PlayAnimationAt(string aniName, float normalizedTime)
    {
        baseA.Play(aniName);
        baseA[aniName].normalizedTime = normalizedTime;
        if (!PhotonNetwork.connected)
        {
            return;
        }
        if (BasePV.IsMine)
        {
            BasePV.RPC("netPlayAnimationAt", PhotonTargets.Others, new object[]
            {
                aniName,
                normalizedTime
            });
        }
    }

    [RPC]
    private void setDust(bool enable)
    {
        if (this.dustParticle.enableEmission)
        {
            this.dustParticle.enableEmission = enable;
        }
    }

    private void Start()
    {
        this.controller = base.gameObject.GetComponent<TITAN_CONTROLLER>();
    }

    private void ToIdleAnimation()
    {
        if (baseR.velocity.magnitude > 0.1f)
        {
            if (baseR.velocity.magnitude > 15f)
            {
                if (!baseA.IsPlaying("horse_Run"))
                {
                    this.CrossFade("horse_Run", 0.1f);
                }
                if (!this.dustParticle.enableEmission)
                {
                    this.dustParticle.enableEmission = true;
                    /*BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                    {
                        true
                    });*/
                }
            }
            else
            {
                if (!baseA.IsPlaying("horse_WALK"))
                {
                    this.CrossFade("horse_WALK", 0.1f);
                }
                if (this.dustParticle.enableEmission)
                {
                    this.dustParticle.enableEmission = false;
                    /*BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                    {
                        false
                    });*/
                }
            }
        }
        else
        {
            if (_idleTime <= 0f)
            {
                if (base.animation.IsPlaying("horse_idle0"))
                {
                    float num = UnityEngine.Random.Range(0f, 1f);
                    if (num < 0.33f)
                    {
                        CrossFade("horse_idle1", 0.1f);
                    }
                    else if (num < 0.66f)
                    {
                        CrossFade("horse_idle2", 0.1f);
                    }
                    else
                    {
                        CrossFade("horse_idle3", 0.1f);
                    }
                    _idleTime = 1f;
                }
                else
                {
                    CrossFade("horse_idle0", 0.1f);
                    _idleTime = UnityEngine.Random.Range(1f, 4f);
                }
            }
            if (this.dustParticle.enableEmission)
            {
                this.dustParticle.enableEmission = false;
                /*BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                {
                    false
                });*/
            }
            _idleTime -= Time.deltaTime; //changed by Sysyfus so horse will use idle animations
            //base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(base.gameObject.transform.position + Vectors.up * 0.1f, -Vectors.up, 0.3f, Layers.EnemyGround.value);
    }

    public void Mounted()
    {
        this.State = HorseState.Mounted;
        base.gameObject.GetComponent<TITAN_CONTROLLER>().enabled = true;
    }

    public void PlayAnimation(string aniName)
    {
        baseA.Play(aniName);
        if (!PhotonNetwork.connected)
        {
            return;
        }
        if (BasePV.IsMine)
        {
            BasePV.RPC("netPlayAnimation", PhotonTargets.Others, new object[]
            {
                aniName
            });
        }
    }

    public void Unmounted()
    {
        this.State = HorseState.Idle;
        base.gameObject.GetComponent<TITAN_CONTROLLER>().enabled = false;
    }

    public enum HorseState
    {
        Idle,
        Follow,
        Mounted,
    }
}