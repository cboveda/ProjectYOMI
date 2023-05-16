using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewMoveInteractionDictionary", menuName ="Moves/Move Interaction Dictionary")]
public class MoveInteractionDictionary : SerializableDictionary<Move.Type, List<Move.Type>> { }
