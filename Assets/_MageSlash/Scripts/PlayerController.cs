using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float moveSpeed = 10f;

    Vector3 targetPosition;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;
        targetPosition=transform.position;
    }

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit,1000,LayerMask.GetMask("Ground")))
            {
                targetPosition = hit.point;
            }
        }
        Vector3 direction = targetPosition - transform.position;
        if (direction.magnitude > moveSpeed * Time.deltaTime)
        {
            Quaternion targetRotation=Quaternion.LookRotation(direction);
            transform.rotation=Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed*Time.deltaTime);
            transform.position += (direction.normalized * moveSpeed * Time.deltaTime);
        }
        Camera.main.transform.position = transform.position + new Vector3(0, 10, -5);
    }
}
