using UnityEngine;

public enum CardRarity
{
    Comum,
    Incomum,
    Raro,
    Epico,
    Mitico,
    Lendario,
    Divino,
    Celestial,
    EX
}

[CreateAssetMenu(menuName = "Cards/CardData")]
public class CardData : ScriptableObject
{
    [Tooltip("ID único (ex: CARD_001)")]
    public string Id;

    public string DisplayName;

    [Header("Sprites")]
    [Tooltip("Sprite principal usado no campo de batalha e no preview detalhado.")]
    public Sprite Sprite;

    [Tooltip("Sprite reduzido usado nos slots de seleção.")]
    public Sprite SelectionSprite;

    [Header("Raridade")]
    public CardRarity Rarity = CardRarity.Comum;

    [Header("Status")]
    public int HP;
    public int MaxHP;
    public int DF;
    public int Pontos;

    [Header("Ataques")]
    public bool PossuiAtaqueMuitoFraco;
    public int AtaqueMuitoFraco;
    public int AtaqueFraco;
    public int AtaqueMedio;
    public int AtaqueForte;

    [Header("Ações / Habilidades")]
    public CardAbility[] Abilities;
}