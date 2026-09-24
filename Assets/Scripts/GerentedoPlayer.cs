using UnityEngine;
using TMPro;

public class GerentedoPlayer : MonoBehaviour
{
    public int Pontos = 100;
    public int Cartas = 15;
    public int AtualRound = 1;
    public int PontosperRound = 30;

    [Header("HUD (TextMeshPro)")]
    [SerializeField] private TextMeshProUGUI pontosText;
    [SerializeField] private TextMeshProUGUI roundText;

    void Start()
    {
        AtualizarHUD();
    }

    public void ProximoRound()
    {
        AtualRound++;
        GanharPontos(PontosperRound);
        AtualizarHUD();
    }

    public void GanharPontos(int pontos)
    {
        Pontos += pontos;
        AtualizarHUD();
    }

    private void AtualizarHUD()
    {
        if (pontosText != null)
            pontosText.text = $"Pontos: {Pontos}";

        if (roundText != null)
            roundText.text = $"Round: {AtualRound}";
    }

    // getters úteis para outros scripts
    public int GetPontos() => Pontos;
    public int GetRound() => AtualRound;
}
