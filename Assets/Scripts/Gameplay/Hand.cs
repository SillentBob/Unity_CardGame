using System.Collections.Generic;
using MethodExtensions;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<Card> Cards => _cardsInHand;
    private List<Card> _cardsInHand = new();

    public void AddCardToHand(Card card)
    {
        _cardsInHand.Add(card);
    }

    public void RemoveCardFromHand(Card card)
    {
        _cardsInHand.Remove(card);
    }

    public int GetCardCount()
    {
        return _cardsInHand.Count;
    }

    public void ReorderCard(Card card, int oldGOIndex, int newGOIndex)
    {
        var currentIdx = _cardsInHand.IndexOf(card);
        _cardsInHand.SafeSwap(currentIdx, newGOIndex);
        card.transform.SetSiblingIndex(newGOIndex);
    }
}