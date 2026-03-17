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

    private Player _player;

    public override void _Ready()
    {
        _player = GetParent() as Player;

        if (_player == null)
        {
            GD.PrintErr("[PlayerMovementComponent] 初始化失败：父节点不是 Player，请将本组件挂载为 Player 节点的子节点。");
            return;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_player == null)
            return;

        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        // 最终速度 = 基础移速 × 速度总乘数（种族 + 宗门加法叠乘）
        float finalSpeed = BaseMoveSpeed * _player.GetTotalSpeedMultiplier();

        _player.Velocity = inputDir * finalSpeed;
        _player.MoveAndSlide();
    }
}
