using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class GoKartMovement : NetworkBehaviour
{
    private Rigidbody rb;

    public NetworkVariable<float> CurrentSpeed = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> RealSpeed = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> BoostTime = new NetworkVariable<float>();

    public float MaxSpeed;
    public float boostSpeed;

    [Header("Tires")]
    public Transform frontLeftTire;
    public Transform frontRightTire;
    public Transform backLeftTire;
    public Transform backRightTire;
    public bool GLIDER_FLY = false;

    private float steerDirection;
    private float driftTime;

    private bool driftLeft = false;
    private bool driftRight = false;
    private float outwardsDriftForce = 50000;
    private bool touchingGround;

    [Header("Particles Drift Sparks")]
    public Transform leftDrift;
    public Transform rightDrift;
    public Color drift1;
    public Color drift2;
    public Color drift3;

    public Transform boostFire;
    public TMP_Text countdownText;
    public bool isSliding = false;
    public static bool canMove = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return; // Only the local player controls their kart
    }

    void FixedUpdate()
    {
        if (!IsOwner || !canMove) return;

        Move();
        TireSteer();
        Steer();
        GroundNormalRotation();
        Drift();
        Boosts();

        rb.AddForce(new Vector3(0, -45f, 0), ForceMode.Acceleration);
    }

    private void Move()
    {
        RealSpeed.Value = transform.InverseTransformDirection(rb.velocity).z;

        if (Input.GetKey(KeyCode.W))
            CurrentSpeed.Value = Mathf.Lerp(CurrentSpeed.Value, MaxSpeed, Time.deltaTime * 0.5f);
        else if (Input.GetKey(KeyCode.S))
            CurrentSpeed.Value = Mathf.Lerp(CurrentSpeed.Value, -MaxSpeed / 1.75f, 1f * Time.deltaTime);
        else
            CurrentSpeed.Value = Mathf.Lerp(CurrentSpeed.Value, 0, Time.deltaTime * 1.5f);

        Vector3 vel = transform.forward * CurrentSpeed.Value;
        vel.y = rb.velocity.y; // Maintain gravity effect
        rb.velocity = vel;

        UpdatePositionServerRpc(transform.position, transform.rotation);
    }

    [ServerRpc]
    private void UpdatePositionServerRpc(Vector3 newPosition, Quaternion newRotation)
    {
        UpdatePositionClientRpc(newPosition, newRotation);
    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 newPosition, Quaternion newRotation)
    {
        if (!IsOwner)
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * 10);
        }
    }

    private void Steer()
    {
        steerDirection = Input.GetAxisRaw("Horizontal");

        float steerAmount = RealSpeed.Value > 30 ? RealSpeed.Value / 4 * steerDirection : RealSpeed.Value / 1.5f * steerDirection;
        Vector3 steerDirVect = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + steerAmount, transform.eulerAngles.z);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, steerDirVect, 3 * Time.deltaTime);
    }

    private void GroundNormalRotation()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 0.75f))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 7.5f * Time.deltaTime);
            touchingGround = true;
        }
        else
        {
            touchingGround = false;
        }
    }

    private void Drift()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && touchingGround)
        {
            transform.GetChild(0).GetComponent<Animator>().SetTrigger("Hop");
            driftRight = Input.GetAxis("Horizontal") > 0;
            driftLeft = Input.GetAxis("Horizontal") < 0;
        }

        if (Input.GetKey(KeyCode.LeftShift) && touchingGround && CurrentSpeed.Value > 40 && Input.GetAxis("Horizontal") != 0)
        {
            driftTime += Time.deltaTime;
            // Drift visuals and logic here
        }

        if (!Input.GetKey(KeyCode.LeftShift) || RealSpeed.Value < 40)
        {
            driftLeft = driftRight = false;
            isSliding = false;
            BoostTime.Value = driftTime switch
            {
                > 1.5f and < 4f => 0.75f,
                >= 4f and < 7f => 1.5f,
                >= 7f => 2.5f,
                _ => BoostTime.Value
            };
            driftTime = 0;
        }
    }

    private void Boosts()
    {
        BoostTime.Value -= Time.deltaTime;
        MaxSpeed = BoostTime.Value > 0 ? boostSpeed : boostSpeed - 20;
        CurrentSpeed.Value = Mathf.Lerp(CurrentSpeed.Value, MaxSpeed, Time.deltaTime);

        if (BoostTime.Value > 0)
            foreach (Transform child in boostFire) child.GetComponent<ParticleSystem>().Play();
        else
            foreach (Transform child in boostFire) child.GetComponent<ParticleSystem>().Stop();
    }

    private void TireSteer()
    {
        float angle = Input.GetKey(KeyCode.LeftArrow) ? 155 :
                      Input.GetKey(KeyCode.RightArrow) ? 205 : 180;

        frontLeftTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, angle, 0), 5 * Time.deltaTime);
        frontRightTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, angle, 0), 5 * Time.deltaTime);

        if (CurrentSpeed.Value > 30)
        {
            frontLeftTire.GetChild(0).Rotate(-90 * Time.deltaTime * CurrentSpeed.Value * 0.5f, 0, 0);
            frontRightTire.GetChild(0).Rotate(-90 * Time.deltaTime * CurrentSpeed.Value * 0.5f, 0, 0);
            backLeftTire.Rotate(90 * Time.deltaTime * CurrentSpeed.Value * 0.5f, 0, 0);
            backRightTire.Rotate(90 * Time.deltaTime * CurrentSpeed.Value * 0.5f, 0, 0);
        }
    }
}
