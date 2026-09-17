using UnityEngine;

public class AttachCard : MonoBehaviour
{
    [SerializeField] private LayerMask cardLayerMask;
    [SerializeField] private bool detachOnClick = true;
    [SerializeField] private bool snapRotation = false;

    private Collider2D candidateCollider;
    private GameObject attachedCard;
    private Rigidbody2D attachedRb;

    // propriedades originais para restaurar
    private RigidbodyType2D originalBodyType;
    private float originalGravity;
    private float originalDrag;

    private void Reset()
    {
        // configura padrão se necessário
    }

    private void Update()
    {
        // se houver um candidato sobre o slot e o jogador soltar o botão do mouse -> anexar
        if (candidateCollider != null && attachedCard == null && Input.GetMouseButtonUp(0))
        {
            TryAttach(candidateCollider.gameObject);
        }

        // permitir desanexar ao clicar na carta anexada
        if (detachOnClick && attachedCard != null && Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 point = new Vector2(mouseWorld.x, mouseWorld.y);

            Collider2D hit = Physics2D.OverlapPoint(point);
            if (hit != null && hit.gameObject == attachedCard)
            {
                Detach();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((cardLayerMask.value & (1 << other.gameObject.layer)) == 0) return;
        // aceita apenas se tiver um Rigidbody2D (cartas devem ter)
        if (other.attachedRigidbody == null) return;
        candidateCollider = other;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (candidateCollider == other) candidateCollider = null;
    }

    public bool TryAttach(GameObject card)
    {
        if (card == null) return false;
        if ((cardLayerMask.value & (1 << card.layer)) == 0) return false;

        Rigidbody2D rb = card.GetComponent<Rigidbody2D>();
        if (rb == null) return false;

        // guarda referências e propriedades originais
        attachedCard = card;
        attachedRb = rb;
        originalBodyType = rb.bodyType;
        originalGravity = rb.gravityScale;
        originalDrag = rb.linearDamping;

        // parar movimento
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // tornar kinematic para fixar no slot (evita física indesejada)
        rb.bodyType = RigidbodyType2D.Kinematic;

        // parent e centraliza no slot, preservando rotação se desejado
        Quaternion originalRot = card.transform.rotation;
        card.transform.SetParent(transform, false); // localPosition usado abaixo
        card.transform.localPosition = Vector3.zero;
        if (snapRotation)
        {
            card.transform.localRotation = Quaternion.identity;
        }
        else
        {
            card.transform.rotation = originalRot;
        }

        return true;
    }

    public bool Detach()
    {
        if (attachedCard == null || attachedRb == null) return false;

        // remove parent e restaura física
        Transform cardT = attachedCard.transform;
        cardT.SetParent(null, true); // mantém transform mundial atual

        attachedRb.bodyType = originalBodyType;
        attachedRb.gravityScale = originalGravity;
        attachedRb.linearDamping = originalDrag;

        attachedCard = null;
        attachedRb = null;
        return true;
    }

    // chamada útil por outros scripts (ex: Agarrar) para forçar anexar/detachar
    public bool IsAttached => attachedCard != null;
    public GameObject AttachedCard => attachedCard;
}
