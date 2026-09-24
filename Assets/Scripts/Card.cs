using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Card : MonoBehaviour
{
    [Tooltip("ID que identifica qual CardData utilizar")]
    [SerializeField] private string cardId;

    [Tooltip("Opcional: arraste o CardDatabase neste campo. Se vazio, tentamos Resources.Load")]
    [SerializeField] private CardDatabase database;

    private CardData data;
    private SpriteRenderer spriteRenderer;

    private void OnValidate()
    {
        // Facilita na editor: tenta apontar para um asset em Resources se não especificado
#if UNITY_EDITOR
        if (database == null)
            database = UnityEngine.Resources.Load<CardDatabase>("CardDatabase");
#endif
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        LoadData();
    }

    public void LoadData()
    {
        if (database == null)
            database = Resources.Load<CardDatabase>("CardDatabase"); // coloque o asset em Resources/

        if (database == null)
        {
            Debug.LogWarning("CardDatabase não encontrado (procure por Resources/CardDatabase).");
            return;
        }

        data = database.GetById(cardId);
        if (data == null)
        {
            Debug.LogWarning($"CardData não encontrado para ID '{cardId}'");
            return;
        }

        ApplyDataToPrefab();
    }

    private void ApplyDataToPrefab()
    {
        if (spriteRenderer != null && data.Sprite != null)
            spriteRenderer.sprite = data.Sprite;

        var status = GetComponent<CartaStatus>();
        if (status != null)
        {
            status.HP = data.HP;
            status.MaxHP = data.MaxHP;
            status.DF = data.DF;
            status.Pontos = data.Pontos;
            status.AtaqueMuitoFraco = data.AtaqueMuitoFraco;
            status.AtaqueFraco = data.AtaqueFraco;
            status.AtaqueMedio = data.AtaqueMedio;
            status.AtaqueForte = data.AtaqueForte;
            status.IsDead = false;
        }

        // Se precisar executar lógica extra ao instanciar (ex: configurar UI do prefab), faça aqui.
    }

    // getters úteis
    public string CardId => cardId;
    public CardData Data => data;

    // Permite alterar ID em runtime e recarregar dados
    public void SetCardId(string id)
    {
        cardId = id;
        LoadData();
    }
}