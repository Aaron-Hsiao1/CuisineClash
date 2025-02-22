using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoKartMovement : MonoBehaviour
{
    private Rigidbody rb;

    public static float CurrentSpeed = 0;
    public float MaxSpeed;
    public float boostSpeed;
    public static float RealSpeed; // not the applied speed

    [Header("Tires")]
    public Transform frontLeftTire;
    public Transform frontRightTire;
    public Transform backLeftTire;
    public Transform backRightTire;
    public bool GLIDER_FLY = false;

    // Drift and steering stuff
    private float steerDirection;
    private float driftTime;

    bool driftLeft = false;
    bool driftRight = false;
    float outwardsDriftForce = 50000;

    public bool isSliding = false;

    private bool touchingGround;

    [Header("Particles Drift Sparks")]
    public Transform leftDrift;
    public Transform rightDrift;
    public Color drift1;
    public Color drift2;
    public Color drift3;

    [HideInInspector]
    public float BoostTime = 0;

    public Transform boostFire;

    public TMP_Text countdownText; // Assign this in the Inspector
    public static bool canMove = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Disable global gravity
        

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!canMove) return;
        move();
        tireSteer();
        steer();
        groundNormalRotation();
        drift();
        boosts();
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(0, -45f, 0), ForceMode.Acceleration);
    }


    private void move()
    {
        RealSpeed = transform.InverseTransformDirection(rb.velocity).z; // Real velocity before setting the value

        if (Input.GetKey(KeyCode.W))
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, Time.deltaTime * 0.5f);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, -MaxSpeed / 1.75f, 1f * Time.deltaTime);
        }
        else
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, 0, Time.deltaTime * 1.5f);
        }

        Vector3 vel = transform.forward * CurrentSpeed;
        vel.y = rb.velocity.y; // Gravity
        rb.velocity = vel;
    }

    private void steer()
    {
        steerDirection = Input.GetAxisRaw("Horizontal"); // -1, 0, 1
        Vector3 steerDirVect;

        float steerAmount;

        if (driftLeft && !driftRight)
        {
            steerDirection = Input.GetAxis("Horizontal") < 0 ? -1.5f : -0.5f;
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, -20f, 0), 8f * Time.deltaTime);

            if (isSliding && touchingGround)
                rb.AddForce(transform.right * outwardsDriftForce * Time.deltaTime, ForceMode.Acceleration);
        }
        else if (driftRight && !driftLeft)
        {
            steerDirection = Input.GetAxis("Horizontal") > 0 ? 1.5f : 0.5f;
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 20f, 0), 8f * Time.deltaTime);

            if (isSliding && touchingGround)
                rb.AddForce(transform.right * -outwardsDriftForce * Time.deltaTime, ForceMode.Acceleration);
        }
        else
        {
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 0f, 0), 8f * Time.deltaTime);
        }

        steerAmount = RealSpeed > 30 ? RealSpeed / 4 * steerDirection : RealSpeed / 1.5f * steerDirection;

        steerDirVect = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + steerAmount, transform.eulerAngles.z);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, steerDirVect, 3 * Time.deltaTime);
    }

    private void groundNormalRotation()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 0.75f))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 7.5f * Time.deltaTime);
            touchingGround = true;
        }
        else
        {
            touchingGround = false;
        }
    }

    private void drift()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && touchingGround)
        {
            transform.GetChild(0).GetComponent<Animator>().SetTrigger("Hop");
            if (steerDirection > 0)
            {
                driftRight = true;
                driftLeft = false;
            }
            else if (steerDirection < 0)
            {
                driftRight = false;
                driftLeft = true;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && touchingGround && CurrentSpeed > 40 && Input.GetAxis("Horizontal") != 0)
        {
            driftTime += Time.deltaTime;

            if (driftTime >= 1.5 && driftTime < 4)
            {
                for (int i = 0; i < leftDrift.childCount; i++)
                {
                    ParticleSystem DriftPS = rightDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN = DriftPS.main;

                    ParticleSystem DriftPS2 = leftDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;

                    PSMAIN.startColor = drift1;
                    PSMAIN2.startColor = drift1;

                    if (!DriftPS.isPlaying && !DriftPS2.isPlaying)
                    {
                        DriftPS.Play();
                        DriftPS2.Play();
                    }
                }
            }
            if (driftTime >= 4 && driftTime < 7)
            {
                for (int i = 0; i < leftDrift.childCount; i++)
                {
                    ParticleSystem DriftPS = rightDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN = DriftPS.main;
                    ParticleSystem DriftPS2 = leftDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;

                    PSMAIN.startColor = drift2;
                    PSMAIN2.startColor = drift2;
                }
            }
            if (driftTime >= 7)
            {
                for (int i = 0; i < leftDrift.childCount; i++)
                {
                    ParticleSystem DriftPS = rightDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN = DriftPS.main;
                    ParticleSystem DriftPS2 = leftDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;

                    PSMAIN.startColor = drift3;
                    PSMAIN2.startColor = drift3;
                }
            }
        }

        if (!Input.GetKey(KeyCode.LeftShift) || RealSpeed < 40)
        {
            driftLeft = false;
            driftRight = false;
            isSliding = false;

            if (driftTime > 1.5 && driftTime < 4)
            {
                BoostTime = 0.75f;
            }
            if (driftTime >= 4 && driftTime < 7)
            {
                BoostTime = 1.5f;
            }
            if (driftTime >= 7)
            {
                BoostTime = 2.5f;
            }

            driftTime = 0;

            for (int i = 0; i < 5; i++)
            {
                ParticleSystem DriftPS = rightDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                ParticleSystem.MainModule PSMAIN = DriftPS.main;

                ParticleSystem DriftPS2 = leftDrift.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;

                DriftPS.Stop();
                DriftPS2.Stop();
            }
        }
    }

    private void boosts()
    {
        BoostTime -= Time.deltaTime;
        if (BoostTime > 0)
        {
            for (int i = 0; i < boostFire.childCount; i++)
            {
                if (!boostFire.GetChild(i).GetComponent<ParticleSystem>().isPlaying)
                {
                    boostFire.GetChild(i).GetComponent<ParticleSystem>().Play();
                }
            }
            MaxSpeed = boostSpeed;

            CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, 1 * Time.deltaTime);
        }
        else
        {
            for (int i = 0; i < boostFire.childCount; i++)
            {
                boostFire.GetChild(i).GetComponent<ParticleSystem>().Stop();
            }
            MaxSpeed = boostSpeed - 20;
        }
    }

    private void tireSteer()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            frontLeftTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 155, 0), 5 * Time.deltaTime);
            frontRightTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 155, 0), 5 * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            frontLeftTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 205, 0), 5 * Time.deltaTime);
            frontRightTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 205, 0), 5 * Time.deltaTime);
        }
        else
        {
            frontLeftTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 180, 0), 5 * Time.deltaTime);
            frontRightTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 180, 0), 5 * Time.deltaTime);
        }

        //tire spinning

        if (CurrentSpeed > 30)
        {
            frontLeftTire.GetChild(0).Rotate(-90 * Time.deltaTime * CurrentSpeed * 0.5f, 0, 0);
            frontRightTire.GetChild(0).Rotate(-90 * Time.deltaTime * CurrentSpeed * 0.5f, 0, 0);
            backLeftTire.Rotate(90 * Time.deltaTime * CurrentSpeed * 0.5f, 0, 0);
            backRightTire.Rotate(90 * Time.deltaTime * CurrentSpeed * 0.5f, 0, 0);
        }
        else
        {
            frontLeftTire.GetChild(0).Rotate(-90 * Time.deltaTime * RealSpeed * 0.5f, 0, 0);
            frontRightTire.GetChild(0).Rotate(-90 * Time.deltaTime * RealSpeed * 0.5f, 0, 0);
            backLeftTire.Rotate(90 * Time.deltaTime * RealSpeed * 0.5f, 0, 0);
            backRightTire.Rotate(90 * Time.deltaTime * RealSpeed * 0.5f, 0, 0);
        }



    }

}

