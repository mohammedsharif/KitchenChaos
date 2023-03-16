using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance {get; private set;}

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private GameInput gameInput;
    [SerializeField] float movementSpeed = 7f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking;
    private Vector3 lastMoveDirection;
    private BaseCounter SelectedCounter;
    private KitchenObject kitchenObject;

    private void Awake() 
    {
        if(Instance != null)
            Debug.LogError("There is an another player instance");

        Instance = this;    
    }

    private void Start() 
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if(!KitchenGameManager.Instance.IsGamePlaying()) return;

        if(SelectedCounter != null)
            SelectedCounter.Interact(this);
    }

    private void GameInput_OnInteractAlternateAction(object sender, System.EventArgs e)
    {
        if(!KitchenGameManager.Instance.IsGamePlaying()) return;

        if(SelectedCounter != null)
            SelectedCounter.InteractAlternate(this);  
    }

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
            if(raycastHit.transform.TryGetComponent<BaseCounter>(out BaseCounter clearCounter))
            {
                if(SelectedCounter != clearCounter)
                    SetSelectedCounter(clearCounter);
            }
            else
                SetSelectedCounter(null);
        }
        else
            SetSelectedCounter(null);
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
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius ,moveDirX,moveDistance);

            if(canMove)
                moveDir = moveDirX;//can move only on X
            else
            {
                //cannot move only on x
                //Attempt only z direction
                Vector3 moveDirZ = new Vector3(0,0,moveDir.z).normalized;
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius ,moveDirZ,moveDistance);

                if(canMove)
                    moveDir = moveDirZ;//can move only on Z
            }
        }

        if(canMove)
            transform.position += moveDir * moveDistance;

        isWalking = moveDir != Vector3.zero;

        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotationSpeed * Time.deltaTime);
    }

    private void SetSelectedCounter(BaseCounter SelectedCounter)
    {
        this.SelectedCounter = SelectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs{
            selectedCounter = SelectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if(kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        this.kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
