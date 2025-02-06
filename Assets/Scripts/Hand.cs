using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<Card> cardsInHand = new();

    public void AddCardToHand(Card card)
    {
        cardsInHand.Add(card);
    }

    public void RemoveCardFromHand(Card card)
    {
        cardsInHand.Remove(card);
    }

    public int GetCardCount()
    {
        return cardsInHand.Count;
    }

}