using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardPreviewUI : MonoBehaviour
{
    [Header("Painel")]
    public GameObject previewPanel;

    [Header("Informações da carta")]
    public Image cardImage;
    public TMP_Text cardName;
    public TMP_Text hpText;
    public TMP_Text defenseText;
    public TMP_Text pointsText;

    [Header("Ataques")]
    public TMP_Text attacksText;

    [Header("Habilidades")]
    public TMP_Text abilitiesText;

    private RectTransform previewRect;
    private Canvas parentCanvas;

    private void Start()
    {
        if (previewPanel != null)
        {
            previewPanel.SetActive(false);
            previewRect = previewPanel.GetComponent<RectTransform>();
            parentCanvas = previewPanel.GetComponentInParent<Canvas>();
        }
    }

    private void Update()
    {
        // Fecha o preview ao clicar fora do previewPanel (botão esquerdo)
        if (previewPanel != null && previewPanel.activeInHierarchy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 screenPos = Input.mousePosition;
                Camera cam = null;
                if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                {
                    cam = parentCanvas.worldCamera;
                }

                bool inside = false;
                if (previewRect != null)
                {
                    inside = RectTransformUtility.RectangleContainsScreenPoint(previewRect, screenPos, cam);
                }
                else if (EventSystem.current != null)
                {
                    // fallback: se o ponteiro não está sobre nenhum elemento UI, considerar como clique fora
                    inside = EventSystem.current.IsPointerOverGameObject();
                }

                if (!inside)
                {
                    ClosePreview();
                }
            }
        }
    }

    public void ShowCard(CardData card)
    {
        if (card == null)
            return;

        previewPanel.SetActive(true);

        // Nome
        if (cardName != null)
        {
            cardName.text = string.IsNullOrEmpty(card.DisplayName)
                ? card.Id
                : card.DisplayName;
        }

        // Imagem
        if (cardImage != null)
        {
            cardImage.sprite = card.Sprite;
            cardImage.enabled = card.Sprite != null;
        }

        // Vida
        if (hpText != null)
        {
            hpText.text =
                "HP: " + card.HP + " / " + card.MaxHP;
        }

        // Defesa
        if (defenseText != null)
        {
            defenseText.text = "Defesa: " + card.DF;
        }

        // Custo
        if (pointsText != null)
        {
            pointsText.text = "Custo: " + card.Pontos + " P";
        }

        // Ataques
        if (attacksText != null)
        {
            string attacks = "";

            if (card.PossuiAtaqueMuitoFraco)
            {
                attacks +=
                    "Muito fraco: " +
                    card.AtaqueMuitoFraco + "\n";
            }

            attacks +=
                "Fraco: " + card.AtaqueFraco + "\n" +
                "Médio: " + card.AtaqueMedio + "\n" +
                "Forte: " + card.AtaqueForte;

            attacksText.text = attacks;
        }

        // Habilidades
        if (abilitiesText != null)
        {
            int count = card.Abilities != null
                ? card.Abilities.Length
                : 0;

            abilitiesText.text = count == 0
                ? "Sem habilidade"
                : "Habilidades cadastradas: " + count;
        }
    }

    public void ClosePreview()
    {
        if (previewPanel != null)
        {
            previewPanel.SetActive(false);
        }
    }
}