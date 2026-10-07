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
    [SerializeField] public CardPreviewUI previewUI;

    [Header("Limites por raridade (por carta)")]
    [SerializeField] private int maxCopies_Comum = 5;
    [SerializeField] private int maxCopies_Incomum = 4;
    [SerializeField] private int maxCopies_Raro = 3;
    [SerializeField] private int maxCopies_Epico = 2;
    [SerializeField] private int maxCopies_Mitico = 1;

    [Header("Limites de raridade alta")]
    [SerializeField] private int maxLendarias = 3;
    [SerializeField] private int maxDivinas = 2;
    [SerializeField] private int maxCelestiais = 1;

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

        // Carrega deck salvo (se existir) e garante tamanho máximo
        var loaded = DeckStorage.Load();
        if (loaded != null && loaded.cardIds != null)
        {
            currentDeck = loaded;
            if (currentDeck.cardIds.Count > maxCards)
            {
                currentDeck.cardIds = currentDeck.cardIds.GetRange(0, maxCards);
            }
        }
        else
        {
            currentDeck = new DeckData();
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

    private bool IsHighRarity(CardRarity r)
    {
        return r == CardRarity.Lendario ||
               r == CardRarity.Divino ||
               r == CardRarity.Celestial;
    }

    private int GetPerCardLimit(CardRarity r)
    {
        switch (r)
        {
            case CardRarity.Comum: return maxCopies_Comum;
            case CardRarity.Incomum: return maxCopies_Incomum;
            case CardRarity.Raro: return maxCopies_Raro;
            case CardRarity.Epico: return maxCopies_Epico;
            case CardRarity.Mitico: return maxCopies_Mitico;
            default: return 1;
        }
    }

    private int CountCardInDeck(string id)
    {
        int c = 0;
        for (int i = 0; i < currentDeck.cardIds.Count; i++)
        {
            if (currentDeck.cardIds[i] == id) c++;
        }
        return c;
    }

    private int CountRarityInDeck(CardRarity rarity)
    {
        int count = 0;

        for (int i = 0; i < currentDeck.cardIds.Count; i++)
        {
            var cid = currentDeck.cardIds[i];
            var cd = database != null ? database.GetById(cid) : null;

            if (cd != null && cd.Rarity == rarity)
            {
                count++;
            }
        }

        return count;
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

        if (database == null)
        {
            Debug.LogWarning("CardDatabase não configurado.");
            return false;
        }

        var card = database.GetById(id);
        if (card == null)
        {
            Debug.LogWarning("Carta não encontrada no banco: " + id);
            return false;
        }

        if (IsHighRarity(card.Rarity))
        {
            // não pode duplicar cartas dessas raridades
            if (CountCardInDeck(id) > 0)
            {
                Debug.LogWarning("Cartas " + card.Rarity + " não podem ter duplicatas: " + id);
                return false;
            }

            int currentHigh = CountHighRarityInDeck();
            if (currentHigh >= maxHighRarityCards)
            {
                Debug.LogWarning("Limite atingido para cartas de raridade Lendário+ (" + maxHighRarityCards + ")");
                return false;
            }
        }
        else
        {
            int copies = CountCardInDeck(id);
            int limit = GetPerCardLimit(card.Rarity);
            if (copies >= limit)
            {
                Debug.LogWarning("Limite de cópias atingido para " + card.DisplayName + " (" + limit + ")");
                return false;
            }
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

            // Tenta obter dados da carta pelo banco para mostrar DisplayName
            CardData card = null;
            if (occupied && database != null)
            {
                var id = currentDeck.cardIds[i];
                card = database.GetById(id);
            }

            if (slotText != null)
            {
                slotText.text = occupied
                    ? (card != null && !string.IsNullOrEmpty(card.DisplayName)
                        ? card.DisplayName
                        : currentDeck.cardIds[i])
                    : "Vazio";
            }

            int slotIndex = i;

            slot.onClick.RemoveAllListeners();

            // Remove ao clicar quando houver carta
            if (occupied)
            {
                slot.onClick.AddListener(
                    () => RemoveCardAt(slotIndex)
                );
            }

            // Configura pré-visualização por clique direito no slot
            var trigger = slot.GetComponent<CardPreviewTrigger>();
            if (occupied)
            {
                if (card != null)
                {
                    if (trigger == null)
                    {
                        trigger = slot.gameObject.AddComponent<CardPreviewTrigger>();
                    }
                    trigger.Initialize(card, previewUI);
                }
                else
                {
                    // Se não há CardData correspondente, remove trigger existente
                    if (trigger != null)
                    {
                        Destroy(trigger);
                    }
                }
            }
            else
            {
                if (trigger != null)
                {
                    Destroy(trigger);
                }
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