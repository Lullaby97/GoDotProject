using Godot;
using System;

/// <summary>
/// 经验值组件，挂载为 Player 的子节点。
/// 管理经验获取、种族加成计算和升级逻辑。
/// </summary>
public partial class ExperienceComponent : Node
{
	[Signal]
	public delegate void LeveledUpEventHandler(int newLevel);

	[Signal]
	public delegate void ExperienceChangedEventHandler(float currentExp, float maxExp);

	/// 每级所需经验的增长系数
	[Export]
	public float LevelUpGrowthRate { get; set; } = 1.2f;

	/// 首级所需的基础经验值
	[Export]
	public float BaseMaxExp { get; set; } = 100.0f;

	public int CurrentLevel { get; private set; } = 1;
	public float CurrentExp { get; private set; }
	public float MaxExp { get; private set; }

	private Player _player;

	public override void _Ready()
	{
		_player = GetParent() as Player;

		if (_player == null)
		{
			GD.PrintErr("[ExperienceComponent] 初始化失败：父节点不是 Player。");
		}

		MaxExp = BaseMaxExp;
	}

	/// <summary>
	/// 增加经验值。实际获得量受种族 ExperienceBonus 加成：
	/// finalExp = amount × (1 + RaceData.ExperienceBonus)
	/// 人族 ExperienceBonus 为正值时获得更多经验（等效于减少升级消耗）。
	/// RaceData 为 null 时加成为 0，不影响基础经验。
	/// </summary>
	public void AddExperience(float amount)
	{
		float bonus = 0.0f;

		if (_player != null && _player.CurrentRace != null)
		{
			bonus = _player.CurrentRace.ExperienceBonus;
		}

		float finalExp = amount * (1.0f + bonus);
		CurrentExp += finalExp;

		EmitSignal(SignalName.ExperienceChanged, CurrentExp, MaxExp);

		while (CurrentExp >= MaxExp)
		{
			CurrentExp -= MaxExp;
			CurrentLevel++;
			MaxExp *= LevelUpGrowthRate;

			GD.Print($"[ExperienceComponent] 升级！当前等级: {CurrentLevel}，下级所需: {MaxExp:F0}");
			EmitSignal(SignalName.LeveledUp, CurrentLevel);
			EmitSignal(SignalName.ExperienceChanged, CurrentExp, MaxExp);
		}
	}
}
