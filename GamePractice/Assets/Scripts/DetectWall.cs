using UnityEngine;
using TMPro;

public class DetectWall : MonoBehaviour
{
    [SerializeField] public GameObject square;
    [SerializeField] private KeyCode key = KeyCode.N;
    [SerializeField] public TextMeshPro textMesh;
    [SerializeField] private Transform beginPos;
    [SerializeField] private Transform endPos;
    [SerializeField] private BoxCollider2D moveTrigger;
    [SerializeField] private float speed = 3f;

    private Vector3 targetPosition;
    private bool isMoving;

    void Start()
    {
        InitializeComponents();
        square.transform.position = beginPos.position;
        targetPosition = beginPos.position;
        isMoving = false;
        if (textMesh != null) textMesh.text = key.ToString().ToUpper();
    }

    void Update()
    {  
        if (Input.GetKeyDown(key)) ToggleWallCollider(false);
        if (Input.GetKeyUp(key)) ToggleWallCollider(true);

        if (isMoving)
        {
            square.transform.position = Vector3.MoveTowards(
                square.transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(square.transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
                moveTrigger.enabled = true; 
                Debug.Log("Wall reached end position!");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isMoving)
        {
            StartMovement();
        }
    }

    private void InitializeComponents()
    {
        // 自动获取未手动赋值的组件
        if (!beginPos) beginPos = transform.Find("BeginPos");
        if (!endPos) endPos = transform.Find("EndPos");
        if (!moveTrigger) moveTrigger = GetComponent<BoxCollider2D>();
        if (!square) square = transform.Find("Square").gameObject;

        // 验证按键有效性
        if (key < KeyCode.A || key > KeyCode.Z)
        {
            Debug.LogWarning("Invalid key! Defaulting to KeyCode.N");
            key = KeyCode.N;
        }
    }

    private void ToggleWallCollider(bool state)
    {
        var collider = square.GetComponent<BoxCollider2D>();
        if (collider)
        {
            collider.enabled = state;
            Debug.Log($"Wall {(state ? "enabled" : "disabled")}!");
        }
    }

    private void StartMovement()
    {
        // 切换目标位置
        targetPosition =endPos.position;
        isMoving = true;
        moveTrigger.enabled = false; 
        Debug.Log("Wall moving!");
    }
}