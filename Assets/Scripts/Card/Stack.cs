using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

[Serializable]
public class Stack: MonoBehaviour
{
    public event Action<Stack> OnStackModified;
    
    public List<Card> cards = new();
    public int Length => cards.Count;
    public Card TopCard => cards.Count > 0 ? cards[0] : null;
    public Card LastCard => cards.Count > 0 ? cards[^1] : null;

    public Dictionary<CardData, int> CardCounts = new();
    public bool IsOneKindOnly => CardCounts.Count == 1;
    
    private Timer _produceTimer;
    public Recipe producingRecipe;
    public bool HasTimer => _produceTimer is { isDone: false };
    
    public void Start()
    {
        GameTableManager.Instance.AddStackToTable(this);
    }
    
    public void OnDestroy()
    {
        GameTableManager.Instance.RemoveStackFromTable(this);
    }
    
    public void AddCard(Card card)
    {
        cards.Add(card);
        card.owningStack = this;
        ReorderZOrder();
        
        if (!CardCounts.TryAdd(card.cardData, 1))
        {
            CardCounts[card.cardData]++;
        }
        
        OnStackModified?.Invoke(this);
    }

    public void AddMultipleCards(Card[] inCards)
    {
        foreach (var card in inCards)
        {
            cards.Add(card);
            card.owningStack = this;
            
            if (!CardCounts.TryAdd(card.cardData, 1))
            {
                CardCounts[card.cardData]++;
            }
        }
        
        ReorderZOrder();
        OnStackModified?.Invoke(this);
    }
    
    public void RemoveCard(Card card)
    {
        cards.Remove(card);
        ReorderZOrder();
        if (cards.Count == 0)
        {
            Debug.Log("Stack count 0. Destroying stack...");
            Destroy(this);
        }

        if (CardCounts.TryGetValue(card.cardData, out var count))
        {
            if (count > 1)
            {
                CardCounts[card.cardData]--;
            }
            else
            {
                CardCounts.Remove(card.cardData);
            }
        }
        
        OnStackModified?.Invoke(this);
    }
    
    public void RemoveRange(int indexInStack, int prevStackLength)
    { 
        for (int i = indexInStack; i < indexInStack + prevStackLength; i++)
        {
            if (!CardCounts.TryGetValue(cards[i].cardData, out var count))
            {
                Debug.LogError($"Card {cards[i].cardData} not found in CardCounts");
                continue;
            }
            if (count > 1)
            {
                CardCounts[cards[i].cardData]--;
            }
            else
            {
                CardCounts.Remove(cards[i].cardData);
            }
        }
        
        cards.RemoveRange(indexInStack,  prevStackLength); 
        if (cards.Count == 0)
        {
            Destroy(this);
        }
        
        ReorderZOrder();
        OnStackModified?.Invoke(this);
    }
    
    public void ConsumeCard(Card card)
    {
        if (!card.cardData.isConsumable) return;
        
        cards.Remove(card);
        
        if (CardCounts.TryGetValue(card.cardData, out var count))
        {
            if (count > 1)
            {
                CardCounts[card.cardData]--;
            }
            else
            {
                CardCounts.Remove(card.cardData);
            }
        }
        
        Destroy(card.gameObject);
    }

    public void ConsumeCards(List<Card> cardsToConsume)
    {
        if (cardsToConsume.Count <= 0) return;
        
        foreach (var card in cardsToConsume)
        {
            Debug.Log($"Consuming card {card}");
            ConsumeCard(card);
        }
        
        Debug.Log($"Card count after consume : {cards.Count}");

        if (cards.Count == 0)
        {
            Destroy(this);
            return;
        }
        
        ReorderZOrder();
        ResetFollowTargets();
        
        OnStackModified?.Invoke(this);
    }

    private void ResetFollowTargets()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            var follow = cards[i].GetComponent<SlowParentConstraint>();
            if (follow)
            {
                if (i == 0)
                {
                    follow.enabled = false;
                    follow.target = null;
                }
                else
                {
                    follow.target = cards[i - 1].transform;
                }
            }
        }
    }

    public void ReorderZOrder(int sortingLayerId = 0)
    {
        float topz = 0f;
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].SetSortingLayer(i * 2, sortingLayerId);
            if (i == 0)
            {
                topz = cards[0].transform.position.z;
            }
            else
            {
                var pos = cards[i].transform.position;
                pos.z = topz;
                cards[i].transform.position = pos;
            }
        }
    }

    public void AddTimer(Recipe matchedRecipe, List<Card> consumedCards)
    {
        if (producingRecipe != null && producingRecipe == matchedRecipe)
        {
            return;
        }
        
        Timer.Cancel(_produceTimer);
        Debug.Log($"Starting recipe {matchedRecipe.recipeName} timer for {matchedRecipe.produceTime} seconds");
        producingRecipe = matchedRecipe;
        TopCard.cardTimerUI.gameObject.SetActive(true);
        _produceTimer = this.AttachTimer(matchedRecipe.produceTime,
            () => ApplyRecipe(matchedRecipe, consumedCards),
            onUpdate: (t) => this.TopCard.cardTimerUI.SetValue(t / matchedRecipe.produceTime),
            useRealTime: false,
            isLooped: true);
    }

    private void ApplyRecipe(Recipe matchedRecipe, List<Card> consumedCards)
    {
        RecipeManager.Instance.ApplyRecipe(this, matchedRecipe, consumedCards);

        if (!RecipeManager.Instance.CheckRecipe(this, matchedRecipe))
        {
            RemoveTimer();
        }
    }

    public void RemoveTimer()
    {
        Debug.Log($"Removing timer");
        TopCard.cardTimerUI.gameObject.SetActive(false);
        Timer.Cancel(_produceTimer);
        producingRecipe = null;
    }
}
