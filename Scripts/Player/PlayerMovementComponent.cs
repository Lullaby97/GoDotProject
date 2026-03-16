using Godot;
using System;

/// <summary>
/// 移动组件：作为 Player 的子节点挂载，负责处理玩家的移动逻辑。
/// 将移动逻辑抽离到独立组件中，使 Player 主节点保持干净，
/// 后续可按需添加冲刺、闪避等组件而无需修改 Player 本身。
/// </summary>
public partial class PlayerMovementComponent : Node
{
    [Export]
    public float BaseMoveSpeed { get; set; } = 300.0f;

    private CharacterBody2D _player;

    public override void _Ready()
    {
        _player = GetParent() as CharacterBody2D;

        if (_player == null)
        {
            GD.PrintErr("[PlayerMovementComponent] 初始化失败：父节点不是 CharacterBody2D，请将本组件挂载为 Player 节点的子节点。");
            return;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_player == null)
            return;

        // 获取归一化的输入方向向量（WASD / 摇杆）
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        _player.Velocity = inputDir * BaseMoveSpeed;
        _player.MoveAndSlide();
    }
}
