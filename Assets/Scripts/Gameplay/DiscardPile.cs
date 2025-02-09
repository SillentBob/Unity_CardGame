using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    public List<Card> DiscardedCards => _discardedCards;
    private readonly List<Card> _discardedCards = new();

    public void AddCardToPile(List<Card> cards)
    {
        cards.ForEach(AddCardToPile);
    }

    private void AddCardToPile(Card c)
    {
        _discardedCards.Add(c);
        c.transform.SetParent(transform);
    }

    public void DeactivateCardsOnPile()
    {
        _discardedCards.ForEach(c=>c.gameObject.SetActive(false)); //TODO return to pool
        _discardedCards.Clear();
    }
}