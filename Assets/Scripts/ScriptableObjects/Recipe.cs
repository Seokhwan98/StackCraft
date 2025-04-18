using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
public class Recipe : ScriptableObject
{
    public string recipeName;
    public CardData[] inputCards;
    public CardData[] outputCards;
    [FormerlySerializedAs("ConsumeInputs")] public bool consumeInputs = true;
    
    // 0f = instant
    public float produceTime = 0f;
    
    private Dictionary<CardData, int> _inputCounts;
    private bool _isCacheValid = false;

    public int TotalInputCount => inputCards?.Length ?? 0;

    public Dictionary<CardData, int> GetInputCards()
    {
        if (!_isCacheValid || _inputCounts == null)
        {
            GenerateInputCards();
        }
        return _inputCounts;
    }

    private void GenerateInputCards()
    {
        _inputCounts = new Dictionary<CardData, int>();
        if (inputCards == null)
        {
            _isCacheValid = true;
            return;
        }

        foreach (CardData cardData in inputCards)
        {
            _inputCounts.TryGetValue(cardData, out int currentCount);
            _inputCounts[cardData] = currentCount + 1;
        }
        _isCacheValid = true;
    }

    private void OnValidate()
    {
        GenerateInputCards();
    }
    
    public void EnsureCacheGenerated()
    {
        GetInputCards();
    }
}
