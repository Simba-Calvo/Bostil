using UnityEngine;
using UnityEngine.UI;
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

    private void Start()
    {
        if (previewPanel != null)
        {
            previewPanel.SetActive(false);
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
                ? "Nenhuma habilidade cadastrada."
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