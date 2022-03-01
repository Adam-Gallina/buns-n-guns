using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [Header("Movement")]
    [SerializeField] private float minDistance = 0.1f;
    [SerializeField] private float maxDistance = 1.5f;
    [SerializeField] private float driftSpeed = 0.5f;
    private bool autoMovement = true;
    private bool trackMouse = false;
    [SerializeField] private float maxDistToMouse = 3;

    private Transform target = null;

    private Camera[] cams;
    private float normalSize = 8;
    private float currSize = 8;
    private bool animatingZoom = false;

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.DrawWireSphere(target.position, minDistance);
            Gizmos.DrawWireSphere(target.position, maxDistance);
        }
    }

    private void Awake()
    {
        instance = this;

        cams = transform.GetComponentsInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        if (LevelController.instance.paused)
            return;

        if (target != null && autoMovement)
        {
            Vector3 targetPos = new Vector3();

            if (trackMouse)
            {
                Vector3 mousePosRelativeToTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition) - target.position;
                targetPos = Vector3.MoveTowards(target.position, target.position + mousePosRelativeToTarget / 2, maxDistToMouse);
            }
            else
            {
                targetPos = target.position;
            }
            
            MoveTowards(targetPos);
        }
    }

    public void MoveTowards(Vector3 targetPos)
    {
        float dist = Vector3.Distance(transform.position, targetPos);

        Vector3 moveDir = new Vector3();

        if (dist > maxDistance)
        {
            float moveDist = dist - maxDistance;
            moveDir = (targetPos - transform.position).normalized * moveDist;
        }
        else if (dist > minDistance)
        {
            moveDir = (targetPos - transform.position) * driftSpeed;
        }


        transform.Translate(moveDir);
    }

    public void SetFollowTarget(Transform t)
    {
        target = t;
    }

    public void TrackMouse(bool trackState)
    {
        trackMouse = trackState;
    }

    public void ClearFollowTarget()
    {
        target = null;
    }

    public void SetZoom(float zoomPercent)
    {
        float size = normalSize / zoomPercent;
        SetSize(size);
    }

    private void SetSize(float size)
    {
        foreach (Camera c in cams)
            c.orthographicSize = size;

        currSize = size;
    }

    public void AnimateZoom(float zoomPercent, float time)
    {
        if (!animatingZoom)
            StartCoroutine(ZoomAnimation(zoomPercent, time));
    }

    private IEnumerator ZoomAnimation(float zoomPercent, float time)
    {
        animatingZoom = true;

        float targetSize = normalSize / zoomPercent;
        float sizeDelta = targetSize - currSize;
        float speed = sizeDelta / time;

        while (Mathf.Abs(targetSize - currSize) > 0.1f)
        {
            SetSize(currSize + speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        animatingZoom = false;
    }
}
