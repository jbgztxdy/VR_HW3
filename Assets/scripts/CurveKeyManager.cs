using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurveKeyManager : MonoBehaviour
{
    public Canvas uiCanvas;
    public GameObject keyPrefab;        // 按键预制体
    public TextMeshProUGUI countText;   // 统计文本
    public int rowCount = 30;           // 行数
    public int colCount = 20;           // 列数
    public float sphereRadius = 300f;   // 曲面半径
    public float moveSpeed = 50f;       // 移动速度
    public float pressDistance = 10f;   // 凹陷距离
    public Color normalColor = Color.white;  // 松开状态颜色
    public Color pressedColor = Color.red;   // 按下状态颜色

    private List<KeyButton> allKeys = new List<KeyButton>();
    private int pressedTotal = 0;

    void Start()
    {
        GenerateCurveKeys();
        UpdateCountDisplay();
    }

    // 在曲面上生成按键
    void GenerateCurveKeys()
    {
        float keyWidth = 20f;
        float keyHeight = 5f;
        float spacing = 0.5f;   // 按键间距

        // 计算矩形区域的中心偏移
        float centerOffsetX = (rowCount - 1) * (keyWidth + spacing) / 2;
        float centerOffsetY = (colCount - 1) * (keyHeight + spacing) / 2;

        // 半圆凸起参数
        float curveStrength = 50f;  // 曲率强度
        float maxZ = 100f;          // 边缘最大Z值

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                // 计算X/Y坐标
                float x = row * (keyWidth + spacing) - centerOffsetX;
                float y = col * (keyHeight + spacing) - centerOffsetY;

                // 计算Z坐标（形成向Z轴负方向凸起的半圆）
                float normalizedX = x / centerOffsetX; 
                // 二次函数：Z = maxZ - curveStrength*(1 - normalizedX²)
                // 中间（X=0）Z值最小（向Z负方向凸起），边缘（X±max）Z值最大
                float z = maxZ - curveStrength * (1 - normalizedX * normalizedX);

                // 实例化按键
                GameObject keyObj = Instantiate(
                    keyPrefab, 
                    new Vector3(x, y, z), 
                    Quaternion.identity,
                    uiCanvas.transform
                );
                keyObj.name = $"Key_{row}_{col}";

                // 初始化按键
                KeyButton key = keyObj.AddComponent<KeyButton>();
                key.Init(
                    this, 
                    moveSpeed, 
                    pressDistance, 
                    normalColor, 
                    pressedColor
                );
                allKeys.Add(key);
            }
        }
    }

    // 更新按下总数
    public void UpdatePressedCount(bool isPressed)
    {
        pressedTotal += isPressed ? 1 : -1;
        UpdateCountDisplay();
    }

    // 更新统计文本
    void UpdateCountDisplay()
    {
        countText.text = $"Pressed Num: {pressedTotal}";
    }
}