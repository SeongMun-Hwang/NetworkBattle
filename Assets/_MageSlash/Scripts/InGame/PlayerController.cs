using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    enum State
    {
        Idle,
        Move,
        Attack,
        Dying
    }
    State state;
    Animator animator;

    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float moveSpeed = 10f;

    public static Action<PlayerController> OnPlayerSpawn;
    public static Action<PlayerController> OnPlayerDespawn;
    public StatDisplayer statDisplayer;

    Vector3 targetPosition;
    Vector3 attackTargetPosition;

    bool isWaitingAttackInput;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            OnPlayerSpawn?.Invoke(this);
        }
        if (!IsOwner) return;
        targetPosition = transform.position;
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawn?.Invoke(this);
        }
    }

    void Start()
    {
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
        state= State.Idle;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            isWaitingAttackInput = true;
        }
        if (Input.GetMouseButtonDown(0) && isWaitingAttackInput && state!=State.Attack)
        {
            isWaitingAttackInput = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("Ground")))
            {
                if (GetComponent<MagicPoint>().UseMagic(20))
                {
                    attackTargetPosition = hit.point;
                    SetState(State.Attack);
                    targetPosition = transform.position;

                    GetComponent<ProjectileLauncher>().Attack(attackTargetPosition);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            isWaitingAttackInput = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("Ground")))
            {
                targetPosition = hit.point;
            }
        }

    }
    private void FixedUpdate()
    {
        if (!IsOwner) return;

        if (state != State.Attack)
        {
            Vector3 direction = targetPosition - transform.position;
            if (direction.magnitude > moveSpeed * Time.fixedDeltaTime)
            {
                SetState(State.Move);
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                Vector3 movementVector = (direction.normalized * moveSpeed * Time.fixedDeltaTime);
                GetComponent<Rigidbody>().MovePosition(movementVector + transform.position);
            }
            else
            {
                SetState(State.Idle);
                GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
        else
        {
            Vector3 direction = attackTargetPosition - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        Camera.main.transform.position = transform.position + new Vector3(0, 10, -5);
    }
    public void AttackEnded()
    {
        SetState(State.Idle);
    }
    void SetState(State newState)
    {
        switch (state)
        {
            case State.Idle:
                if (newState == State.Attack)
                {
                    animator.SetTrigger("Attack");
                }
                else if (newState == State.Move)
                {
                    animator.SetTrigger("Move");
                }
                break;
            case State.Move:
                if(newState == State.Attack)
                {
                    animator.SetTrigger("Attack");
                }
                else if(newState == State.Idle)
                {
                    animator.SetTrigger("Idle");
                }
                break;
            case State.Attack:
                animator.SetTrigger("Idle");
                break;
        }
        state= newState;
    }
}
