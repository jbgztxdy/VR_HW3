using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class KeyButton : MonoBehaviour, IPointerClickHandler
{
    private CurveKeyManager manager;
    private Image keyImage;           // 用于控制颜色
    private bool isPressed = false;   // 当前状态
    private Vector3 originalPos;      // 初始位置（松开）
    private Vector3 pressedPos;       // 按下位置（Z轴移动后）
    private float moveSpeed;
    private float pressDistance;
    private Color normalColor;
    private Color pressedColor;
    private Coroutine moveCoroutine;  // 平滑移动协程

    // 初始化按键属性
    public void Init(
        CurveKeyManager _manager, 
        float _moveSpeed, 
        float _pressDistance, 
        Color _normalColor, 
        Color _pressedColor
    )
    {
        manager = _manager;
        moveSpeed = _moveSpeed;
        pressDistance = _pressDistance;
        normalColor = _normalColor;
        pressedColor = _pressedColor;

        keyImage = GetComponent<Image>();
        if (keyImage == null)
            keyImage = gameObject.AddComponent<Image>();

        // 记录初始位置和按下位置（Z轴正向移动）
        originalPos = transform.localPosition;
        pressedPos = originalPos + Vector3.forward * pressDistance;

        // 初始状态
        SetState(false);
    }

    // 切换状态
    public void ToggleState()
    {
        isPressed = !isPressed;
        SetState(isPressed);
        manager.UpdatePressedCount(isPressed);
    }

    // 设置状态
    private void SetState(bool pressed)
    {
        // 停止当前移动协程（防止冲突）
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        Vector3 targetPos = pressed ? pressedPos : originalPos;
        moveCoroutine = StartCoroutine(MoveTo(targetPos));

        keyImage.color = pressed ? pressedColor : normalColor;
    }

    // 平滑移动到目标位置的协程
    private IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.localPosition, target) > 0.01f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                target,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        transform.localPosition = target;
    }

    // 处理鼠标点击
    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleState();
    }

    // VR交互接口
    public void OnRaycastTrigger()
    {
        ToggleState();
    }
}