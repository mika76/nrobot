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
using NRobot.Robot;

// Appdomain todo:
// * Finish going through all the methods in Game and deferring them appropriately
// * Make sure that the other types in the namespace are truly serializable
// * Add some protection to *guarantee* that Team inside arena will not load a DLL
// * Sandbox the domain
// - Replace TeamShotsPermitted with something like "team shot adjustment", which is
//   the *difference* between Sum(robot.BotShotsPermitted) and the team value.
//   This can be constant between teams without affecting the fact that each
//   team and each robot on a team might have a different ShotsPermitted tradeoff.
// - Make sure robots are aware of their own maximums and the size of other robots,
//   without the Rules class.
// - Game is IDisposable, Dispose() unloads arena domain. Tick() also unloads
//   arena domain if state.over. Quite possibly arena domain only gets created on
//   Start().
// - Implement and require robot classes to call state.Verify(this) before anything
//   else works. Verify should test to ensure that the argument is actually equal
//   to the IRobot it's supposed to be. If Verify is never called a flag doesn't get
//   set, and the robot should suicide; if Verify is called with the wrong arg then
//   a different flag gets set and regardless of whether it's called correctly before
//   or after, the robot should still suicide. Make sure all classes in the NRobot
//   assembly are sealed, even though the lack of public ctors should be enough...
// - Write guidelines for robot authors to be hackproof:
//   - No public fields or methods except for the necessary ones:
//     - No-arg ctor
//     - Start
//     - Tick
//     All other methods and all fields period should be "internal" or "private".
//   - Make all your classes except for the robot class "internal".
//   - Make your robot class "sealed"
//   - Do not do anything in the ctor - preferably just leave the implicit one the
//     compiler puts in for you. If you need initialization, do it in Start.
//   - Make state.Verify(this) the very first thing you do in both Start and Tick.
//     Do *not* enclose this in a try-catch or try-finally block - if Verify throws an
//     exception it indicates a hack attempt and you *want* to abort immediately.
//   - Note that these guidelines are actually overkill and it's possible to be secure
//     with only a subset of them. But isn't it better to be paranoid? Also note that
//     these suggestions will eventually become unnecessary altogether once NRobot's
//     security model is enhanced to deal with inter-robot attacks in a more
//     sophisticated way. But I'll *still* recommend following them, because again,
//     it's better to be paranoid :)
// - NRobot should explicitly verify at least that:
//   - No public fields or methods except for the necessary ones
//   - No public classes except *either* the IRobot or the IMakeRobots
//   - Robot class is sealed
// - Change OwnerAttribute to explicitly refer to email address. Make the
//   sample bots owned by the nrobot mailing list.
// - Add an IMakeRobots interface with the following API:
//     public String OwnerEmail {get;}
//     public TeamInfoAttribute TeamInfo {get;}
//     public RobotInfoAttribute GetRobotInfo();
//     public IRobot CreateABot(RobotInfoAttribute ria);
//     These will be called in the order:
//     TeamInfo get
//     foreach robot:
//       GetRobotInfo
//       CreateABot
//   This allows two things:
//   - More control over robot creation process; ability to be more dynamic,
//     vary more stuff randomly, change things based on rules, etc.
//   - Ability to be used in languages without Attribute support, such as IKVM'd java.
//     This works as follows:
//     - If an assembly references NRobot.dll, first look for a RobotClassAttribute.
//       If it's found, grab that type and check whether it implements IMakeRobots.
//       if it does, make one and use it; otherwise, use our own private implementation
//       of IMakeRobots which looks at attributes to do its job.
//       If no RCA is found, scan the assembly for a public class that *does* implement
//       IMakeRobots. If one is found, use it; if not, skip the assembly with a warning.
//       If the class *doesn't* reference NRobot.dll, skip it with no warning.

namespace NRobot.Engine 
{

	/// <summary>Represents a game, with a particular set of rules.</summary>
	// This class provides the main entry point used by a GUI to create and
	// manage a new game. It also provides the bulk of the implementation of
	// the game itself.
	public class Game : MarshalByRefObject
	{
		internal GameState state = new GameState();
		internal GameRules rules = new GameRules();

		private AppDomain arenaDomain;
		private GameArena arena;

		private static readonly bool inArenaDomain = AppDomain.CurrentDomain.FriendlyName == "NRobotArena";

		internal Game() 
		{
			state.game = this;
		}

		/// <summary>Create a new game, with the rules randomized.</summary>
		// This is the only means of obtaining a Game object, and hence,
		// the only way to get access to any of the functionality of this
		// namespace.
		public static Game Create() 
		{
			// TODO: pick a random seed and call Create(seed)
			return new Game();
		}

		/// <summary>Create a new game, with the rules randomized based on a specified seed value.</summary>
		public static Game Create(int randomSeed) 
		{
			// TODO: use the random seed and initialize all parameters randomly;
			// store the seed for future use.
			return new Game();
		}

		private int argInt(string arg) 
		{
			return int.Parse(arg.Substring(arg.IndexOf(":") + 1));
		}
		private void printUsage() 
		{
			if (!state.over) 
			{
				callSay(null, "Valid parameters: [options] [robot dll files or dirs]");				callSay(null, "Options are:");				callSay(null, "  -insecure             - Disable NRobot's security sandbox and allow");				callSay(null, "                          robot code full access to the system.");				callSay(null, "  -teamsize:n           - Each team will consist of n robots. Default is 1.");				callSay(null, "  -starthealth:n        - Robots will start with n health points. Default is 100.");				callSay(null, "  -robotradius:n        - Robots will be n units in radius. Default is 1000.");				callSay(null, "  -bulletspeed:n        - Bullets will travel n units per tick. Default is 500.");				callSay(null, "  -bulletdamage:n       - A direct hit will do n health points of damage. Default is 5.");				callSay(null, "  -shotdelay:n          - There will be a delay of n ticks between shots. Default is 5.");				callSay(null, "  -arenawidth:n         - The arena will be n units wide. Default is 30000.");				callSay(null, "  -arenaheight:n        - The arena will be n units tall. Default is 20000.");				callSay(null, "  -maxmovespeed:n       - Robots can move at n units per tick. Default is 100.");				callSay(null, "  -maxturnspeed:n       - Robots can turn n thousandths of a full circle per tick. Default is 10.");				callSay(null, "  -maxturretturnspeed:n - Robots can turn their turrets n thousandths per tick. Default is 15.");				callSay(null, "  -shotspermitted:n     - Robots can have n bullets in flight at once. Default is 5.");				state.over = true;			}		}

		/// <summary>
		/// Set up game parameters in bulk, based on the command-line arguments.
		/// </summary>
		/// <param name="args">The command line arguments as passed to Main()</param>
		public void InitFromCommandLineArgs(string[] args) 
		{
			try 
			{
				foreach (string rawarg in args) 
				{					string arg = rawarg.StartsWith("-") ? "/" + rawarg.Substring(1) : rawarg;					string larg = arg.ToLower();					if (larg == "/?" || larg == "/h" || larg == "/help") 
					{						printUsage();					}					if (larg == "/insecure") 
					{						this.Sandboxed = false;					} 
					else if (larg.StartsWith("/teamsize:")) 
					{
						this.TeamSize = argInt(larg);
					} 
					else if (larg.StartsWith("/starthealth:")) 
					{
						this.StartHealth = argInt(larg);
					}
					else if (larg.StartsWith("/robotradius:")) 
					{
						this.RobotRadius = argInt(larg);
					}
					else if (larg.StartsWith("/bulletspeed:")) 
					{
						this.BulletSpeed = argInt(larg);
					}
					else if (larg.StartsWith("/bulletdamage:")) 
					{
						this.BulletDamage = argInt(larg);
					}
					else if (larg.StartsWith("/shotdelay:")) 
					{
						this.ShotDelay = argInt(larg);
					}
					else if (larg.StartsWith("/arenawidth:")) 
					{
						this.ArenaWidth = argInt(larg);
					}
					else if (larg.StartsWith("/arenaheight:")) 
					{
						this.ArenaHeight = argInt(larg);
					}
					else if (larg.StartsWith("/maxmovespeed:")) 
					{
						this.MaxMoveSpeed = argInt(larg);
					}
					else if (larg.StartsWith("/maxturnspeed:")) 
					{
						this.MaxTurnSpeed = argInt(larg);
					}
					else if (larg.StartsWith("/maxturretturnspeed:")) 
					{
						this.MaxTurretTurnSpeed = argInt(larg);
					}
					else if (larg.StartsWith("/shotspermitted:")) 
					{
						this.BotShotsPermitted = argInt(larg);
					}
					else 
					{						this.AddTeams(arg);					}				}			} 
			catch 
			{				printUsage();			}			if (state.teams.Count < 2) 
			{				printUsage();			}		}

		/// <summary>Load a DLL and add a new team based on that DLL.</summary>
		// You cannot load the same DLL twice, or even copies of it.
		// This cannot be called once the game has started.
		public Team AddTeam(string dllPath) 
		{
			if (state.started) throw new ApplicationException("Cannot add teams after the game has started!");
			Team team = new Team(state, dllPath);
			state.teams.Add(team);
			return team;
		}

		/// <summary>Load one or more DLLs based on a file or folder name.</summary>
		public void AddTeams(string dllPath) 
		{
			if (state.started) throw new ApplicationException("Cannot add teams after the game has started!");
			if (Directory.Exists(dllPath)) 
			{
				foreach (string name in Directory.GetFiles(dllPath, "*.dll")) 
				{
					AddTeam(name);
				}
			} 
			else if (File.Exists(dllPath)) 
			{
				AddTeam(dllPath);
			} 
			else 
			{
				throw new ApplicationException("Path " + dllPath + " not found.");
			}
		}

		/// <summary>Remove a team from the game.</summary>
		// This cannot be called once the game has started.
		public void RemoveTeam(Team team) 
		{
			if (state.started) throw new ApplicationException("Cannot remove teams after the game has started!");
			state.teams.Remove(team);
		}

		/// <summary>The random seed used to randomize the rules of this game.</summary>
		// TODO: Store the random seed passed to Create()
		public int RandomSeed {get {return 0;}}


		/// <summary>The number of robots per team.</summary>
		// Read-only once the game has started.
		public int TeamSize 
		{
			get {return rules.TeamSize;}
			set 
			{
				if (state.started) throw new ApplicationException("Cannot set game parameters after game has started");
				rules.TeamSize = value;
			}
		}

		private bool sandboxed = true;
		public bool Sandboxed 
		{
			get {return sandboxed;}
			set 
			{
				if (state.started) throw new ApplicationException("Cannot set game parameters after game has started");
				sandboxed = value;
			}
		}

		/// <summary>The initial health of each robot.</summary>
		// Read-only once the game has started.
		public int StartHealth 
		{
			get {return rules.StartHealth;}
			set 
			{
				if (state.started) throw new ApplicationException("Cannot set game parameters after game has started");
				rules.StartHealth = value;
			}
		}

		/// <summary>The radius of each robot.</summary>
		// Read-only once the game has started.
		public int RobotRadius 
		{
			get {return rules.RobotRadius;}
			set 
			{
				if (state.started) throw new ApplicationException("Cannot set game parameters after game has started");
				rules.RobotRadius = value;
			}
		}

		/// <summary>The speed bullets fire at, in units per tick.</summary>
		// Read-only once the game has started.
		public int BulletSpeed 
		{
			get {return rules.BulletSpeed;}
			set 
			{
				if (state.started) throw new ApplicationException("Cannot set game parameters after game has started");
				rules.BulletSpeed = value;
			}
		}

		/// <summary>The amount of health subtracted for a direct hit.</summary>
		// Read-only once the game has started.
		public int BulletDamage 
		{
			get {return rules.BulletDamage;}
			set 
			{
				if (state.started) throw new ApplicationException("Cannot set game parameters after game has started");
				rules.BulletDamage = value;
			}
		}

		/// <summary>The minimum number of ticks between shots fired.</summary>
		// Read-only once the game has started.
		public int ShotDelay 
		{
			get {return rules.ShotDelay;}
			set 
			{
				if (state.started) throw new ApplicationException("Cannot set game parameters after game has started");
				rules.ShotDelay = value;
			}
		}

		/// <summary>The width of the arena.</summary>
		// Read-only once the game has started.
		public int ArenaWidth 
		{
			get {return rules.ArenaWidth;}
			set 
			{
				if (state.started) throw new ApplicationException("Cannot set game parameters after game has started");
				rules.ArenaWidth = value;
			}
		}

		/// <summary>The height of the arena.</summary>
		// Read-only once the game has started.
		public int ArenaHeight 
		{
			get {return rules.ArenaHeight;}
			set 
			{
				if (state.started) throw new ApplicationException("Cannot set game parameters after game has started");
				rules.ArenaHeight = value;
			}
		}

		/// <summary>The maximum speed of a robot.</summary>
		// Read-only once the game has started.
		public int MaxMoveSpeed 
		{
			get {return rules.MaxMoveSpeed;}
			set 
			{
				if (state.started) throw new ApplicationException("Cannot set game parameters after game has started");
				rules.MaxMoveSpeed = value;
			}
		}

		/// <summary>The maximum speed a robot can turn, in FullCircleths of a full turn per tick.</summary>
		// Read-only once the game has started.
		public int MaxTurnSpeed 
		{
			get {return rules.MaxTurnSpeed;}
			set 
			{
				if (state.started) throw new ApplicationException("Cannot set game parameters after game has started");
				rules.MaxTurnSpeed = value;
			}
		}

		/// <summary>The maximum speed a robot can turn its gun or camera, in FullCircleths of a full turn per tick.</summary>
		// Read-only once the game has started.
		public int MaxTurretTurnSpeed 
		{
			get {return rules.MaxTurretTurnSpeed;}
			set 
			{
				if (state.started) throw new ApplicationException("Cannot set game parameters after game has started");
				rules.MaxTurretTurnSpeed = value;
			}
		}

		/// <summary>The maximum number of bullets any team can have in the air at once.</summary>
		// Read-only once the game has started.
		public int TeamShotsPermitted 
		{
			get {return rules.TeamShotsPermitted;}
			set 
			{
				if (state.started) throw new ApplicationException("Cannot set game parameters after game has started");
				rules.TeamShotsPermitted = value;
			}
		}

		/// <summary>The maximum number of bullets any bot can have in the air at once.</summary>
		// Read-only once the game has started.
		public int BotShotsPermitted 
		{
			get {return rules.BotShotsPermitted;}
			set 
			{
				if (state.started) throw new ApplicationException("Cannot set game parameters after game has started");
				rules.BotShotsPermitted = value;
			}
		}

		/// <summary>Starts the game.</summary>
		// This freezes all the rules so they can no longer be modified and
		// allocates resources, starting positions, etc ready for the game to
		// start. The game will not progress all by itself; instead, you need to
		// call Tick() repeatedly to make things actually happen.
		public void Start() 
		{
			if (state.started) throw new ApplicationException("Cannot start a game twice");

			if (sandboxed && !SecurityManager.SecurityEnabled) 
			{
				callSay(null, "NRobot cannot run securely in this environment, because Code Access Security is disabled. Use the -insecure switch if you fully trust the authors of all the robots that will be playing.");
				return;
			}

			arenaDomain = SandboxUtility.CreateDomain("NRobotArena", sandboxed);
			arena = (GameArena) arenaDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(GameArena).FullName);

			state = arena.Start(state, rules);
			state.game = this;

			foreach (SayMessage message in state.sayMessages) 
			{
				callSay(message.Robot, message.Message);
			}
		}

		/// <summary>True if the game has started, false otherwise.</summary>
		public bool Started {get {return state.started;}}
    
		/// <summary>Perform one tick's worth of game progress.</summary>
		// This method should be called from a timer or a loop to cause the game
		// to go forward. Each robot's controlling class will have its Tick()
		// method called, then the game will move each robot the appropriate
		// distance in the appropriate direction, calculate damage, update bullet
		// locations etc.
		public void Tick() 
		{
			if (!state.started) throw new ApplicationException("Cannot call Tick() before starting game");
			if (state.over) throw new ApplicationException("Cannot call Tick() once game is over");

			state = arena.Tick();
			state.game = this;

			foreach (SayMessage message in state.sayMessages) 
			{
				callSay(message.Robot, message.Message);
			}

			if (state.over) 
			{
				arena = null;
				AppDomain.Unload(arenaDomain);
				arenaDomain = null;
			}
		}

		/// <summary>Obtain the list of teams in the game, in the order they were
		/// added.</summary>
		// Teams remain in this list even after they are eliminated.
		public IList Teams {get {return state.teams;}}

		/// <summary>Obtain the list of robots in the game, in an undefined
		/// order.</summary>
		// Robots remain in this list even after they are eliminated.
		// Cannot be called before starting game.
		public IList Robots 
		{
			get 
			{
				if (!state.started) throw new ApplicationException("Cannot get robots list until game start");
				return state.robots;
			}
		}

		/// <summary>Obtain the list of robots in the game, in an undefined
		/// order.</summary>
		// Robots are removed from this list when they are eliminated.
		// Cannot be called before starting game.
		public IList AliveBots 
		{
			get 
			{
				if (!state.started) throw new ApplicationException("Cannot get alive bots list until game start");
				return state.aliveBots;
			}
		}

		/// <summary>Obtain a list of all bullets in flight.</summary>
		// Cannot be called before starting game.
		public IList Bullets 
		{
			get 
			{
				if (!state.started) throw new ApplicationException("Cannot get bullets list until game start");
				return state.bullets;
			}
		}

		/// <summary>True if the game is over.</summary>
		public bool Over {get {return state.over;}}

		/// <summary>Return the team that won the game.</summary>
		// Cannot be called if the game is not over.
		public Team Winner 
		{
			get 
			{
				if (!state.over) throw new ApplicationException("Cannot get winner until game is over");
				return state.winner;
			}
		}

		internal void callSay(Robot robot, string message) 
		{
			Sayer say = Say;
			if (say != null) say(robot, message);
		}

		/// <summary>Fires when a robot says something using the "Say" method</summary>
		public event Sayer Say;

	}

	public delegate void Sayer(Robot bot, string message);
}