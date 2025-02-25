using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoKartWheel : MonoBehaviour
{
    public Transform frontLeftTire;
    public Transform frontRightTire;
    public Transform backLeftTire;
    public Transform backRightTire;
    // Start is called before the first frame update
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

        if (GoKartMovement.CurrentSpeed > 30)
        {
            frontLeftTire.GetChild(0).Rotate(-90 * Time.deltaTime * GoKartMovement.CurrentSpeed * 0.5f, 0, 0);
            frontRightTire.GetChild(0).Rotate(-90 * Time.deltaTime * GoKartMovement.CurrentSpeed * 0.5f, 0, 0);
            backLeftTire.Rotate(90 * Time.deltaTime * GoKartMovement.CurrentSpeed * 0.5f, 0, 0);
            backRightTire.Rotate(90 * Time.deltaTime * GoKartMovement.CurrentSpeed * 0.5f, 0, 0);
        }
        else
        {
            frontLeftTire.GetChild(0).Rotate(-90 * Time.deltaTime * GoKartMovement.RealSpeed * 0.5f, 0, 0);
            frontRightTire.GetChild(0).Rotate(-90 * Time.deltaTime * GoKartMovement.RealSpeed * 0.5f, 0, 0);
            backLeftTire.Rotate(90 * Time.deltaTime * GoKartMovement.RealSpeed * 0.5f, 0, 0);
            backRightTire.Rotate(90 * Time.deltaTime * GoKartMovement.RealSpeed * 0.5f, 0, 0);
        }



    }
}
