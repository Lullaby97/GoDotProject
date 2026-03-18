using Godot;
using System;

/// <summary>
/// 玩家主节点。自身不包含任何具体逻辑，
/// 所有行为（移动、战斗、状态等）均通过挂载子组件节点实现，
/// 遵循"组合优于继承"的架构原则。
///
/// 持有种族和宗门数据引用，并提供统一的加成计算接口供各组件调用。
/// 监听 HealthComponent 的死亡信号以处理玩家死亡。
/// </summary>
public partial class Player : CharacterBody2D
{
    /// 当前种族数据，在编辑器中拖入对应的 .tres 资源
    [Export]
    public RaceData CurrentRace { get; set; }

    /// 当前宗门数据，在编辑器中拖入对应的 .tres 资源
    [Export]
    public SectData CurrentSect { get; set; }

    private HealthComponent _healthComponent;

    public override void _Ready()
    {
        // 玩家不与敌人产生刚体碰撞（Layer 2），否则会被怪物卡住无法移动
        // 受击判定由 Hurtbox (Area2D) 单独负责
        SetCollisionMaskValue(2, false);

        _healthComponent = GetNodeOrNull<HealthComponent>("HealthComponent");

        if (_healthComponent != null)
        {
            _healthComponent.Died += OnPlayerDied;
        }
        else
        {
            GD.PrintErr("[Player] 未找到 HealthComponent 子节点，玩家将无法死亡。");
        }
    }

    /// <summary>
    /// 计算速度总乘数。采用 GDD "加法叠乘" 公式：
    /// Total = 1 + (Race.SpeedMultiplier - 1) + (Sect速度加成)
    /// 当 SpeedMultiplier 为 1.0 时，该项贡献为 0（无加成）。
    /// SectData 目前没有速度字段，预留为 0。
    /// 若资源为 null 则对应项视为 0，最终回退到 1.0（无加成）。
    /// </summary>
    public float GetTotalSpeedMultiplier()
    {
        float raceBonus = CurrentRace != null ? CurrentRace.SpeedMultiplier - 1.0f : 0.0f;
        float sectBonus = 0.0f;

        return 1.0f + raceBonus + sectBonus;
    }

    /// <summary>
    /// 计算攻击力总乘数。采用 GDD "加法叠乘" 公式：
    /// Total = 1 + (Race攻击加成) + (Sect.AttackPowerBonus)
    /// RaceData 目前没有攻击力字段，预留为 0。
    /// 若资源为 null 则对应项视为 0，最终回退到 1.0（无加成）。
    /// </summary>
    public float GetTotalAttackMultiplier()
    {
        float raceBonus = 0.0f;
        float sectBonus = CurrentSect != null ? CurrentSect.AttackPowerBonus : 0.0f;

        return 1.0f + raceBonus + sectBonus;
    }

    private void OnPlayerDied()
    {
        // 延迟到帧末再重载，避免在物理回调链中销毁正在遍历的节点
        GetTree().CallDeferred("reload_current_scene");
    }
}
