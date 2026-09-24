using UnityEngine;

public abstract class CardAbility : ScriptableObject
{
    public string AbilityName;
    // Exemplo de método que a habilidade executa.
    // Implemente várias habilidades derivadas com lógica específica.
    public abstract void Execute(GameObject user, GameObject target);
}