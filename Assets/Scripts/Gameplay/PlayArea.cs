using System.Collections.Generic;
using UnityEngine;

public class PlayArea : MonoBehaviour
{
    public List<Card> CardsInPlayArea => _cardsInPlayArea;
    private readonly List<Card> _cardsInPlayArea = new();

    public void AddCardToPlayArea(Card  c)
    {
        _cardsInPlayArea.Add(c);
        c.transform.SetParent(this.transform, false);
    }
    
    public void RemoveAllCards()
    {
        _cardsInPlayArea.Clear();
    }
}