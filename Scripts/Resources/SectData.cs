using Godot;
using System;

/// <summary>
/// 宗门数据资源。每个宗门通过加法加成和初始配置定义差异化玩法，
/// 最终公式示例：实际攻击力 = 基础攻击力 + AttackPowerBonus
/// </summary>
[GlobalClass]
public partial class SectData : Resource
{
    /// 宗门名称，如"剑宗"、"丹宗"、"散修"
    [Export]
    public string SectName { get; set; } = "";

    /// 初始武器场景路径，角色创建时自动装备，如 "res://Scenes/Weapons/Sword.tscn"
    [Export(PropertyHint.File, "*.tscn")]
    public string InitialWeaponPath { get; set; } = "";

    /// 攻击力加法加成，最终攻击力 = 基础攻击力 + AttackPowerBonus
    [Export]
    public float AttackPowerBonus { get; set; } = 0.0f;

    /// 冷却缩减（加法），最终冷却 = 基础冷却 × (1 - CooldownReduction)；值域建议 0~1
    [Export]
    public float CooldownReduction { get; set; } = 0.0f;

    /// 技能/武器槽位上限（加法），最终槽位 = SlotLimit；散修可设为 8 以获得更多装备空间
    [Export]
    public int SlotLimit { get; set; } = 6;
}
