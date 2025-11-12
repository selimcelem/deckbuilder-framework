using UnityEngine;
using DB.Cards;

public class TestPlayCard : MonoBehaviour
{
    public CardPlayer cardPlayer;
    public CardData testCard;

    private void Start()
    {
        cardPlayer.PlayCard(testCard);
    }
}
