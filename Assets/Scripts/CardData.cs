using UnityEngine;

[CreateAssetMenu(menuName = "Cards/CardData")]
public class CardData : ScriptableObject
{
    [Tooltip("ID único (ex: CARD_001)")]
    public string Id;

    public string DisplayName;
    public Sprite Sprite;

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