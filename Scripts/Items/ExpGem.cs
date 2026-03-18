using Godot;
using System;

/// <summary>
/// 经验球掉落物，挂载在 exp_gem.tscn 的 Area2D 根节点上。
/// 状态机：Idle → Magnetized → 拾取销毁。
/// 通过 BodyEntered 检测玩家 CharacterBody2D 进入后磁吸追踪并拾取。
/// </summary>
public partial class ExpGem : Area2D
{
    private enum State { Idle, Magnetized }

    [Export]
    public float MagnetSpeed { get; set; } = 500.0f;

    [Export]
    public float ExpValue { get; set; } = 10.0f;

    private State _currentState = State.Idle;
    private Node2D _targetPlayer;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_currentState != State.Magnetized)
            return;

        if (_targetPlayer == null || !IsInstanceValid(_targetPlayer))
            return;

        GlobalPosition = GlobalPosition.MoveToward(
            _targetPlayer.GlobalPosition,
            MagnetSpeed * (float)delta
        );

        if (GlobalPosition.DistanceTo(_targetPlayer.GlobalPosition) < 10.0f)
        {
            Pickup();
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (_currentState != State.Idle)
            return;

        if (!body.IsInGroup("Player"))
            return;

        _targetPlayer = body;
        _currentState = State.Magnetized;
    }

    private void Pickup()
    {
        SetDeferred("monitoring", false);

        if (_targetPlayer != null && IsInstanceValid(_targetPlayer))
        {
            ExperienceComponent expComp = _targetPlayer.GetNodeOrNull<ExperienceComponent>("ExperienceComponent");
            expComp?.AddExperience(ExpValue);
        }

        QueueFree();
    }
}
