using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Carrega o deck salvo automaticamente na cena de partida.
/// Opcional: chame InstantiateDeck para criar instâncias do prefab da carta usando SetCardId.
/// </summary>
public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }

    private List<string> deck = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadDeckFromStorage();
    }

    private void LoadDeckFromStorage()
    {
        var data = DeckStorage.Load();
        deck = new List<string>(data.cardIds);
        Debug.Log($"DeckManager: deck carregado com {deck.Count} cartas.");
    }

    public List<string> GetDeckIds() => new List<string>(deck);

    /// <summary>
    /// Instancia cartas a partir dos IDs usando um prefab padrão que possui o componente `Card`.
    /// </summary>
    public void InstantiateDeck(GameObject cardPrefab, Transform parent = null)
    {
        if (cardPrefab == null)
        {
            Debug.LogWarning("DeckManager: cardPrefab é null.");
            return;
        }

        foreach (var id in deck)
        {
            var go = Instantiate(cardPrefab, parent);
            var cardComp = go.GetComponent<Card>();
            if (cardComp != null)
                cardComp.SetCardId(id);
            else
                Debug.LogWarning("DeckManager: prefab não tem componente Card.");
        }
    }
}
