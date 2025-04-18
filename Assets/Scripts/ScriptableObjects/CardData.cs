using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    public string description;
    public Sprite sprite;
    public CardType cardType;
    public bool isConsumable = true;
}
