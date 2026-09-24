using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/CardDatabase")]
public class CardDatabase : ScriptableObject
{
    public CardData[] cards;

    private Dictionary<string, CardData> lookup;

    private void OnEnable()
    {
        lookup = new Dictionary<string, CardData>();
        if (cards == null) return;
        foreach (var c in cards)
        {
            if (c == null || string.IsNullOrEmpty(c.Id)) continue;
            lookup[c.Id] = c;
        }
    }

    public CardData GetById(string id)
    {
        if (lookup == null) OnEnable();
        if (string.IsNullOrEmpty(id)) return null;
        lookup.TryGetValue(id, out var data);
        return data;
    }
}