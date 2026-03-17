using Godot;
using System;

/// <summary>
/// 敌人主节点。作为实体容器持有 CharacterBody2D 的物理能力，
/// 所有行为（追踪、攻击等）通过子组件挂载实现。
/// 负责监听 HealthComponent 的死亡信号并执行销毁。
/// </summary>
public partial class Enemy : CharacterBody2D
{
    private HealthComponent _healthComponent;

    public override void _Ready()
    {
        _healthComponent = GetNode<HealthComponent>("HealthComponent");

        if (_healthComponent != null)
        {
            _healthComponent.Died += OnDied;
        }
        else
        {
            GD.PrintErr("[Enemy] 未找到 HealthComponent 子节点，敌人将无法被击杀。");
        }
    }

    public HealthComponent GetHealthComponent()
    {
        return _healthComponent;
    }

    private void OnDied()
    {
        QueueFree();
    }
}
