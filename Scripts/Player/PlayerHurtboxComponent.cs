using Godot;
using System;

/// <summary>
/// 玩家受击判定组件，挂载在 Player 子节点 Hurtbox (Area2D) 上。
/// 持续检测重叠的敌人 Body，受击后进入无敌帧并播放闪烁反馈。
/// </summary>
public partial class PlayerHurtboxComponent : Area2D
{
    /// 无敌帧持续时间（秒）
    [Export]
    public float InvincibilityDuration { get; set; } = 0.5f;

    /// 每次受击扣除的固定伤害值
    [Export]
    public float DamageTakenPerHit { get; set; } = 10.0f;

    private Player _player;
    private HealthComponent _healthComponent;
    private Sprite2D _sprite;
    private Timer _invincibilityTimer;
    private bool _isInvincible;

    public override void _Ready()
    {
        _player = GetParent() as Player;

        if (_player == null)
        {
            GD.PrintErr("[PlayerHurtboxComponent] 初始化失败：父节点不是 Player。");
            return;
        }

        _healthComponent = _player.GetNodeOrNull<HealthComponent>("HealthComponent");

        if (_healthComponent == null)
        {
            GD.PrintErr("[PlayerHurtboxComponent] 未在 Player 下找到 HealthComponent 子节点。");
        }

        _sprite = _player.GetNodeOrNull<Sprite2D>("Sprite2D");

        // 动态创建无敌帧计时器
        _invincibilityTimer = new Timer();
        _invincibilityTimer.OneShot = true;
        _invincibilityTimer.Timeout += OnInvincibilityEnd;
        AddChild(_invincibilityTimer);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isInvincible || _healthComponent == null)
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
        _healthComponent.TakeDamage(DamageTakenPerHit);

        _isInvincible = true;
        _invincibilityTimer.Start(InvincibilityDuration);

        PlayFlashEffect();
    }

    private async void PlayFlashEffect()
    {
        if (_sprite == null)
            return;

        // 闪烁 3 次：透明度 1.0 → 0.3 → 1.0，每次 ~0.15 秒
        for (int i = 0; i < 3; i++)
        {
            Tween tween = CreateTween();
            tween.TweenProperty(_sprite, "modulate:a", 0.3f, 0.08f);
            tween.TweenProperty(_sprite, "modulate:a", 1.0f, 0.08f);
            await ToSignal(tween, Tween.SignalName.Finished);
        }

        // 确保最终完全不透明
        _sprite.Modulate = new Color(_sprite.Modulate, 1.0f);
    }

    private void OnInvincibilityEnd()
    {
        _isInvincible = false;
    }
}
