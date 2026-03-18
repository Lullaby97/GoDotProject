using Godot;
using System;

/// <summary>
/// 玩家受击判定组件，挂载在 Player 子节点 Hurtbox (Area2D) 上。
/// 持续检测重叠的敌人 Body，受击后进入无敌帧并播放闪烁反馈。
///
/// 【设计决策】完全不使用 Timer 节点，改用 double 倒计时变量在 _PhysicsProcess 中递减。
/// 出生保护复用同一套无敌帧系统，Monitoring 始终保持 true，杜绝 API 调用报错。
/// </summary>
public partial class PlayerHurtboxComponent : Area2D
{
    /// 无敌帧持续时间（秒）
    [Export]
    public float InvincibilityDuration { get; set; } = 0.5f;

    /// 每次受击扣除的固定伤害值
    [Export]
    public float DamageTakenPerHit { get; set; } = 10.0f;

    /// 出生保护时间（秒），复用无敌帧系统实现
    [Export]
    public float SpawnProtectionTime { get; set; } = 0.5f;

    private Player _player;
    private HealthComponent _healthComponent;
    private Sprite2D _sprite;
    private double _invincibilityRemaining;

    public override void _Ready()
    {
        _player = GetParent() as Player;

        if (_player == null)
        {
            GD.PrintErr($"[PlayerHurtboxComponent @ {GetPath()}] 初始化失败：父节点不是 Player。");
            return;
        }

        _healthComponent = _player.GetNodeOrNull<HealthComponent>("HealthComponent");

        if (_healthComponent == null)
        {
            GD.PrintErr($"[PlayerHurtboxComponent @ {GetPath()}] 未在 Player 下找到 HealthComponent 子节点。");
        }

        _sprite = _player.GetNodeOrNull<Sprite2D>("Sprite2D");

        // 出生保护：只激活无敌倒计时，不播放闪烁（没有实际受击不需要视觉反馈）
        _invincibilityRemaining = SpawnProtectionTime;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsInsideTree())
            return;

        if (_invincibilityRemaining > 0.0)
        {
            _invincibilityRemaining -= delta;
            return;
        }

        if (_healthComponent == null)
            return;

        foreach (Node2D body in GetOverlappingBodies())
        {
            if (body is Enemy)
            {
                ApplyHit();
                return;
            }
        }
    }

    private void ApplyHit()
    {
        if (_invincibilityRemaining > 0.0)
            return;

        _healthComponent.TakeDamage(DamageTakenPerHit);
        TriggerInvincibility(InvincibilityDuration);
    }

    /// <summary>
    /// 统一的无敌帧触发入口，出生保护和受击无敌共用同一套倒计时 + 闪烁逻辑。
    /// </summary>
    private void TriggerInvincibility(float duration)
    {
        _invincibilityRemaining = duration;
        PlayFlashEffect();
    }

    private async void PlayFlashEffect()
    {
        if (_sprite == null || !IsInsideTree())
            return;

        for (int i = 0; i < 3; i++)
        {
            if (!IsInsideTree())
                return;

            Tween tween = CreateTween();
            tween.TweenProperty(_sprite, "modulate:a", 0.3f, 0.08f);
            tween.TweenProperty(_sprite, "modulate:a", 1.0f, 0.08f);
            await ToSignal(tween, Tween.SignalName.Finished);
        }

        if (_sprite != null && IsInstanceValid(_sprite))
        {
            _sprite.Modulate = new Color(_sprite.Modulate, 1.0f);
        }
    }
}
