using System;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Serializable]
    public enum KnockbackDirection
    {
        Left = -1,
        Right = 1
    }

    [SerializeField] private float _knockIncrement;
    private float _initialX;
    private KnockbackDirection _direction = KnockbackDirection.Left;
    private PlayerCharacter _player;
    private Transform _transform;
    public float InitialX { get => _initialX; set => _initialX = value; }
    public KnockbackDirection Direction { get => _direction; set => _direction = value; }
    public float KnockIncrement { get => _knockIncrement; set => _knockIncrement = value; }

    void Awake()
    {
        _transform = transform;
        _initialX = transform.position.x;
        _player = GetComponent<PlayerCharacter>();
    }

    public void UpdatePosition()
    {
        var currentPosition = _transform.position;
        float newX = _initialX - (_player.Position * _knockIncrement * (float)_direction);
        var newPosition = new Vector3
        {
            x = newX,
            y = currentPosition.y,
            z = currentPosition.z
        };
        _transform.position = newPosition;
    }
}
