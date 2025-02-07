using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Const;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;

    private CardsDatabase _cardDatabase;
    private GameSettings _gameSettings;

    private Hand _hand;
    private PlayArea _playArea;
    private Button _endTurnButton;

    private readonly List<CardModel> _deck = new();

    private void Start()
    {
        _cardDatabase = Game.Instance.CardDatabase;
        _gameSettings = Game.Instance.GameSettings;
        _hand = GameObject.FindGameObjectWithTag(GameObjectTags.PLAYER_HAND).GetComponent<Hand>();
        _playArea = GameObject.FindGameObjectWithTag(GameObjectTags.PLAY_AREA).GetComponent<PlayArea>();
        _endTurnButton = GameObject.FindGameObjectWithTag(GameObjectTags.END_TURN_BUTTON).GetComponent<Button>();
        _endTurnButton.onClick.AddListener(EndTurn);
        InitializeDeck();

#if UNITY_EDITOR
        StringBuilder deckString = new();
        Debug.Log($"Deck before shuffle: {deckString.AppendJoin(',', _deck.Select(model => model.name))}");
#endif

        ShuffleDeck();

#if UNITY_EDITOR
        deckString.Clear();
        Debug.Log($"Deck before shuffle: {deckString.AppendJoin(',', _deck.Select(model => model.name))}");
#endif

        DrawStartingHand();
    }

    private void InitializeDeck()
    {
        var variousCardsCount = _cardDatabase.cardModels.Count;
        for (int i = 0; i < _gameSettings.initialDeckSize; i++)
        {
            _deck.Add(_cardDatabase.cardModels[i % variousCardsCount]);
        }
    }

    private void ShuffleDeck()
    {
        Random rng = new Random();
        int n = _deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (_deck[k], _deck[n]) = (_deck[n], _deck[k]); //swap
        }
    }

    private void DrawStartingHand()
    {
        for (int i = 0; i < _gameSettings.playerHandSize; i++)
        {
            DrawCard();
        }
    }

    private void DrawCard()
    {
        if (_deck.Count > 0)
        {
            CardModel cardData = _deck[0];
            _deck.RemoveAt(0);

            GameObject cardObject = Instantiate(cardPrefab, _hand.transform);
            Card card = cardObject.GetComponent<Card>();
            card.Setup(cardData);
            _hand.AddCardToHand(card);
        }
        else
        {
            Debug.Log("Deck to draw from is empty!");
        }
    }

    private void EndTurn()
    {
        _playArea.RemoveAllCards();
        
        int cardsToDraw = _gameSettings.playerHandSize - _hand.GetCardCount();
        for (int i = 0; i < cardsToDraw; i++)
        {
            DrawCard();
        }
    }

    private void OnDestroy()
    {
        _endTurnButton.onClick.RemoveListener(EndTurn);
    }
}