using Godot;
using System;

/// <summary>
/// 敌人主节点。作为实体容器持有 CharacterBody2D 的物理能力，
/// 所有行为（追踪、攻击等）通过子组件挂载实现。
/// 负责监听 HealthComponent 的死亡信号并执行掉落 + 销毁。
/// </summary>
public partial class Enemy : CharacterBody2D
{
    /// 死亡时掉落的经验球场景，在编辑器中拖入 exp_gem.tscn
    [Export]
    public PackedScene ExpGemScene { get; set; }

    private HealthComponent _healthComponent;

    public override void _Ready()
    {
        // 敌人不与玩家产生刚体碰撞（Layer 1），自由穿过玩家
        // 伤害判定由玩家的 Hurtbox (Area2D) 负责
        SetCollisionMaskValue(1, false);

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
        SpawnExpGem();
        QueueFree();
    }

    private void SpawnExpGem()
    {
        if (ExpGemScene == null)
            return;

        Node2D gem = ExpGemScene.Instantiate<Node2D>();

        if (gem == null)
        {
            GD.PrintErr("[Enemy] 经验球场景实例化失败。");
            return;
        }

        // 先设坐标，再用 CallDeferred 延迟入树。
        // OnDied 处于信号回调链（Projectile.BodyEntered → TakeDamage → Died），
        // 此时 PhysicsServer 正在结算碰撞，直接 AddChild 会引发 "flushing queries" 错误。
        gem.Position = GlobalPosition;
        GetTree().CurrentScene.CallDeferred("add_child", gem);
    }
}
