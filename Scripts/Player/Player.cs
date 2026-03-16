using Godot;
using System;

/// <summary>
/// 玩家主节点。自身不包含任何具体逻辑，
/// 所有行为（移动、战斗、状态等）均通过挂载子组件节点实现，
/// 遵循"组合优于继承"的架构原则。
/// </summary>
public partial class Player : CharacterBody2D
{
}
