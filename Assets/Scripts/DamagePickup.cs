using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SphereCollider))]
public class DamagePickup : MonoBehaviour
{
    [Header("Pickup")]
    public int damageIncrease = 1;

    [Header("Motion")]
    public float bobHeight = 0.25f;
    public float bobSpeed = 2.4f;
    public float spinSpeed = 110f;

    [Header("Visuals")]
    public Color pickupColor = new Color(1f, 0.78f, 0.15f);
    public Vector3 visualScale = new Vector3(1f, 1f, 1f);

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

    public void Collect(PlayerController playerController)
    {
        if (collected || playerController == null)
            return;

        playerController.IncreaseDamage(damageIncrease);
        collected = true;
        gameObject.SetActive(false);
    }

    private void ConfigureCollider()
    {
        SphereCollider trigger = GetComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 1.15f;
        trigger.center = new Vector3(0f, 0.55f, 0f);
    }

    private void EnsureVisual()
    {
        Transform visualRoot = transform.Find("DamageVisual");
        if (visualRoot == null)
        {
            visualRoot = new GameObject("DamageVisual").transform;
            visualRoot.SetParent(transform, false);

            CreatePiece(visualRoot, PrimitiveType.Cube, "Core", new Vector3(0f, 0.55f, 0f), new Vector3(0.38f, 0.95f, 0.38f), Quaternion.identity);
            CreatePiece(visualRoot, PrimitiveType.Cube, "LeftArm", new Vector3(-0.3f, 0.28f, 0f), new Vector3(0.3f, 0.55f, 0.24f), Quaternion.Euler(0f, 0f, 32f));
            CreatePiece(visualRoot, PrimitiveType.Cube, "RightArm", new Vector3(0.3f, 0.82f, 0f), new Vector3(0.3f, 0.55f, 0.24f), Quaternion.Euler(0f, 0f, 32f));
        }

        visualRoot.localScale = visualScale;

        foreach (Renderer renderer in visualRoot.GetComponentsInChildren<Renderer>())
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            block.SetColor("_BaseColor", pickupColor);
            block.SetColor("_Color", pickupColor);
            renderer.SetPropertyBlock(block);
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }
    }

    private void CreatePiece(Transform parent, PrimitiveType primitiveType, string pieceName, Vector3 localPosition, Vector3 localScale, Quaternion localRotation)
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
