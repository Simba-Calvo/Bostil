using UnityEngine;
using UnityEngine.SceneManagement;

public class Botões : MonoBehaviour
{
    public void Sair()
    {
        Application.Quit();
    }
    public void Jogar()
    {
        SceneManager.LoadScene("Partida");
    }
    public void DeckBuilder()
    {
        SceneManager.LoadScene("DeckBuilder");
    }
    public void Voltar()
    {
        SceneManager.LoadScene("Menu");
    }
    public void PassarRound()
    {
        // buscar o método ProximoRound() do script GerentedoPlayer e chamar ele
        GerentedoPlayer gerentedoPlayer = Object.FindFirstObjectByType<GerentedoPlayer>();
        if (gerentedoPlayer != null)
        {
            gerentedoPlayer.ProximoRound();
        }
        else
        {
            Debug.LogWarning("GerentedoPlayer não encontrado na cena.");
        }
    }
}
