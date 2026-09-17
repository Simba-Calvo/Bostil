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
}
