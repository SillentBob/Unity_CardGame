using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Gameplay;
using UnityEngine;
using Random = System.Random;

public class DeckManager : MonoBehaviour
{
    [Header("Mandatory")] [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject cardSpawnRoot;
    [SerializeField] private GameObject dragAnchor;
    [SerializeField] private Hand hand;
    [SerializeField] private PlayArea playArea;
    [SerializeField] private DiscardPile discardPile;
    [SerializeField] private EndTurnButton endTurnButton;
    [Header("Optional")] [SerializeField] private ParticleSystem particle;

    private CardsDatabase _cardDatabase;
    private GameSettings _gameSettings;

    private readonly DeckManagerAnimator _animations = new();
    private readonly List<CardModel> _deck = new();

    private void Start()
    {
        _cardDatabase = Game.Instance.CardDatabase;
        _gameSettings = Game.Instance.GameSettings;
        endTurnButton.Button.onClick.AddListener(EndTurn);
        InitializeDeck();
        ShuffleDeck();
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
        List<Card> newCards = new();
        for (int i = 0; i < _gameSettings.playerHandSize; i++)
        {
            var drawnCard = TryDrawCard();
            if (drawnCard != null)
            {
                newCards.Add(drawnCard);
            }
        }

        _animations.DrawCardsSequence(newCards, cardSpawnRoot.transform, () => EnableInput(false),
            () => EnableInput(true)).Forget();
    }

    private void OnPlayCard(Card c)
    {
        c.OnPlayCard -= OnPlayCard;
        hand.RemoveCardFromHand(c);
        playArea.AddCardToPlayArea(c);
        _animations.PlayEffectsOnCardPlayed(particle);
    }

    private void OnSwapCardPlaces(Card draggedCard, int draggedCardIdx, Card stationaryCard, int stationaryCardIdx)
    {
        hand.ReorderCard(draggedCard, draggedCardIdx, stationaryCardIdx);
        hand.ReorderCard(stationaryCard, stationaryCardIdx, draggedCardIdx);
    }


    private Card TryDrawCard()
    {
        Card card = null;
        if (_deck.Count > 0)
        {
            CardModel cardData = _deck[0];
            _deck.RemoveAt(0);
            GameObject cardObject = Instantiate(cardPrefab, hand.transform);
            card = cardObject.GetComponent<Card>();
            card.Setup(cardData, dragAnchor);
            card.OnPlayCard += OnPlayCard;
            card.OnSwapCardPlaces += OnSwapCardPlaces;
            hand.AddCardToHand(card);
        }
        else
        {
            Debug.Log("Deck to draw from is empty!");
        }

        return card;
    }

    private void EndTurn()
    {
        discardPile.AddCardToPile(playArea.CardsInPlayArea);
        playArea.RemoveAllCards();
        int cardsToDraw = _gameSettings.playerHandSize - hand.GetCardCount();
        List<Card> newCards = new();
        for (int i = 0; i < cardsToDraw; i++)
        {
            newCards.Add(TryDrawCard());
        }

        _animations.DrawCardsSequence(newCards, cardSpawnRoot.transform, () => EnableInput(false),
            () => EnableInput(true)).Forget();
    }

    private void EnableInput(bool value)
    {
        hand.Cards.ForEach(c => c.SetDraggable(value));
        endTurnButton.Button.interactable = value;
    }


    private void OnDestroy()
    {
        endTurnButton.Button.onClick.RemoveListener(EndTurn);
    }
}