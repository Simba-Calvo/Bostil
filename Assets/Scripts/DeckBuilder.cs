using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia a construção do deck na cena DeckBuilder.
/// Ligue os botões/itens da UI aos métodos públicos abaixo:
/// - AddCardById(string id)
/// - RemoveCardAt(int index)
/// - SaveDeck()
/// - ClearDeck()
/// Use GetAvailableCards() para popular a UI a partir do CardDatabase.
/// </summary>
public class DeckBuilder : MonoBehaviour
{
    [SerializeField] private CardDatabase database;
    [SerializeField] private int maxCards = 15;

    private DeckData currentDeck = new DeckData();

    private void Reset()
    {
        if (database == null)
            database = Resources.Load<CardDatabase>("CardDatabase");
    }

    public CardData[] GetAvailableCards()
    {
        return database != null ? database.cards : new CardData[0];
    }

    public bool AddCardById(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        if (currentDeck.cardIds.Count >= maxCards)
        {
            Debug.LogWarning("DeckBuilder: deck cheio.");
            return false;
        }

        currentDeck.cardIds.Add(id);
        return true;
    }

    public bool RemoveCardAt(int index)
    {
        if (index < 0 || index >= currentDeck.cardIds.Count) return false;
        currentDeck.cardIds.RemoveAt(index);
        return true;
    }

    public void ClearDeck()
    {
        currentDeck.cardIds.Clear();
    }

    public bool SaveDeck()
    {
        if (currentDeck.cardIds.Count != maxCards)
        {
            Debug.LogWarning($"DeckBuilder: o deck precisa ter exatamente {maxCards} cartas antes de salvar.");
            return false;
        }

        DeckStorage.Save(currentDeck);
        Debug.Log("DeckBuilder: deck salvo com sucesso.");
        return true;
    }

    // Métodos auxiliares para UI (ex.: botões)
    public void AddCardByIndex(int index)
    {
        var cards = GetAvailableCards();
        if (index < 0 || index >= cards.Length) return;
        AddCardById(cards[index].Id);
    }

    public DeckData GetCurrentDeck() => currentDeck;
}
