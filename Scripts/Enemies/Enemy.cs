using Godot;
using System;

/// <summary>
/// 敌人主节点。作为实体容器持有 CharacterBody2D 的物理能力，
/// 所有行为（追踪、攻击等）通过子组件挂载实现。
/// </summary>
public partial class Enemy : CharacterBody2D
{
}
