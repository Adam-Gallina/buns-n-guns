using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTesting : MonoBehaviour
{
    public Transform linkedImage;
    public Transform linkedArm;
    public Transform firePoint;

    public Transform targetMarker;

    public LineRenderer[] debugLines;

    private float gunAngle;
    private float armToGun;

    void Awake()
    {
        UpdateAngles();
    }

    private void UpdateAngles()
    {
        gunAngle = Vector2.Angle(linkedArm.position - firePoint.position, Vector2.right);
        armToGun = (linkedArm.position - firePoint.position).magnitude;
    }

    void Update()
    {
        //UpdateAngles();

        if (Input.GetKey(KeyCode.Mouse0))
        {
            targetMarker.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Aim(targetMarker.position);   
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            LogTriangle(firePoint.position, linkedArm.position, targetMarker.position);
            Debug.Log(Angle(linkedArm.position + linkedArm.right, linkedArm.position, targetMarker.position));
        }
        DebugRay(debugLines[3], firePoint.position, firePoint.right, 100);
    }

    public void Aim(Vector2 target)
    {
        Vector2 rightTarget = ReflectOverBody(target);
        Vector2 rightArm = ReflectOverBody(linkedArm.position);

        // Law of Sines
        float a = (rightTarget - rightArm).magnitude;
        float c = armToGun;
        float A = gunAngle;
        float C = Mathf.Asin(c / a * Mathf.Sin(A * Mathf.Deg2Rad)) * Mathf.Rad2Deg;

        float angle = -(C + Vector2.SignedAngle(rightTarget - rightArm, Vector2.right));

        if (target.x <= linkedImage.position.x)
        {
            linkedImage.localScale = new Vector3(-1, 1, 1);
            linkedArm.eulerAngles = new Vector3(0, 0, -angle);
            firePoint.localEulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            linkedImage.localScale = new Vector3(1, 1, 1);
            linkedArm.eulerAngles = new Vector3(0, 0, angle);
            firePoint.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    private Vector3 ReflectOverBody(Vector2 v)
    {
        Vector2 rightTarget = v - (Vector2)linkedImage.position;
        rightTarget.x = Mathf.Abs(rightTarget.x);
        rightTarget += (Vector2)linkedImage.position;
        return rightTarget;
    }

    public void LogTriangle(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        float a = Angle(pointB, pointA, pointC);
        float b = Angle(pointA, pointB, pointC);
        float c = Angle(pointB, pointC, pointA);

        DebugLine(debugLines[0], pointA, pointB);
        DebugLine(debugLines[1], pointC, pointB);
        DebugLine(debugLines[2], pointA, pointC);

        Debug.Log(a + " " + b + " " + c);
    }

    public float Angle(Vector2 a, Vector2 b, Vector2 c)
    {
        return Vector2.Angle(b - a, b - c);
    }

    public void DebugLine(LineRenderer line, Vector2 pointA, Vector2 pointB)
    {
        Vector3[] points = new Vector3[]
        {
            pointA,
            pointB
        };

        line.SetPositions(points);
    }

    public void DebugRay(LineRenderer line, Vector2 startPoint, Vector2 direction, float length)
    {
        Vector3[] points = new Vector3[]
        {
            startPoint,
            startPoint + direction * length
        };

        line.SetPositions(points);
    }
}
