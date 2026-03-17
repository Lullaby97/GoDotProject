using Godot;
using System;

/// <summary>
/// 生命值组件：可挂载到任何需要血量的实体上。
/// 通过信号 Died 通知宿主节点死亡，实现解耦的生命管理。
/// </summary>
public partial class HealthComponent : Node
{
    [Signal]
    public delegate void DiedEventHandler();

    /// 最大生命值
    [Export]
    public float MaxHealth { get; set; } = 50.0f;

    /// 当前生命值，运行时由 _Ready 初始化
    public float CurrentHealth { get; private set; }

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// 扣除生命值。血量归零时发射 Died 信号，
    /// 由宿主节点自行决定死亡表现（销毁、播放动画等）。
    /// </summary>
    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;

        if (CurrentHealth <= 0.0f)
        {
            CurrentHealth = 0.0f;
            EmitSignal(SignalName.Died);
        }
    }
}
