using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    public Transform fill;
    public Transform parentEnemy;
    public Color fullColor = Color.green;
    public Color midColor = Color.yellow;
    public Color lowColor = Color.red;
    private Camera targetCamera;
    private Vector3 fillBaseScale;
    private Renderer fillRenderer;

    void Awake()
    {
        transform.SetParent(null);
        if (fill != null)
        {
            fillBaseScale = fill.localScale;
            fillRenderer = fill.GetComponent<Renderer>();
        }
    }

    void Start()
    {
        targetCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (parentEnemy != null)
            transform.position = parentEnemy.position + Vector3.up * 2f;

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null) return;
        }
        transform.rotation = Quaternion.LookRotation(transform.position - targetCamera.transform.position);
    }

    public void SetFill(float ratio)
    {
        if (fill == null) return;
        ratio = Mathf.Clamp01(ratio);
        Vector3 s = fillBaseScale;
        s.x = fillBaseScale.x * ratio;
        fill.localScale = s;
        if (fillRenderer != null)
        {
            Color target = ratio > 0.5f
                ? Color.Lerp(midColor, fullColor, (ratio - 0.5f) * 2f)
                : Color.Lerp(lowColor, midColor, ratio * 2f);
            fillRenderer.material.color = target;
        }
    }
}