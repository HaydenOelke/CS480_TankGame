using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SphereCollider))]
public class HealthPickup : MonoBehaviour
{
    [Header("Pickup")]
    public int healAmount = 20;

    [Header("Motion")]
    public float bobHeight = 0.35f;
    public float bobSpeed = 2f;
    public float spinSpeed = 90f;

    [Header("Visuals")]
    public Color heartColor = new Color(0.95f, 0.2f, 0.3f);
    public Vector3 visualScale = new Vector3(1.1f, 1.1f, 1.1f);

    private Vector3 basePosition;
    private bool collected;

    void Reset()
    {
        ConfigureCollider();
        gameObject.tag = "PickUp";
    }

    void Awake()
    {
        basePosition = transform.position;
        ConfigureCollider();
        EnsureVisual();
    }

    void OnEnable()
    {
        basePosition = transform.position;
    }

    void OnValidate()
    {
        ConfigureCollider();
        EnsureVisual();
    }

    void Update()
    {
        if (!Application.isPlaying)
            return;

        float bobOffset = (Mathf.Sin(Time.time * bobSpeed) + 1f) * 0.5f * bobHeight;
        transform.position = basePosition + Vector3.up * bobOffset;
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
    }

    public void Collect(PlayerHealth playerHealth)
    {
        if (collected || playerHealth == null)
            return;

        playerHealth.Heal(healAmount);
        collected = true;
        gameObject.SetActive(false);
    }

    private void ConfigureCollider()
    {
        SphereCollider trigger = GetComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 1.2f;
        trigger.center = new Vector3(0f, 0.65f, 0f);
    }

    private void EnsureVisual()
    {
        Transform visualRoot = transform.Find("HeartVisual");
        if (visualRoot == null)
        {
            visualRoot = new GameObject("HeartVisual").transform;
            visualRoot.SetParent(transform, false);
            CreateHeartPiece(visualRoot, PrimitiveType.Sphere, "TopLeft", new Vector3(-0.28f, 0.92f, 0f), new Vector3(0.55f, 0.55f, 0.42f), Quaternion.identity);
            CreateHeartPiece(visualRoot, PrimitiveType.Sphere, "TopRight", new Vector3(0.28f, 0.92f, 0f), new Vector3(0.55f, 0.55f, 0.42f), Quaternion.identity);
            CreateHeartPiece(visualRoot, PrimitiveType.Cube, "Bottom", new Vector3(0f, 0.46f, 0f), new Vector3(0.68f, 0.68f, 0.26f), Quaternion.Euler(0f, 0f, 45f));
        }

        visualRoot.localScale = visualScale;

        foreach (Renderer renderer in visualRoot.GetComponentsInChildren<Renderer>())
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            block.SetColor("_BaseColor", heartColor);
            block.SetColor("_Color", heartColor);
            renderer.SetPropertyBlock(block);
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }
    }

    private void CreateHeartPiece(Transform parent, PrimitiveType primitiveType, string pieceName, Vector3 localPosition, Vector3 localScale, Quaternion localRotation)
    {
        GameObject piece = GameObject.CreatePrimitive(primitiveType);
        piece.name = pieceName;
        piece.transform.SetParent(parent, false);
        piece.transform.localPosition = localPosition;
        piece.transform.localRotation = localRotation;
        piece.transform.localScale = localScale;

        Collider pieceCollider = piece.GetComponent<Collider>();
        if (pieceCollider != null)
        {
            if (Application.isPlaying)
                Destroy(pieceCollider);
            else
                DestroyImmediate(pieceCollider);
        }
    }
}
