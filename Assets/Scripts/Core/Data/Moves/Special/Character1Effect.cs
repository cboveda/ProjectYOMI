using UnityEngine;

public class Character1Effect : CharacterBaseEffect
{
    [SerializeField] private float _healAmountOnSpecial = 15;

    public override void DoSpecial(bool didWinTurn)
    {
        if (!didWinTurn)
        {
            return;
        }
        var myId = _playerCharacter.ClientId;
        var opponentPlayerCharacter = _players.GetByOpponentClientId(myId);
        opponentPlayerCharacter.Position -= 1;
        _playerCharacter.Position += 1;
    }
}
