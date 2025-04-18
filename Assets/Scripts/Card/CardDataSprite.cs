using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDataSprite : MonoBehaviour
{
    [SerializeField] private SpriteRenderer artworkSprite;
    [SerializeField] private TextMeshPro cardNameText;
    [SerializeField] private GameObject canStackOnIndicator;
    
    private Card _owner;
    private readonly Dictionary<SpriteRenderer, int> _spriteInitialSortingOrder = new();
    
    private void Awake()
    {
        _owner = GetComponentInParent<Card>();
        Debug.Assert(_owner != null, $"CardDataSprite {name}가 카드에 붙어있지 않음");

        _owner.OnSortingLayerChanged += SetSortingLayer;
        _owner.OnShowCanStackOnIndicator += () => canStackOnIndicator?.SetActive(true);
        _owner.OnHideCanStackOnIndicator += () => canStackOnIndicator?.SetActive(false);
        
        var sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in sprites)
        {
            _spriteInitialSortingOrder.Add(sprite, sprite.sortingOrder);
        }
    }

    private void Start()
    {
        Debug.Assert(artworkSprite);
        StartCoroutine(SetupCardData());
    }
    
    private IEnumerator SetupCardData()
    {
        yield return new WaitUntil(() => _owner.cardData != null);
        artworkSprite.sprite = _owner.cardData.sprite;
        //cardNameText.text = _owner.cardData.cardName;
    }
    
    private void SetSortingLayer(int sortingOrder, int sortingLayerId = 0)
    {
        foreach (var sprite in _spriteInitialSortingOrder.Keys)
        {
            sprite.sortingLayerID = SortingLayer.layers[sortingLayerId].id;
            sprite.sortingOrder = _spriteInitialSortingOrder[sprite] + sortingOrder;
        }
    }
}
