using UnityEngine;

public static class DeckStorage
{
    private const string PrefKey = "PlayerDeck_v1";

    public static void Save(DeckData deck)
    {
        var json = JsonUtility.ToJson(deck);
        PlayerPrefs.SetString(PrefKey, json);
        PlayerPrefs.Save();
    }

    public static DeckData Load()
    {
        if (!PlayerPrefs.HasKey(PrefKey)) return new DeckData();
        var json = PlayerPrefs.GetString(PrefKey);
        try
        {
            return JsonUtility.FromJson<DeckData>(json) ?? new DeckData();
        }
        catch
        {
            Debug.LogWarning("DeckStorage: falha ao desserializar deck, retornando vazio.");
            return new DeckData();
        }
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(PrefKey);
    }
}
