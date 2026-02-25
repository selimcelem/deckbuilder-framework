using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DB.Battle;
using DB.Cards;
using DB.Core;

namespace DB.UI
{
    public class HandUI : MonoBehaviour
    {
        [Header("References")]
        public BattleController battle;
        public DeckManager deck;
        public ManaSystem mana;

        [Header("UI")]
        public Transform handPanel;
        public Button endTurnButton;
        public TMP_Text manaText;

        [Header("Prefabs")]
        public Button cardButtonPrefab;

        private readonly List<Button> spawned = new();

        private void Start()
        {
            if (endTurnButton != null)
                endTurnButton.onClick.AddListener(() => battle.EndPlayerTurn());

            StartCoroutine(DelayedInit());
        }

        private System.Collections.IEnumerator DelayedInit()
        {
            yield return null;
            RefreshAll();
        }

        private void RefreshAll()
        {
            if (manaText != null && mana != null)
                manaText.text = $"Mana: {mana.currentMana}/{mana.maxMana}";

            RefreshHand();
            
            if (deck != null)
                Debug.Log("RefreshHand running. Hand count = " + deck.Hand.Count);
        }

        private void RefreshHand()
        {
            if (deck == null || handPanel == null || cardButtonPrefab == null || battle == null)
                return;

            // Clear old buttons
            foreach (var b in spawned)
                if (b != null) Destroy(b.gameObject);

            spawned.Clear();

            var hand = deck.Hand;

            for (int i = 0; i < hand.Count; i++)
            {
                int index = i;
                CardData card = hand[i];

                Button btn = Instantiate(cardButtonPrefab, handPanel);
                Debug.Log("Spawned button for: " + card.cardName);
                var tmp = btn.GetComponentInChildren<TMP_Text>();
                if (tmp != null)
                    tmp.text = $"{card.cardName} ({card.cost})";

                btn.onClick.AddListener(() =>
                {
                    Debug.Log("CARD CLICKED: " + card.cardName);
                    battle.PlayCardFromHand(index);
                });
                Debug.Log("Listener added for index " + index);
                spawned.Add(btn);
            }
        }

        public void ForceRefresh()
        {
            RefreshAll();
        }
    }
}