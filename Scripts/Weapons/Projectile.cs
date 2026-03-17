using Godot;
using System;

/// <summary>
/// 通用弹幕基类，挂载在子弹场景的 Area2D 根节点上。
/// 子弹沿自身 Transform.X（局部右方）方向飞行，
/// 离开屏幕后自动销毁以回收资源。
/// </summary>
public partial class Projectile : Area2D
{
	/// 飞行速度（像素/秒）
	[Export]
	public float Speed { get; set; } = 600.0f;

	/// 单次命中伤害
	[Export]
	public float Damage { get; set; } = 10.0f;

	private VisibleOnScreenNotifier2D _notifier;

	public override void _Ready()
	{
		_notifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");

		if (_notifier != null)
		{
			_notifier.ScreenExited += OnScreenExited;
		}
		else
		{
			GD.PrintErr("[Projectile] 未找到 VisibleOnScreenNotifier2D 子节点，子弹将无法自动销毁。");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// 沿局部 X 轴正方向（即子弹朝向）匀速前进
		GlobalPosition += Transform.X * Speed * (float)delta;
	}

	/// <summary>
	/// 由发射器调用，设置子弹的初始位置与飞行朝向。
	/// direction 会被归一化后转为旋转角度，使 Transform.X 对齐飞行方向。
	/// </summary>
	public void Initialize(Vector2 startPos, Vector2 direction)
	{
		GlobalPosition = startPos;
		Rotation = direction.Angle();
	}

	private void OnScreenExited()
	{
		QueueFree();
	}
}
