
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckBuilder : MonoBehaviour
{
    [Header("Dados")]
    [SerializeField] private CardDatabase database;
    [SerializeField] private int maxCards = 15;

    [Header("Lista de cartas")]
    [SerializeField] private Transform cardListContent;
    [SerializeField] private Button cardButtonPrefab;

    [Header("Deck")]
    [SerializeField] private Button[] deckSlots;
    [SerializeField] private TMP_Text deckCount;
    [SerializeField] private Button saveButton;

    [Header("Pré-visualização")]
    [SerializeField] private CardPreviewUI previewUI;

    private DeckData currentDeck = new DeckData();

    private void Start()
    {
        if (database == null)
        {
            database = Resources.Load<CardDatabase>("CardDatabase");
        }

        if (database == null)
        {
            Debug.LogError("DeckBuilder: CardDatabase não encontrado!");
            return;
        }

        PopulateCardList();
        UpdateDeckUI();
    }

    public CardData[] GetAvailableCards()
    {
        return database != null
            ? database.cards
            : new CardData[0];
    }

    // Cria um botão para cada carta do banco
    public void PopulateCardList()
    {
        if (cardListContent == null || cardButtonPrefab == null)
        {
            Debug.LogError(
                "DeckBuilder: configure Card List e Card Button Prefab no Inspector."
            );
            return;
        }

        // Limpa os botões antigos
        foreach (Transform child in cardListContent)
        {
            Destroy(child.gameObject);
        }

        CardData[] cards = GetAvailableCards();

        for (int i = 0; i < cards.Length; i++)
        {
            CardData card = cards[i];

            if (card == null) continue;

            // Cria o botão
            Button newButton = Instantiate(
                cardButtonPrefab,
                cardListContent
            );

            // Configura o texto
            TMP_Text buttonText =
                newButton.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
            {
                buttonText.text = card.DisplayName;
            }

            // Configura a pré-visualização
            CardPreviewTrigger trigger =
                newButton.GetComponent<CardPreviewTrigger>();

            if (trigger != null)
            {
                trigger.Initialize(card, previewUI);
            }
            else
            {
                Debug.LogWarning(
                    "O prefab do botão não possui CardPreviewTrigger!"
                );
            }

            // Configura o clique esquerdo para adicionar
            string cardId = card.Id;

            newButton.onClick.RemoveAllListeners();

            newButton.onClick.AddListener(
                () => AddCardById(cardId)
            );
        }
    }

    public bool AddCardById(string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;

        if (currentDeck.cardIds.Count >= maxCards)
        {
            Debug.LogWarning("Deck cheio!");
            return false;
        }

        currentDeck.cardIds.Add(id);

        UpdateDeckUI();

        return true;
    }

    public void AddCardByIndex(int index)
    {
        CardData[] cards = GetAvailableCards();

        if (index < 0 || index >= cards.Length)
            return;

        if (cards[index] != null)
        {
            AddCardById(cards[index].Id);
        }
    }

    public bool RemoveCardAt(int index)
    {
        if (index < 0 ||
            index >= currentDeck.cardIds.Count)
        {
            return false;
        }

        currentDeck.cardIds.RemoveAt(index);

        UpdateDeckUI();

        return true;
    }

    // Atualiza os slots e o contador
    private void UpdateDeckUI()
    {
        if (deckCount != null)
        {
            deckCount.text =
                currentDeck.cardIds.Count + " / " + maxCards;
        }

        if (saveButton != null)
        {
            saveButton.interactable =
                currentDeck.cardIds.Count == maxCards;
        }

        if (deckSlots == null) return;

        for (int i = 0; i < deckSlots.Length; i++)
        {
            Button slot = deckSlots[i];

            if (slot == null) continue;

            TMP_Text slotText =
                slot.GetComponentInChildren<TMP_Text>();

            bool occupied =
                i < currentDeck.cardIds.Count;

            if (slotText != null)
            {
                slotText.text = occupied
                    ? currentDeck.cardIds[i]
                    : "Vazio";
            }

            int slotIndex = i;

            slot.onClick.RemoveAllListeners();

            if (occupied)
            {
                slot.onClick.AddListener(
                    () => RemoveCardAt(slotIndex)
                );
            }
        }
    }

    public void ClearDeck()
    {
        currentDeck.cardIds.Clear();
        UpdateDeckUI();
    }

    public bool SaveDeck()
    {
        if (currentDeck.cardIds.Count != maxCards)
        {
            Debug.LogWarning(
                "O deck precisa ter exatamente " + maxCards + " cartas."
            );

            return false;
        }

        DeckStorage.Save(currentDeck);

        Debug.Log("Deck salvo com sucesso!");

        return true;
    }

    public DeckData GetCurrentDeck()
    {
        return currentDeck;
    }

    public void OnSaveButtonClicked()
    {
        SaveDeck();
    }
}