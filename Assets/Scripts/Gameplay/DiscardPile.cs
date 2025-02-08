using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    private readonly List<Card> _discardedCards = new();

    private void Awake()
    {
        EventManager.AddListener<DiscardCardToPileEvent>(OnAddCardToDiscardPileEvent);
        EventManager.AddListener<DiscardMultipleCardsToPileEvent>(OnAddCardToDiscardPileEvent);
    }

    private void OnAddCardToDiscardPileEvent(DiscardMultipleCardsToPileEvent cards)
    {
        cards.DiscardedCards.ForEach(AddCardToPile);
    }

    private void OnAddCardToDiscardPileEvent(DiscardCardToPileEvent discardCardToPileEvent)
    {
        AddCardToPile(discardCardToPileEvent.DiscardedCard);
    }

    private void AddCardToPile(Card c)
    {
        _discardedCards.Add(c);
        c.transform.SetParent(transform);
        c.gameObject.SetActive(false); //TODO play animation moving to discard pile, then deactivate and move to pool
    }
    
    private void OnDestroy()
    {
        EventManager.RemoveListener<DiscardCardToPileEvent>(OnAddCardToDiscardPileEvent);
        EventManager.RemoveListener<DiscardMultipleCardsToPileEvent>(OnAddCardToDiscardPileEvent);
    }
}
