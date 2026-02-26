using System.Collections.Generic;
using UnityEngine;

namespace DB.Cards
{
    public class DeckManager : MonoBehaviour
    {
        [Header("Deck Source")]
        public DeckDefinition startingDeck;

        [Header("Runtime Piles (Debug)")]
        [SerializeField] private List<CardData> drawPile = new();
        [SerializeField] private List<CardData> hand = new();
        [SerializeField] private List<CardData> discardPile = new();

        public IReadOnlyList<CardData> Hand => hand;

        public void InitFromStartingDeck()
        {
            drawPile.Clear();
            hand.Clear();
            discardPile.Clear();

            if (startingDeck == null)
            {
                Debug.LogError("DeckManager: No startingDeck assigned.");
                return;
            }

            drawPile.AddRange(startingDeck.cards);
            Shuffle(drawPile);
        }

        public void Draw(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (drawPile.Count == 0)
                {
                    ReshuffleDiscardIntoDraw();
                    if (drawPile.Count == 0) return; // nothing to draw
                }

                CardData top = drawPile[^1];
                drawPile.RemoveAt(drawPile.Count - 1);
                hand.Add(top);
            }

            Debug.Log($"Drew {amount}. Hand={hand.Count} Draw={drawPile.Count} Discard={discardPile.Count}");
        }

        public void DiscardHand()
        {
            discardPile.AddRange(hand);
            hand.Clear();
        }

        public void DiscardCardFromHandAt(int handIndex)
        {
            if (handIndex < 0 || handIndex >= hand.Count) return;

            CardData card = hand[handIndex];
            hand.RemoveAt(handIndex);
            discardPile.Add(card);
        }

        private void ReshuffleDiscardIntoDraw()
        {
            if (discardPile.Count == 0) return;

            drawPile.AddRange(discardPile);
            discardPile.Clear();
            Shuffle(drawPile);
            Debug.Log("Reshuffled discard into draw pile.");
        }

        private void Shuffle(List<CardData> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int j = Random.Range(i, list.Count);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}