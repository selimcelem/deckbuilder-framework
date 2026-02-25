using UnityEngine;
using DB.Core;
using DB.Cards;

namespace DB.Battle
{
    public class BattleController : MonoBehaviour
    {
        [Header("Scene References")]
        public TurnManager turnManager;
        public DeckManager deckManager;
        public CardPlayer cardPlayer;
        public ManaSystem manaSystem;

        [Header("Rules")]
        public int cardsPerTurn = 5;

        private void Start()
        {
            if (!turnManager || !deckManager || !cardPlayer || !manaSystem)
            {
                Debug.LogError("BattleController: Missing references in Inspector.");
                return;
            }

            StartBattle();
        }

        public void StartBattle()
        {
            deckManager.InitFromStartingDeck();
            StartPlayerTurn();
        }

        public void StartPlayerTurn()
        {
            manaSystem.ResetMana();
            deckManager.Draw(cardsPerTurn);
        }

        public void EndPlayerTurn()
        {
            deckManager.DiscardHand();
            turnManager.EndPlayerTurn(); // enemy acts inside TurnManager for now
            StartPlayerTurn();
        }

        // Temporary: play by index from hand (replace with UI clicks soon)
        public void PlayCardFromHand(int handIndex)
        {
            var hand = deckManager.Hand;
            if (handIndex < 0 || handIndex >= hand.Count) return;

            CardData card = hand[handIndex];

            if (!manaSystem.CanAfford(card.cost))
            {
                Debug.Log($"Not enough mana to play {card.cardName} (cost {card.cost})");
                return;
            }

            manaSystem.Spend(card.cost);
            cardPlayer.PlayCard(card);
            deckManager.DiscardCardFromHand(card);
        }
    }
}