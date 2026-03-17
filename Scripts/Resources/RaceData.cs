using Godot;
using System;

/// <summary>
/// 种族数据资源。每个种族通过乘法倍率和加法加成定义差异化属性，
/// 最终公式示例：实际生命值 = 基础生命值 × HealthMultiplier
/// </summary>
[GlobalClass]
public partial class RaceData : Resource
{
    /// 种族名称，如"人族"、"妖族"、"灵族"
    [Export]
    public string RaceName { get; set; } = "";

    /// 生命值乘法倍率，最终生命 = 基础生命 × HealthMultiplier
    [Export]
    public float HealthMultiplier { get; set; } = 1.0f;

    /// 移速乘法倍率，最终移速 = 基础移速 × SpeedMultiplier
    [Export]
    public float SpeedMultiplier { get; set; } = 1.0f;

    /// 经验加成（加法），升级所需经验 = 基础经验 × (1 - ExperienceBonus)；人族可设为正值以减少升级消耗
    [Export]
    public float ExperienceBonus { get; set; } = 0.0f;

    /// 闪避概率（加法），受击闪避率 = 基础闪避率 + DodgeChance；灵族天赋闪避
    [Export]
    public float DodgeChance { get; set; } = 0.0f;

    /// 种族的多行文本描述，用于 UI 展示
    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; } = "";
}
