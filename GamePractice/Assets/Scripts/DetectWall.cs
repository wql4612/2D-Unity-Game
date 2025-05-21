using UnityEngine;
using TMPro;

public class DetectWall : MonoBehaviour
{
    [SerializeField] private GameObject square;
    [SerializeField] private GameObject triggerObj; // 新增：Trigger对象
    [SerializeField] private KeyCode key = KeyCode.N;
    [SerializeField] public TextMeshPro textMesh;
    [SerializeField] private bool playerInside = false;
    private bool waitForToggle = false; // 新增等待标志

    void Start()
    {
        // 验证按键有效性
        if (key < KeyCode.A || key > KeyCode.Z)
        {
            Debug.LogWarning("Invalid key! Defaulting to KeyCode.N");
            key = KeyCode.N;
        }
        if (textMesh != null) textMesh.text = key.ToString().ToUpper();
        if (square == null)
        {
            square = transform.Find("moveObj").gameObject;
        }
        if (triggerObj == null)
        {
            triggerObj = transform.Find("TriggerObj")?.gameObject;
        }
        if (square == null)
        {
            Debug.LogWarning("No square found!");
            return;
        }
        if (triggerObj == null)
        {
            Debug.LogWarning("No triggerObj found!");
            return;
        }
        triggerObj.SetActive(false); // 初始关闭
    }

    void Update()
    {
        if(triggerObj.activeSelf)
            playerInside = triggerObj.GetComponent<TriggerDetecter>().playerInside;

        if (Input.GetKeyDown(key))
        {
            ToggleWallCollider(false);
        }
        if (Input.GetKeyUp(key))
        {
            if (playerInside)
            {
                waitForToggle = true; // 标记等待
            }
            else
            {
                ToggleWallCollider(true);
            }
        }
        // 检查是否需要延迟Toggle
        if (waitForToggle && !playerInside)
        {
            ToggleWallCollider(true);
            waitForToggle = false;
        }
    }

    private void ToggleWallCollider(bool state)
    {
        var collider = square.GetComponent<BoxCollider2D>();
        if (collider)
        {
            collider.isTrigger = !state; // state==true时恢复物理，isTrigger=false
            Debug.Log($"Wall collider isTrigger set to {!state}");
        }
        // 修改透明度
        var sr = square.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = state ? 1f : 0.5f;
            sr.color = c;
        }
        // 控制TriggerObj的激活
        if (triggerObj != null)
        {
            triggerObj.SetActive(!state); // 只有wall不可用时启用trigger
        }
    }
}