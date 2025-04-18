using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTableManager : MonoBehaviour
{
    public static GameTableManager Instance;
    public event Action<Card> CardAddedOnTable;
    public event Action<Card> CardRemovedFromTable;

    public event Action<Stack> StackAddedOnTable;
    public event Action<Stack> StackRemovedFromTable;
    
    [SerializeField] public List<Card> cardsOnTable;
    [SerializeField] public List<Stack> stacksOnTable;

    [SerializeField] private GameObject cardPrefab;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        StackManager.Instance.CardAddedToStackByDrag += HideCardCanStackIndicator;
    }

    public void AddNewCardToTable(CardData newCardData, Vector3 position)
    {
        var newCard = Instantiate(cardPrefab, position, Quaternion.identity);
        newCard.GetComponent<Card>().cardData = newCardData;
    }

    public void AddCardToTable(Card card)
    {
        if (!cardsOnTable.Contains(card))
        {
            cardsOnTable.Add(card);
            
#if UNITY_EDITOR
            card.name = card.cardData.cardName + " " + cardsOnTable.Count((c) => c.cardData == card.cardData);
#endif
            
            var cardDrag = card.GetComponent<CardDrag>();
            if (cardDrag != null)
            {
                cardDrag.CardDragStarted += OnCardDragStarted;
                cardDrag.CardDragEnded += OnCardDragEnded;
            }
            else
            {
                //Debug.LogError($"CardDrag component not found on {card.name}");
            }
                
            CardAddedOnTable?.Invoke(card);
        }
    }
    
    public void RemoveCardFromTable(Card card)
    {
        if (cardsOnTable.Contains(card))
        {
            cardsOnTable.Remove(card);
            var cardDrag = card.GetComponent<CardDrag>();
            if (cardDrag != null)
            {
                cardDrag.CardDragStarted -= OnCardDragStarted;
                cardDrag.CardDragEnded -= OnCardDragEnded;
            }
            
            CardRemovedFromTable?.Invoke(card);
        }
    }
    
    public void AddStackToTable(Stack stack)
    {
        if (!stacksOnTable.Contains(stack))
        {
            stacksOnTable.Add(stack);
            StackAddedOnTable?.Invoke(stack);
        }
    }
    
    public void RemoveStackFromTable(Stack stack)
    {
        if (stacksOnTable.Contains(stack))
        {
            stacksOnTable.Remove(stack);
            StackRemovedFromTable?.Invoke(stack);
        }
    }
    
    private void OnCardDragStarted(Card card)
    {
        //Debug.Log($"Card {card.name} started dragging");
        ShowCardCanStackIndicator(card);
        var owningStack = card.owningStack;
        if (owningStack)
        {
            foreach (var stackCard in owningStack.cards)
            {
                stackCard.gameObject.layer = LayerMask.NameToLayer("DraggingCard");
            }
        }
    }

    private void OnCardDragEnded(Card card)
    {
        //Debug.Log($"Card {obj.name} ended dragging");
        HideCardCanStackIndicator();
        
        var owningStack = card.owningStack;
        if (owningStack)
        {
            foreach (var stackCard in owningStack.cards)
            {
                stackCard.gameObject.layer = LayerMask.NameToLayer("Card");
            }
        }
    }
    
    private void ShowCardCanStackIndicator(Card card)
    {
        foreach (var c in cardsOnTable)
        {
            if (c == card) continue;
            if (!c.IsLastCard) continue;
            if (c.owningStack == card.owningStack) continue;
            
            var draggingCardData = card.cardData;
            var releasedCardData = c.cardData;

            if (StackingRules.CanStackByType(draggingCardData.cardType, releasedCardData.cardType))
            {
                c.ShowCanStackOnIndicator();
            }
        }
    }
    
    private void HideCardCanStackIndicator()
    {
        foreach (var c in cardsOnTable)
        { 
            c.HideCanStackOnIndicator();
        }
    }
}
