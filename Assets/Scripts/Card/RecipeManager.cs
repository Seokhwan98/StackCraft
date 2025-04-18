using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance;
    
    public event Action<Recipe> OnRecipeFinished;
    
    [SerializeField] private Recipe[] recipes;
    [SerializeField] private Recipe woodRecipe;
    [SerializeField] private Recipe berryRecipe;
    [SerializeField] private int woodRecipeNeedForBerry = 4;
    private int _woodRecipeDone = 0;
    
    private Dictionary<CardData, List<Recipe>> _recipesByInput;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        
        foreach (var recipe in recipes)
        {
            recipe.EnsureCacheGenerated();
        }

        BuildRecipeIndex();
    }

    private void BuildRecipeIndex()
    {
        _recipesByInput = new Dictionary<CardData, List<Recipe>>();
        foreach (var recipe in recipes)
        {
            foreach (var cardData in recipe.GetInputCards().Keys)
            {
                if (!_recipesByInput.ContainsKey(cardData))
                {
                    _recipesByInput[cardData] = new List<Recipe>();
                }

                if (!_recipesByInput[cardData].Contains(recipe))
                {
                    _recipesByInput[cardData].Add(recipe);
                }
            }
        }
    }

    private void Start()
    {
        foreach (var stack in GameTableManager.Instance.stacksOnTable)
        {
            stack.OnStackModified += OnStackModified;
        }
        
        GameTableManager.Instance.StackAddedOnTable += OnStackAddedOnTable;
        GameTableManager.Instance.StackRemovedFromTable += OnStackRemovedFromTable;
    }
    
    private void OnStackAddedOnTable(Stack stack)
    {
        stack.OnStackModified += OnStackModified;
    }
    
    private void OnStackRemovedFromTable(Stack stack)
    {
        stack.OnStackModified -= OnStackModified;
    }
    
    private void OnStackModified(Stack stack)
    {
        if (stack.HasTimer)
        {
            if (!CheckRecipe(stack, stack.producingRecipe))
            {
                stack.RemoveTimer();
            }
        }
        
        // Don't bother checking for recipes if the stack is empty or has only one card
        if (stack.Length <= 1) return;
        
        Debug.Log($"Stack modified: {stack.name}. Checking for recipes...");
        if (TryFindMatchingRecipe(stack, out var matchedRecipe, out var consumedCards))
        {
            Debug.Log($"Recipe matched: {matchedRecipe.name} with {stack.name}, consumed cards: {string.Join(", ", consumedCards.Select(c => c.name))}");
            
            // OnRecipeStarted?.Invoke(matchedRecipe);
            if (matchedRecipe.produceTime > 0)
            {
                stack.AddTimer(matchedRecipe, consumedCards);
            }
            else
            {
                ApplyRecipe(stack, matchedRecipe, consumedCards);
            }
        }
    }

    public void ApplyRecipe(Stack stack, Recipe recipe, List<Card> consumedCards)
    {
        var originStackPos = stack.cards[0].transform.position;
        
        if (recipe.consumeInputs)
        {
            stack.ConsumeCards(consumedCards);
        }

        var outputCards = recipe.outputCards;
            
        foreach (var spawningCard in outputCards)
        {
            var randomDirection = Random.insideUnitCircle.normalized;
            var randomUnitCircle = randomDirection * 3f;
            
            var spawningPos = originStackPos + new Vector3(randomUnitCircle.x, randomUnitCircle.y, 0);
            GameTableManager.Instance.AddNewCardToTable(spawningCard, spawningPos);
        }
        

        if (recipe == woodRecipe)
        {
            _woodRecipeDone++;
            if (_woodRecipeDone >= woodRecipeNeedForBerry)
            {
                _woodRecipeDone = 0;
                ApplyRecipe(stack, berryRecipe, new List<Card>());
            }
        }
        OnRecipeFinished?.Invoke(recipe);
    }

    public bool CheckRecipe(Stack stack, Recipe recipe)
    {
        if (stack.Length < recipe.TotalInputCount)
        {
            return false;
        }

        var stackCardCounts = stack.CardCounts;
            
        var inputCards = recipe.GetInputCards();
        bool canMatch = true;
            
        foreach (var requirement in inputCards)
        {
            if (!stackCardCounts.TryGetValue(requirement.Key, out var count))
            {
                canMatch = false;
                break;
            }
            if (count < requirement.Value)
            {
                canMatch = false;
                break;
            }
        }

        return canMatch;
    }

    public bool TryFindMatchingRecipe(Stack stack, out Recipe matchedRecipe, out List<Card> consumedCards)
    {
        matchedRecipe = null;
        consumedCards = new List<Card>();

        var stackCardCounts = stack.CardCounts;

        var potentialRecipes = new HashSet<Recipe>();
        foreach (var cardData in stackCardCounts.Keys)
        {
            if (_recipesByInput.TryGetValue(cardData, out var inputRecipes))
            {
                potentialRecipes.UnionWith(inputRecipes);
            }

            if (potentialRecipes.Count == 0) return false;
        }
        
        foreach (var recipe in potentialRecipes)
        {
            if (stack.Length < recipe.TotalInputCount)
            {
                continue;
            }
            
            var inputCards = recipe.GetInputCards();
            //Debug.Log($"Checking recipe {recipe.recipeName} with input cards: {string.Join(", ", inputCards)}");
            bool canMatch = true;
            
            foreach (var requirement in inputCards)
            {
                if (!stackCardCounts.TryGetValue(requirement.Key, out var count))
                {
                    canMatch = false;
                    break;
                }
                if (count < requirement.Value)
                {
                    canMatch = false;
                    break;
                }
            }

            if (canMatch)
            {
                matchedRecipe = recipe;
                consumedCards = FindConsumedCards(stack.cards, inputCards);

                if (consumedCards != null && consumedCards.Count == recipe.TotalInputCount)
                {
                    return true;
                }
                else
                {
                    Debug.LogError("incorrect number of consumed cards");
                }
            }
        }

        return false;
    }

    private List<Card> FindConsumedCards(List<Card> cards, Dictionary<CardData, int> inputCards)
    {
        List<Card> consumedCards = new List<Card>();
        // Copy the dictionary first
        Dictionary<CardData, int> needed = new Dictionary<CardData, int>(inputCards);

        foreach (var card in cards)
        {
            if (needed.ContainsKey(card.cardData))
            {
                consumedCards.Add(card);
                needed[card.cardData]--;
                
                if (needed[card.cardData] <= 0)
                {
                    needed.Remove(card.cardData);
                }
            }
            
            if (needed.Count == 0)
            {
                break;
            }
        }
        
        if (needed.Count > 0)
        {
            Debug.LogError("Not all input cards were consumed");
            return null;
        }
        
        return consumedCards;
    }
}
