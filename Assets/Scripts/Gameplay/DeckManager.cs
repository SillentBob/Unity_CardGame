using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DG.Tweening;
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

    private readonly List<CardModel> _deck = new();

    private void Start()
    {
        _cardDatabase = Game.Instance.CardDatabase;
        _gameSettings = Game.Instance.GameSettings;
        endTurnButton.Button.onClick.AddListener(EndTurn);
        InitializeDeck();
        ShuffleDeck();
        DrawStartingHand().Forget();
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

    private async UniTask DrawStartingHand()
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

        await DrawCardsSequence(newCards);
    }

    private void OnPlayCard(Card c)
    {
        c.OnPlayCard -= OnPlayCard;
        hand.RemoveCardFromHand(c);
        playArea.AddCardToPlayArea(c);
        PlayEffectsOnCardPlayed(c);
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

        DrawCardsSequence(newCards).Forget();
    }

    private void EnableInput(bool value)
    {
        hand.Cards.ForEach(c => c.SetDraggable(value));
        endTurnButton.Button.interactable = value;
    }

    private async UniTask DrawCardsSequence(List<Card> cards)
    {
        cards.ForEach(c => c.GraphicsRoot.transform.SetParent(cardSpawnRoot.transform, false));
        EnableInput(false);
        await UniTask.Delay(TimeSpan.FromSeconds(1)); //wait for ui panels to update
        await AnimateDrawCards(cards);
        cards.ForEach(c =>
        {
            c.GraphicsRoot.transform.SetParent(c.transform, true);
            c.GraphicsRoot.transform.localPosition = Vector3.zero;
        });
        EnableInput(true);
    }

    private async UniTask AnimateDrawCards(List<Card> cards)
    {
        await UniTask.DelayFrame(1);
        var delayAfterEachCard = 1f;
        var durationEachCard = 1f;
        var totalTimeForDraw = cards.Count * delayAfterEachCard + (durationEachCard - delayAfterEachCard);
        for (var i = cards.Count - 1; i >= 0; i--)
        {
            Card c = cards[i];
            var tween = c.GraphicsRoot.transform.DOMove(c.transform.position, durationEachCard).SetEase(Ease.OutSine)
                .SetDelay(i * delayAfterEachCard);
            tween.OnStart(() => c.PlayFlipFromReverseToForeground());
        }

        await UniTask.Delay(TimeSpan.FromSeconds(totalTimeForDraw));
        Debug.Log("Draw animations finished");
    }

    private void PlayEffectsOnCardPlayed(Card c)
    {
        if (particle == null)
        {
            return;
        }

        particle.gameObject.SetActive(true);
        particle.Stop();
        particle.Play();
    }

    private void OnDestroy()
    {
        endTurnButton.Button.onClick.RemoveListener(EndTurn);
    }
}