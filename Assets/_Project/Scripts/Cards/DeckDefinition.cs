using System.Collections.Generic;
using UnityEngine;

namespace DB.Cards
{
    [CreateAssetMenu(menuName = "DB/Deck Definition")]
    public class DeckDefinition : ScriptableObject
    {
        public string deckName = "Starter Deck";
        public List<CardData> cards = new();
    }
}