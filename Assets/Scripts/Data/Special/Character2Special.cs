using UnityEngine;

public class Character2Special : CharacterBaseSpecialComponent
{
    public override void DoSpecial(GameData context, ulong clientId)
    {
        int actionSelection = (clientId == context.ClientIdPlayer1.Value) ? context.ActionPlayer1.Value : context.ActionPlayer2.Value;
        CharacterMove opponentMove = context.CharacterMoveDatabase.GetMoveById(actionSelection);
        CharacterMove.Type type = opponentMove ? opponentMove.MoveType : CharacterMove.Type.LightAttack; //What to do if the player didn't select?

        context.CombatCommands.Add(new ApplyLockout(clientId, context.RoundNumber.Value, type));
        context.CombatCommands.Add(new ApplyLockout(clientId, context.RoundNumber.Value + 1, type));
        context.CombatCommands.Add(new UndoLockout(clientId, context.RoundNumber.Value + 2, type));
    }


    public class ApplyLockout : CombatCommandBase
    {
        private CharacterMove.Type _moveToLockout;

        public ApplyLockout(ulong clientId, int round, CharacterMove.Type moveToLockout) : base(clientId, round)
        {
            _moveToLockout = moveToLockout;
        }

        public override void Execute(GameData context)
        {
            base.Execute(context);

            if (ClientId == context.ClientIdPlayer1.Value)
            {
                context.UsableMoveListPlayer2.Value &= (byte) ~_moveToLockout;
            }
            else if (ClientId == context.ClientIdPlayer2.Value)
            {
                context.UsableMoveListPlayer1.Value &= (byte)~_moveToLockout;
            }
        }
    }

    public class UndoLockout : CombatCommandBase
    {
        private CharacterMove.Type _moveToLockout;

        public UndoLockout(ulong clientId, int round, CharacterMove.Type moveToLockout) : base(clientId, round)
        {
            _moveToLockout = moveToLockout;
        }

        public override void Execute(GameData context)
        {
            base.Execute(context);

            if (ClientId == context.ClientIdPlayer1.Value)
            {
                context.UsableMoveListPlayer2.Value |= (byte)_moveToLockout;
            }
            else if (ClientId == context.ClientIdPlayer2.Value)
            {
                context.UsableMoveListPlayer1.Value |= (byte)_moveToLockout;
            }
        }
    }
}
