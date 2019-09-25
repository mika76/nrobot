#region Copyright and License
///////////////////////////////////////////////////////////////////////////////
// NRobot - Autonomous robot fighting game
// Copyright (c) 2004,2005 Stuart Ballard
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// The GNU General Public License should be located in the file COPYING.
//
// For more information about NRobot, please contact nrobot-list@gna.org or
// write to Stuart Ballard at NetReach Inc, 124 S Maple Street, Ambler,
// PA  19002  USA.
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Security;
using System.Security.Policy;
using System.Security.Permissions;
using NRobot.Robot;

namespace NRobot.Engine 
{

	/// <summary>Lives inside the Arena domain and supervises the actual running
	/// of the game.</summary>
	public class GameArena : MarshalByRefObject
	{

		private static bool alreadyCreated = false;
		private static readonly bool inArenaDomain = AppDomain.CurrentDomain.FriendlyName == "NRobotArena";

		public GameArena() 
		{
			if (inArenaDomain) 
			{
				if (alreadyCreated) throw new ApplicationException("Cannot create more than one GameArena object per arena");
				alreadyCreated = true;
			} 
			else 
			{
				throw new ApplicationException("GameArena objects can only be created in Arena domain");
			}
		}

		internal GameState state;
		internal GameRules rules;

		/// <summary>Starts the game.</summary>
		// This freezes all the rules so they can no longer be modified and
		// allocates resources, starting positions, etc ready for the game to
		// start. The game will not progress all by itself; instead, you need to
		// call Tick() repeatedly to make things actually happen.
		[PermissionSet (SecurityAction.Assert, Unrestricted = true)]
		internal GameState Start(GameState state, GameRules rules) 
		{

			this.state = state;
			this.rules = rules;
			state.arena = this;

			state.robots = new ArrayList();
			state.bullets = new ArrayList();
			state.started = true;

			try 
			{

				foreach (Team team in state.teams) 
				{
					team.robots = new ArrayList();
					team.bullets = new ArrayList();

					ArrayList robotNames = new ArrayList();
					ConstructorInfo robotCtor;
					TeamInfoAttribute teamInfo = new TeamInfoAttribute();
					try 
					{
						Assembly asm = Assembly.LoadFrom(team.dllPath);
						Type robotType = null;
						foreach (object attr in asm.GetCustomAttributes(typeof(RobotClassAttribute), false)) 
						{
							RobotClassAttribute rca = attr as RobotClassAttribute;
							if (rca != null) 
							{
								robotType = asm.GetType(rca.ClassName);
								if (robotType == null) throw new ApplicationException("Cannot find robot class " + rca.ClassName);
							}
						}
						if (robotType == null) throw new ApplicationException("Assembly does not have a RobotClass attribute");
						foreach (object attr in robotType.GetCustomAttributes(false)) 
						{
							OwnerEmailAttribute oa = attr as OwnerEmailAttribute;
							if (oa != null) 
							{
								team.ownerEmail = oa.OwnerEmail;
							}
							TeamInfoAttribute tia = attr as TeamInfoAttribute;
							if (tia != null) 
							{
								teamInfo = tia;
								team.name = tia.Name;
								team.bodyColor = defColor(tia.BodyColor, -1); // default random
								team.turretColor = defColor(tia.TurretColor, -1); // default random
								team.gunColor = defColor(tia.GunColor, team.turretColor);
								team.wheelColor = defColor(tia.WheelColor, team.turretColor);
								team.cameraColor = defColor(tia.CameraColor, 0xffffff);
								team.lensColor = defColor(tia.LensColor, 0x000000);
								team.bulletColor = defColor(tia.BulletColor, team.bodyColor);
							}
							RobotInfoAttribute ria = attr as RobotInfoAttribute;
							if (ria != null) 
							{
								robotNames.Add(ria);
							}
						}
						if (team.ownerEmail == null) throw new ApplicationException("Class " + robotType.FullName + " does not have an Owner attribute");
						if (team.name == null) team.name = team.ownerEmail;
						robotNames.Sort();
						robotCtor = robotType.GetConstructor(new Type[] {});
						if (robotCtor == null) throw new ApplicationException("Class " + robotType.FullName + " does not have a public no-arg constructor");
					} 
					catch (Exception e) 
					{
						callSay(null, "Could not load " + (team.Name == null ? team.dllPath : team.Name) + ": " + e.Message);
						team.aliveBots = new ArrayList();
						continue;
					}

					for (int i = 0; i < rules.TeamSize; i++) 
					{
						IRobot iRobot;
						try 
						{
							iRobot = (IRobot) robotCtor.Invoke(new object[] {});
						} 
						catch 
						{
							callSay(null, "Robot on " + team.Name + " team failed to construct.");
							continue;
						}
						RobotInfoAttribute ria = (RobotInfoAttribute) robotNames[i % robotNames.Count];

						int netgain = 0;
						int radius = tradeoffResult(ria.Size, teamInfo.Size, rules.RobotRadius, 90, 110, ref netgain);

						Robot robot = new Robot(team, paddedRand(rules.ArenaWidth, radius),
							paddedRand(rules.ArenaHeight, radius), random.Next(NRMath.FullCircle));
						robot.iRobot = iRobot;

						robot.name = ria.Name;
						robot.bodyColor = defColor(ria.BodyColor, team.bodyColor);
						robot.turretColor = defColor(ria.TurretColor, team.turretColor);
						robot.gunColor = defColor(ria.GunColor, team.gunColor);
						robot.wheelColor = defColor(ria.WheelColor, team.wheelColor);
						robot.cameraColor = defColor(ria.CameraColor, team.cameraColor);
						robot.lensColor = defColor(ria.LensColor, team.lensColor);
						robot.bulletColor = defColor(ria.BulletColor, team.bulletColor);

						// Tradeoffs
						robot.bulletDamage = tradeoffResult(ria.BulletDamage, teamInfo.BulletDamage, rules.BulletDamage, 125, 80, ref netgain);
						robot.shotDelay = tradeoffResult(ria.ShotDelay, teamInfo.ShotDelay, rules.ShotDelay, 80, 125, ref netgain);
						robot.bulletSpeed = tradeoffResult(ria.BulletSpeed, teamInfo.BulletSpeed, rules.BulletSpeed, 125, 80, ref netgain);
						robot.shotsPermitted = tradeoffResult(ria.ShotsPermitted, teamInfo.ShotsPermitted, rules.BotShotsPermitted, 125, 80, ref netgain);
						robot.maxMoveSpeed = tradeoffResult(ria.MaxMoveSpeed, teamInfo.MaxMoveSpeed, rules.MaxMoveSpeed, 125, 80, ref netgain);
						robot.maxTurnSpeed = tradeoffResult(ria.MaxTurnSpeed, teamInfo.MaxTurnSpeed, rules.MaxTurnSpeed, 125, 80, ref netgain);
						robot.maxTurretTurnSpeed = tradeoffResult(ria.MaxTurretTurnSpeed, teamInfo.MaxTurretTurnSpeed, rules.MaxTurretTurnSpeed, 125, 80, ref netgain);
						robot.radius = radius;

						int tries = 10;
						bool collide = true;
						while (collide && tries > 0) 
						{
							collide = false;
							foreach (Robot other in state.robots) 
							{
								if (sq(robot.x - other.x) + sq(robot.y - other.y) <
									sq(robot.Radius + other.Radius)) 
								{
									collide = true;
									break;
								}
							}
							if (collide) 
							{
								tries--;
								if (tries == 0) throw new ApplicationException("Cannot place all robots on this size arena without collisions");
								robot.x = paddedRand(rules.ArenaWidth, robot.Radius);
								robot.y = paddedRand(rules.ArenaHeight, robot.Radius);
							}
						}

						if (i >= robotNames.Count ||
							rules.TeamSize > robotNames.Count + i) 
						{
							robot.name += " " + (i / robotNames.Count + 1);
						}
						if (netgain == 0) 
						{
							team.robots.Add(robot);
							state.robots.Add(robot);
						} 
						else 
						{
							robot.health = 0;
							callSay(robot, "Illegal tradeoffs - net gain of " + netgain);
						}
					}
					team.aliveBots = new ArrayList(team.robots);
				}
				state.aliveBots = new ArrayList(state.robots);

				foreach (Robot robot in state.robots) 
				{
					StartState startState = new StartState(robot);
					startState.isActive = true;
					try 
					{
						robot.iRobot.Start(startState);
					} 
					catch (Exception e)
					{
						robot.health = 0;
						callSay(robot, "Suicide due to exception: " + e);
						state.aliveBots.Remove(robot);
						robot.Team.aliveBots.Remove(robot);
					}
					startState.isActive = false;
				}
			} 
			catch 
			{
				state.started = false;
				throw;
			}
			return state;
		}
		Random random = new Random();
		private int paddedRand(int bound, int padding) 
		{
			return random.Next(padding, bound - padding);
		}
		private int tradeoffResult(TradeOff robotTradeOff, TradeOff teamTradeOff, int startValue, int betterPct, int worsePct, ref int netgain) 
		{
			TradeOff tradeOff = robotTradeOff;
			if (tradeOff == TradeOff.Unset) tradeOff = teamTradeOff;

			switch (tradeOff) 
			{
				case TradeOff.Better:
					netgain++;
					return tradeOffValue(startValue, betterPct);
				case TradeOff.Worse:
					netgain--;
					return tradeOffValue(startValue, worsePct);
				case TradeOff.Unset:
				case TradeOff.Neutral:
					return startValue;
				default:
					throw new ArgumentException("Invalid tradeOff value " + tradeOff);
			}
		}
		private int tradeOffValue(int startValue, int pct) 
		{
			// Add 1 to get rounding rather than floor behavior
			int result = (startValue * pct * 2 + 1) / 200;
			if (result == startValue && pct != 100) 
			{
				result = startValue + (pct > 100 ? 1 : -1);
			}
			return result;
		}

		/// <summary>Perform one tick's worth of game progress.</summary>
		// This method should be called from a timer or a loop to cause the game
		// to go forward. Each robot's controlling class will have its Tick()
		// method called, then the game will move each robot the appropriate
		// distance in the appropriate direction, calculate damage, update bullet
		// locations etc.
		internal GameState Tick() 
		{
			state.sayMessages.Clear();

			foreach (Robot robot in state.aliveBots) 
			{
				robot.prevHealth = robot.health;
				robot.impactsThisTick.Clear();
			}

			foreach (Bullet bullet in new ArrayList(state.bullets)) 
			{
				bullet.x += bullet.Robot.BulletSpeed * NRMath.Sin(bullet.direction);
				bullet.y += bullet.Robot.BulletSpeed * NRMath.Cos(bullet.direction);
				if (bullet.x < 0 || bullet.x > rules.ArenaWidth ||
					bullet.y < 0 || bullet.y > rules.ArenaHeight) 
				{
					bullet.Team.bullets.Remove(bullet);
					bullet.Robot.bullets.Remove(bullet);
					state.bullets.Remove(bullet);
				} 
				else 
				{
					foreach (Robot robot in new ArrayList(state.aliveBots)) 
					{
						if (sq(bullet.x - robot.x) + sq(bullet.y - robot.y) < sq(robot.Radius)) 
						{
							// How to figure out bullet damage:
							// Construct a unit vector perpendicular to the bullet's direction of
							// movement.
							// Dot-product this with the distance from the bullet to the robot.
							// RobotRadius minus that value / * bulletdamage / robotradius + 1
							// capped at bulletdamage
							// is the appropriate amount of damage.
							decimal dot = (bullet.x - robot.x) * NRMath.Cos(bullet.direction) +
								(robot.y - bullet.y) * NRMath.Sin(bullet.direction);
							decimal damage = ((robot.Radius - Math.Abs(dot)) * bullet.Robot.BulletDamage / robot.Radius) + 0.5m;
							int intDamage = (int) damage;
							if (intDamage > bullet.Robot.BulletDamage) intDamage = bullet.Robot.BulletDamage;
							else if (intDamage < 1) intDamage = 1;
							robot.health -= intDamage;
							robot.impactsThisTick.Add(bullet);
							if (robot.health <= 0) 
							{
								robot.health = 0;
								state.aliveBots.Remove(robot);
								robot.Team.aliveBots.Remove(robot);
							}
							bullet.Team.bullets.Remove(bullet);
							bullet.Robot.bullets.Remove(bullet);
							state.bullets.Remove(bullet);
						}
					}
				}
			}

			ArrayList tickStates = new ArrayList();
			foreach (Robot robot in new ArrayList(state.aliveBots)) 
			{
				TickState tickState = new TickState(robot);
				tickState.isActive = true;
				try 
				{
					robot.iRobot.Tick(tickState);
				} 
				catch (Exception e) 
				{
					robot.health = 0;
					callSay(robot, "Suicide due to exception: " + e);
					state.aliveBots.Remove(robot);
					robot.Team.aliveBots.Remove(robot);
				}
				tickState.isActive = false;
				tickStates.Add(tickState);
			}

			foreach (TickState tickState in tickStates) 
			{
				Robot robot = tickState.robot;
				robot.moveSpeed = tickState.moveSpeed;
				robot.moveDuration = tickState.moveDuration;
				robot.turnSpeed = tickState.turnSpeed;
				robot.turnDuration = tickState.turnDuration;
				robot.gunTurnSpeed = tickState.gunTurnSpeed;
				robot.gunTurnDuration = tickState.gunTurnDuration;
				robot.cameraTurnSpeed = tickState.cameraTurnSpeed;
				robot.cameraTurnDuration = tickState.cameraTurnDuration;
				if (robot.turnDuration > 0) 
				{
					robot.direction = NRMath.Angle(robot.direction + robot.turnSpeed);
					robot.turnDuration--;
					if (robot.turnDuration == 0) robot.turnSpeed = 0;
				}
				if (robot.gunTurnDuration > 0) 
				{
					robot.gunAngle = NRMath.AngOff(robot.gunAngle + robot.gunTurnSpeed);
					robot.gunTurnDuration--;
					if (robot.gunTurnDuration == 0) robot.gunTurnSpeed = 0;
				}
				if (robot.cameraTurnDuration > 0) 
				{
					robot.cameraAngle = NRMath.AngOff(robot.cameraAngle + robot.cameraTurnSpeed);
					robot.cameraTurnDuration--;
					if (robot.cameraTurnDuration == 0) robot.cameraTurnSpeed = 0;
				}

				tickState.newX = robot.x;
				tickState.newY = robot.y;
				robot.actualDistanceMoved = 0;
				if (robot.moveDuration > 0) 
				{

					// Implement movement
					tickState.newX += robot.moveSpeed * NRMath.Sin(robot.direction);
					tickState.newY += robot.moveSpeed * NRMath.Cos(robot.direction);
					robot.actualDistanceMoved = robot.moveSpeed;
					robot.moveDuration--;
					if (robot.moveDuration == 0) robot.moveSpeed = 0;

					// Detect wall collisions
					if (tickState.newX < robot.Radius ||
						tickState.newY < robot.Radius ||
						tickState.newX > rules.ArenaWidth - robot.Radius ||
						tickState.newY > rules.ArenaHeight - robot.Radius) 
					{
						tickState.newX = robot.x;
						tickState.newY = robot.y;
						robot.actualDistanceMoved = 0;
					}
				}
			}

			// Detect inter-bot collisions
			bool foundCollision = true;
			while (foundCollision) 
			{
				foundCollision = false;

				foreach (TickState state1 in tickStates) 
				{
					Robot robot1 = state1.robot;
					decimal xmv1 = state1.newX - robot1.x;
					decimal ymv1 = state1.newY - robot1.y;
					bool moved1 = (xmv1 != 0m || ymv1 != 0m);

					foreach (TickState state2 in tickStates) 
					{
						if (state1 == state2) continue;

						Robot robot2 = state2.robot;
						decimal xmv2 = state2.newX - robot2.x;
						decimal ymv2 = state2.newY - robot2.y;
						bool moved2 = (xmv2 != 0m || ymv2 != 0m);

						if (!moved1 && !moved2) continue;

						decimal xDist = state1.newX - state2.newX;
						decimal yDist = state1.newY - state2.newY;
						if (sq(xDist) + sq(yDist) < sq(robot1.Radius + robot2.Radius)) 
						{
							decimal dot1 = !moved1 ? 0m : xDist * xmv1 + yDist * ymv1;
							decimal dot2 = !moved2 ? 0m : xDist * xmv2 + yDist * ymv2;
							if (dot1 < 0m) 
							{
								state1.newX = robot1.x;
								state1.newY = robot1.y;
								robot1.actualDistanceMoved = 0;
								foundCollision = true;
							}
							if (dot2 > 0m) 
							{
								state2.newX = robot2.x;
								state2.newY = robot2.y;
								robot2.actualDistanceMoved = 0;
								foundCollision = true;
							}
						}
					}
				}
			}

			// Update bot positions and implement firing
			foreach (TickState tickState in tickStates) 
			{
				Robot robot = tickState.robot;
				if (robot.currentShotDelay > 0) robot.currentShotDelay--;
				robot.x = tickState.newX;
				robot.y = tickState.newY;
				if (tickState.fired && robot.bullets.Count < robot.ShotsPermitted &&
					robot.Team.bullets.Count < rules.TeamShotsPermitted &&
					robot.currentShotDelay <= 0) 
				{
					robot.currentShotDelay = robot.ShotDelay;
					Bullet bullet = new Bullet(robot, rules);
					robot.Team.bullets.Add(bullet);
					robot.bullets.Add(bullet);
					state.bullets.Add(bullet);
				}
			}

			int aliveTeams = 0;
			Team maybeWinner = null;
			foreach (Team team in state.teams) 
			{
				if (team.AliveBots.Count > 0) 
				{
					aliveTeams++;
					maybeWinner = team;
				}
			}
			if (aliveTeams <= 1) 
			{
				state.over = true;
				state.winner = maybeWinner;
			}
			return state;
		}

		private decimal sq(decimal n) 
		{
			return n * n;
		}

		private void loadTeam(Team team) 
		{
		}

		internal void callSay(Robot robot, string message) 
		{
			state.sayMessages.Add(new SayMessage(robot, message));
		}

		private static NRColor defColor(int color, NRColor def) 
		{
			return color == int.MinValue ? def : new NRColor(color);
		}
		private static NRColor defColor(int color, int def) 
		{
			return new NRColor(color == int.MinValue ? def : color);
		}
	}
}