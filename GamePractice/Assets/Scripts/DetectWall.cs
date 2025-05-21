using UnityEngine;
using TMPro;

public class DetectWall : MonoBehaviour
{
    [SerializeField] private GameObject square;
    [SerializeField] private KeyCode key = KeyCode.N;
    [SerializeField] public TextMeshPro textMesh;
    [SerializeField] private bool isStay;
    void Start()
    {
        // 验证按键有效性
        if (key < KeyCode.A || key > KeyCode.Z)
        {
            Debug.LogWarning("Invalid key! Defaulting to KeyCode.N");
            key = KeyCode.N;
        }
        if (textMesh != null) textMesh.text = key.ToString().ToUpper();
        if(square == null)
        {
            square = transform.Find("moveObj").gameObject;
        }
        if (square == null)
        {
            Debug.LogWarning("No square found!");
            return;
        }
    }

    void Update()
    {  
        if(!isStay){
            if (Input.GetKeyDown(key)) ToggleWallCollider(false);
            if (Input.GetKeyUp(key)) ToggleWallCollider(true);
        }
    }
    private void ToggleWallCollider(bool state)
    {
        //取消碰撞体
        var collider = square.GetComponent<BoxCollider2D>();
        if (collider)
        {
            collider.enabled = state;
            Debug.Log($"Wall {(state ? "enabled" : "disabled")}!");
        }
        // 修改透明度
        var sr = square.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = state ? 1f : 0.5f;
            sr.color = c;
        }
    }
}