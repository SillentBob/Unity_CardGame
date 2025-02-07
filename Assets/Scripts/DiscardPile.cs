using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    private readonly List<Card> _discardedCards = new();

    public void AddCardToDiscardPile(Card card)
    {
        _discardedCards.Add(card);
        card.transform.SetParent(transform);
        card.gameObject.SetActive(false); //TODO play animation moving to discard pile, then deactivate and move to pool
    }

}
