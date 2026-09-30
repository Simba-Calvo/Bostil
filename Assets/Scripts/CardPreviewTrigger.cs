
using UnityEngine;
using UnityEngine.EventSystems;

public class CardPreviewTrigger : MonoBehaviour, IPointerClickHandler
{
    private CardData cardData;
    private CardPreviewUI previewUI;

    public void Initialize(CardData data, CardPreviewUI preview)
    {
        cardData = data;
        previewUI = preview;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (cardData != null && previewUI != null)
            {
                previewUI.ShowCard(cardData);
            }
        }
    }
}