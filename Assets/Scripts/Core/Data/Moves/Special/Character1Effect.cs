using UnityEngine;
using Zenject;

public class Character1Effect : CharacterBaseEffect
{
    [SerializeField] private float _healAmountOnSpecial = 15;

    public override void DoSpecial()
    {
        _playerCharacter.Health += _healAmountOnSpecial;
    }
}
