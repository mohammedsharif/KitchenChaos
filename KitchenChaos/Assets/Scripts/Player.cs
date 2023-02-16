using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] float movementSpeed = 7f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] LayerMask countersLayerMask;

    private bool isWalking;
    private Vector3 lastMoveDirection;
    private void Update() 
    {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking()
    {
        return isWalking;
    }
    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        if(moveDir != Vector3.zero)
            lastMoveDirection = moveDir;

        float interactDistance = 2f;
        if(Physics.Raycast(transform.position, lastMoveDirection, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if(raycastHit.transform.TryGetComponent<ClearCounter>(out ClearCounter clearCounter))
                clearCounter.Interact();
        }
    }
    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        float moveDistance = movementSpeed * Time.deltaTime;
        float playerRadius = 0.68f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius ,moveDir,moveDistance);

        if(!canMove)
        {
            //cannot move to movedir
            //Attempt only x direction
            Vector3 moveDirX = new Vector3(moveDir.x,0,0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius ,moveDirX,moveDistance);

            if(canMove)
                moveDir = moveDirX;//can move only on X
            else
            {
                //cannot move only on x
                //Attempt only z direction
                Vector3 moveDirZ = new Vector3(0,0,moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius ,moveDirZ,moveDistance);

                if(canMove)
                    moveDir = moveDirZ;//can move only on Z
            }
        }

        if(canMove)
            transform.position += moveDir * moveDistance;

        isWalking = moveDir != Vector3.zero;

        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotationSpeed * Time.deltaTime);
    }

}
