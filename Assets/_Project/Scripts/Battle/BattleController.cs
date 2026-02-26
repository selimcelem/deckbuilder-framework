using UnityEngine;
using DB.Core;
using DB.Cards;
using DB.UI;

namespace DB.Battle
{
    public class BattleController : MonoBehaviour
    {
        [Header("Scene References")]
        public TurnManager turnManager;
        public DeckManager deckManager;
        public CardPlayer cardPlayer;
        public ManaSystem manaSystem;
        public HandUI ui;
        public BlockChargeManager playerBlockCharges;
        public BlockMinigameController blockMinigame;
        public GameObject handRoot; // drag your HandPanel or the parent UI for hand

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

            // refresh AFTER state changes
            if (ui != null) ui.ForceRefresh();
        }

        public void EndPlayerTurn()
        {
            deckManager.DiscardHand();

            turnManager.EndPlayerTurn(); // enemy acts

            StartPlayerTurn(); // this will refresh UI
        }

        public void ResolveEnemyAttack(int damage)
        {
            if (playerBlockCharges != null && playerBlockCharges.Charges > 0 && blockMinigame != null)
            {
                if (handRoot) handRoot.SetActive(false);

                blockMinigame.StartMinigame(outcome =>
                {
                    if (handRoot) handRoot.SetActive(true);

                    switch (outcome)
                    {
                        case BlockOutcome.Miss:
                            turnManager.player.TakeDamage(damage);
                            break;

                        case BlockOutcome.Block:
                            // no damage
                            break;

                        case BlockOutcome.Perfect:
                            // no damage + reflect 50%
                            turnManager.enemy.TakeDamage(Mathf.CeilToInt(damage * 0.5f));
                            break;
                    }

                    // decay AFTER enemy turn, regardless of outcome
                    playerBlockCharges.DecayAfterEnemyTurn();
                });

                return;
            }

            // no charges -> normal hit
            turnManager.player.TakeDamage(damage);

            // still decay if you had charges? (no, because you didn't)
        }

        // Temporary: play by index from hand
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
            deckManager.DiscardCardFromHandAt(handIndex);

            // refresh AFTER card removed from hand
            if (ui != null) ui.ForceRefresh();
        }
    }
}