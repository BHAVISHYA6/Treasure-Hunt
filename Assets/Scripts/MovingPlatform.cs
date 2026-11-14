using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Waypoints")]
    public Vector3 localEndPoint = new Vector3(0f, 0f, 6f);
    public float moveSpeed = 2.5f;
    public float waitTimeAtEnds = 1.0f;

    private Vector3 worldStartPos;
    private Vector3 worldEndPos;
    private Vector3 lastPos;
    private bool movingToEnd = true;
    private float waitTimer = 0f;

    private CharacterController riderController;

    private void Start()
    {
        worldStartPos = transform.position;
        worldEndPos = transform.TransformPoint(localEndPoint);
        lastPos = transform.position;
    }

    private void Update()
    {
        Vector3 targetPos = movingToEnd ? worldEndPos : worldStartPos;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                movingToEnd = !movingToEnd;
                waitTimer = waitTimeAtEnds;
            }
        }

        // Move rider by delta position so player rides smoothly
        Vector3 delta = transform.position - lastPos;
        if (riderController != null && delta.sqrMagnitude > 0f)
        {
            riderController.Move(delta);
        }

        lastPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        var cc = other.GetComponent<CharacterController>();
        if (cc != null)
        {
            riderController = cc;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var cc = other.GetComponent<CharacterController>();
        if (cc != null && riderController == cc)
        {
            riderController = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 end = Application.isPlaying ? worldEndPos : transform.TransformPoint(localEndPoint);
        Gizmos.DrawLine(transform.position, end);
        Gizmos.DrawWireCube(end, transform.lossyScale);
    }
}
