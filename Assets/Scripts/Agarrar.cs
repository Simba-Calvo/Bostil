using UnityEngine;

public class Agarrar : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask grabbableLayerMask;
    [SerializeField] private float followSpeed = 20f;
    [SerializeField] private float grabDrag = 10f;
    [SerializeField] private float throwMultiplier = 1f;
    [SerializeField] private float rotationAngle = 90f;
    [SerializeField] private float rotationSpeed = 30f; // deg/s, maior = mais rápido

    private Rigidbody2D grabbedRb;
    private Vector2 grabOffset;
    private float originalGravity;
    private float originalDrag;
    private Vector2 lastTarget;
    private bool hasLastTarget = false;

    // rotação
    private float originalRotation;
    private float targetRotation;
    private bool isRotated = false;

    private void Reset()
    {
        mainCamera = Camera.main;
    }

    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) TryGrab();
        if (Input.GetMouseButtonUp(0)) Release();

        // toggle rotação enquanto segura
        if (grabbedRb != null && Input.GetKeyDown(KeyCode.R))
        {
            isRotated = !isRotated;
            targetRotation = originalRotation + (isRotated ? rotationAngle : 0f);
        }
    }

    private void FixedUpdate()
    {
        if (grabbedRb == null) return;

        Vector3 mouseWorld3 = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 target = new Vector2(mouseWorld3.x, mouseWorld3.y) + grabOffset;

        Vector2 newPos = Vector2.Lerp(grabbedRb.position, target, 1f - Mathf.Exp(-followSpeed * Time.fixedDeltaTime));
        grabbedRb.MovePosition(newPos);

        // rotação suave usando MoveRotation
        float currentRot = grabbedRb.rotation;
        float lerpFactor = 1f - Mathf.Exp(-rotationSpeed * Time.fixedDeltaTime);
        float newRot = Mathf.LerpAngle(currentRot, targetRotation, lerpFactor);
        grabbedRb.MoveRotation(newRot);

        lastTarget = target;
        hasLastTarget = true;
    }

    private void TryGrab()
    {
        Vector3 mouseWorld3 = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 point = new Vector2(mouseWorld3.x, mouseWorld3.y);

        Collider2D[] cols = Physics2D.OverlapPointAll(point);
        if (cols == null || cols.Length == 0) return;

        Collider2D chosen = null;
        foreach (var c in cols)
        {
            if ((grabbableLayerMask.value & (1 << c.gameObject.layer)) != 0)
            {
                chosen = c;
                break;
            }
        }

        if (chosen == null) return;

        Rigidbody2D rb = chosen.attachedRigidbody;
        if (rb == null) return;

        grabbedRb = rb;
        grabOffset = grabbedRb.position - point;

        originalGravity = grabbedRb.gravityScale;
        originalDrag = grabbedRb.linearDamping;

        grabbedRb.gravityScale = 0f;
        grabbedRb.linearDamping = grabDrag;
        grabbedRb.angularVelocity = 0f;
        grabbedRb.linearVelocity = Vector2.zero;

        // setup rotação
        originalRotation = grabbedRb.rotation;
        isRotated = false;
        targetRotation = originalRotation;

        hasLastTarget = false;
    }

    private void Release()
    {
        if (grabbedRb == null) return;

        if (hasLastTarget)
        {
            Vector3 mouseWorld3 = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 currentTarget = new Vector2(mouseWorld3.x, mouseWorld3.y) + grabOffset;
            Vector2 estimatedVel = (currentTarget - lastTarget) / Time.fixedDeltaTime * throwMultiplier;
            grabbedRb.linearVelocity = estimatedVel;
        }

        grabbedRb.gravityScale = originalGravity;
        grabbedRb.linearDamping = originalDrag;
        grabbedRb = null;
    }
}
