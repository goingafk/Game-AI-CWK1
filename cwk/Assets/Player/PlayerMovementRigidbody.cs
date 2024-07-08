using UnityEngine;

public class PlayerMovementRigidbody : MonoBehaviour
{
    public float moveSpeed = 5f;
    public LayerMask unwalkableLayer;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ).normalized * moveSpeed * Time.deltaTime;

        // Check if the movement direction hits any unwalkable objects
        if (!IsMoveBlocked(move))
        {
            rb.MovePosition(transform.position + move);
        }
    }

    private bool IsMoveBlocked(Vector3 move)
    {
        // Perform a capsule cast to check if the intended move collides with an unwalkable object
        float capsuleHeight = GetComponent<Collider>().bounds.size.y / 2;
        Vector3 capsuleBottom = transform.position - new Vector3(0, capsuleHeight, 0);
        Vector3 capsuleTop = transform.position + new Vector3(0, capsuleHeight, 0);

        return Physics.CapsuleCast(capsuleBottom, capsuleTop, rb.GetComponent<Collider>().bounds.extents.x, move, out RaycastHit hit, move.magnitude, unwalkableLayer);
    }
}
