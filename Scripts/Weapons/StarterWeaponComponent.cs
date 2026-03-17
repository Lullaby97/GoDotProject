using Godot;
using System;

/// <summary>
/// 初始武器发射器组件，挂载为 Player 的子节点。
/// 按固定冷却间隔朝鼠标方向发射弹幕，
/// 子弹实例被添加到场景根节点以避免跟随玩家移动。
/// </summary>
public partial class StarterWeaponComponent : Node2D
{
	/// 子弹预制场景，在编辑器中拖入对应的 .tscn
	[Export]
	public PackedScene ProjectileScene { get; set; }

	/// 攻击冷却时间（秒）
	[Export]
	public float AttackCooldown { get; set; } = 0.5f;

	private float _currentCooldown;

	public override void _Process(double delta)
	{
		_currentCooldown -= (float)delta;

		if (_currentCooldown <= 0.0f)
		{
			Fire();
			_currentCooldown = AttackCooldown;
		}
	}

	private void Fire()
	{
		if (ProjectileScene == null)
		{
			GD.PrintErr("[StarterWeaponComponent] ProjectileScene 未设置，无法发射弹幕。");
			return;
		}

		Projectile projectile = ProjectileScene.Instantiate<Projectile>();

		if (projectile == null)
		{
			GD.PrintErr("[StarterWeaponComponent] 实例化失败：场景根节点不是 Projectile 类型。");
			return;
		}

		// 计算从武器位置指向鼠标的归一化方向
		Vector2 direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();
		projectile.Initialize(GlobalPosition, direction);

		// 将子弹挂到场景根节点，使其不跟随玩家移动
		GetTree().CurrentScene.AddChild(projectile);
	}
}
