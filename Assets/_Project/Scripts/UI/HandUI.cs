using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DB.Battle;
using DB.Cards;

namespace DB.UI
{
    public class HandUI : MonoBehaviour
    {
        [Header("References")]
        public BattleController battle;
        public DeckManager deck;

        [Header("UI")]
        public Transform handPanel;
        public Button endTurnButton;
        public TMP_Text manaText;

        [Header("Prefabs")]
        public GameObject cardPrefab; // CardView prefab root

        [Header("Layout")]
        public HandFanLayout handFanLayout; // drag the HandPanel's HandFanLayout here

        private readonly List<GameObject> spawned = new();

        private void Start()
        {
            if (endTurnButton != null && battle != null)
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

            RefreshHand();

            if (deck != null)
                Debug.Log("RefreshHand running. Hand count = " + deck.Hand.Count);
        }

        private void RefreshHand()
        {
            if (deck == null || handPanel == null || cardPrefab == null || battle == null)
                return;

            // Clear old cards
            foreach (var go in spawned)
                if (go != null) Destroy(go);

            spawned.Clear();

            var hand = deck.Hand;

            for (int i = 0; i < hand.Count; i++)
            {
                int index = i;
                CardData card = hand[i];

                // Spawn CardView root
                GameObject go = Instantiate(cardPrefab, handPanel);
                spawned.Add(go);

                RectTransform cardRoot = go.transform as RectTransform;

                // Find the Button (HitArea) inside prefab
                Button btn = go.GetComponentInChildren<Button>(true);
                if (btn == null)
                {
                    Debug.LogError("Card prefab is missing a Button (expected on HitArea).");
                    continue;
                }

                // Update visible text (adapt if you have specific fields)
                TMP_Text tmp = go.GetComponentInChildren<TMP_Text>(true);
                if (tmp != null)
                    tmp.text = $"{card.cardName} ({card.cost})";

                // Click
                btn.onClick.AddListener(() =>
                {
                    Debug.Log("CARD CLICKED: " + card.cardName);
                    battle.PlayCardFromHand(index);
                });

                // Hover (HitArea drives hover, but we lift the root)
                if (handFanLayout != null && cardRoot != null)
                {
                    var hover = btn.gameObject.GetComponent<CardHover>();
                    if (!hover) hover = btn.gameObject.AddComponent<CardHover>();
                }
            }

            // Optional: force a layout pass immediately
            if (handFanLayout != null)
                handFanLayout.Layout();
        }

        public void ForceRefresh()
        {
            RefreshAll();
        }
    }
}