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
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace NRobot.Engine
{
	/// <summary>
	/// Utility methods that combine to allow setting up a sandboxed environment.
	/// </summary>
	// Most of these methods came verbatim from Shawn Farkas's blog. Presumably he
	// intended them to be used...
	public class SandboxUtility
	{
		/// <summary>
		/// Get a named permission set from the security policy
		/// </summary>
		/// <param name="name">Name of the permission set to retrieve</param>
		/// <exception cref="ArgumentException">If name is null or empty</exception>
		/// <returns>
		/// The intersection of permission sets with the given name from all policy
		/// levels, or an empty set if the name is not found
		/// </returns>
		public static PermissionSet GetNamedPermissionSet(string name)
		{
			if(name == null || name == "")
				throw new ArgumentException("name", "Cannot search for a permission set without a name");

			bool foundName = false;
			PermissionSet setIntersection = new PermissionSet(PermissionState.Unrestricted);

			// iterate over each policy level
			IEnumerator levelEnumerator = SecurityManager.PolicyHierarchy();
			while(levelEnumerator.MoveNext())
			{
				PolicyLevel level = levelEnumerator.Current as PolicyLevel;
				Debug.Assert(level != null);

				// if this level has defined a named permission set with the
				// given name, then intersect it with what we've retrieved
				// from all the previous levels
				PermissionSet levelSet = level.GetNamedPermissionSet(name);
				if(levelSet != null)
				{
					foundName = true;
					setIntersection = setIntersection.Intersect(levelSet);
				}
			}

			// Intersect() can return null for an empty set, so convert that
			// to an empty set object. Also return an empty set if we didn't find
			// the named permission set we were looking for
			if(setIntersection == null || !foundName)
				throw new ArgumentException("No permission set found named: " + name);
			else
				setIntersection = new NamedPermissionSet(name, setIntersection);

			// if no named permission sets were found, return an empty set,
			// otherwise return the set that was found
			return setIntersection;
		}


		static public PermissionSet ExecuteOnlyPermissionSet()
		{
			PermissionSet ps = new PermissionSet(PermissionState.None);
			ps.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
			return ps;
		}

		/// <summary>
		/// Create a StrongNameMembershipCondition that matches a specific assembly
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// if <paramref name="assembly"/> is null
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// if <paramref name="assembly"/> does not represent a strongly named assembly
		/// </exception>
		/// <param name="assembly">Assembly that will match the strong name membership condition</param>
		/// <returns>A membership condition that matches the given assembly</returns>
		public static StrongNameMembershipCondition CreateStrongNameMembershipCondition(Assembly assembly)
		{
			if(assembly == null)
				throw new ArgumentNullException("assembly");

			AssemblyName assemblyName = assembly.GetName();
			Debug.Assert(assemblyName != null, "Could not get assembly name");
        
			// get the public key blob
			byte[] publicKey = assemblyName.GetPublicKey();
			if(publicKey == null || publicKey.Length == 0)
				throw new InvalidOperationException(String.Format("{0} is not strongly named", assembly));

			StrongNamePublicKeyBlob keyBlob = new StrongNamePublicKeyBlob(publicKey);

			// and create the membership condition
			StrongNameMembershipCondition mc = new StrongNameMembershipCondition(
				keyBlob, assemblyName.Name, assemblyName.Version);

			Debug.Assert(mc.Check(assembly.Evidence), "Did not generate a matching membership condition");
			return mc;
		}	

		/// <summary>
		/// Create an AppDomain that contains policy restricting code to execute
		/// with only the permissions granted by a named permission set
		/// </summary>
		/// <param name="domainName">name of the new domain</param>
		/// <param name="appBase">directory to base the domain in; code will be loaded from here by default</param>
		/// <param name="permissionSetName">name of the permission set to restrict to, or null to restrict to execute only</param>
		/// <param name="extraCodeGroup">extra code groups to add</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// if <paramref name="permissionSetName"/> is empty
		/// </exception>
		/// <returns>AppDomain with a restricted security policy</returns>
		public static AppDomain CreateRestrictedDomain(string domainName, string permissionSetName)
		{
			if(permissionSetName != null && permissionSetName.Length == 0)
				throw new ArgumentOutOfRangeException("permissionSetName", permissionSetName, "Cannot have an empty permission set name");
        
			// Default to all code getting nothing
			PolicyStatement emptyPolicy = new PolicyStatement(new PermissionSet(PermissionState.None));
			UnionCodeGroup policyRoot = new UnionCodeGroup(new AllMembershipCondition(), emptyPolicy);

			// Get the right permission set
			PermissionSet ps;
			if (permissionSetName == null) 
			{
				ps = ExecuteOnlyPermissionSet();
			} 
			else 
			{
				ps = GetNamedPermissionSet(permissionSetName);
			}

			// Grant all code the named permission set passed in
			PolicyStatement permissions = new PolicyStatement(ps);
			policyRoot.AddChild(new UnionCodeGroup(new AllMembershipCondition(), permissions));
        
			// add a code group that causes the current assembly to be loaded with
			// full trust, as long as it is strongly named.
			PolicyStatement fullTrust = new PolicyStatement(new PermissionSet(PermissionState.Unrestricted));
			CodeGroup trustSelf = new UnionCodeGroup(CreateStrongNameMembershipCondition(Assembly.GetExecutingAssembly()), fullTrust);
			policyRoot.AddChild(trustSelf);
        
			// create an AppDomain policy level for the policy tree
			PolicyLevel appDomainLevel = PolicyLevel.CreateAppDomainLevel();
			appDomainLevel.RootCodeGroup = policyRoot;

			// Create a setup object that will set the path to the appropriate base directory.
			// Setting the ApplicationBase has a problem in that it's difficult to then
			// persuade the new AppDomain to load the current assembly...
			//AppDomainSetup ads = new AppDomainSetup();
			//ads.ApplicationBase = appBase;

			// Create Evidence for top-of-stack, this forces everything into the
			// internet zone.
			Evidence ev = new Evidence();
			ev.AddHost(new Zone(SecurityZone.Internet));
 
			// Create the AppDomain with the setup and evidence objects
			AppDomain restrictedDomain = AppDomain.CreateDomain(domainName, ev);
			restrictedDomain.SetAppDomainPolicy(appDomainLevel);

			return restrictedDomain;
		}
		public static AppDomain CreateDomain(string domainName, bool restricted) 
		{
			return restricted ? CreateRestrictedDomain(domainName, null) : AppDomain.CreateDomain(domainName);
		}
	}
}
