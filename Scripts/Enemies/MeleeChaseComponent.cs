using Godot;
using System;

/// <summary>
/// 近战追击组件：挂载为敌人的子节点，驱动父节点朝玩家直线移动。
/// 通过 Group 查找玩家引用，每帧计算方向并调用 MoveAndSlide。
/// </summary>
public partial class MeleeChaseComponent : Node
{
    /// 追击移速（像素/秒），默认 150 约为玩家基础速度的一半
    [Export]
    public float MoveSpeed { get; set; } = 150.0f;

    private CharacterBody2D _body;
    private Node2D _target;

    public override void _Ready()
    {
        _body = GetParent() as CharacterBody2D;

        if (_body == null)
        {
            GD.PrintErr("[MeleeChaseComponent] 初始化失败：父节点不是 CharacterBody2D。");
            return;
        }

        _target = GetTree().GetFirstNodeInGroup("Player") as Node2D;

        if (_target == null)
        {
            GD.PrintErr("[MeleeChaseComponent] 警告：场景中未找到 Player Group 的节点，敌人将无法追踪。");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_body == null)
            return;

        if (_target == null || !IsInstanceValid(_target))
            return;

        Vector2 direction = _body.GlobalPosition.DirectionTo(_target.GlobalPosition);

        _body.Velocity = direction * MoveSpeed;
        _body.MoveAndSlide();
    }
}
