/*
 Copyright 2011 MCGalaxy
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
 */
using System;
using System.IO;
using System.Text;

namespace MCGalaxy {
	
	/// <summary> Can check the info about someone's ban, find out if there's info about someone,
	/// and add / remove someone to the baninfo (NOT THE BANNED.TXT !) </summary>
	public static class Ban {
		
		/// <summary> Adds a ban entry for the given user, and who banned them and why they were banned. </summary>
		public static void BanPlayer(Player p, string who, string reason, bool stealth, string oldrank) {
			string dayname = DateTime.Now.DayOfWeek.ToString();
			string daynumber = DateTime.Now.Day.ToString();
			string month = DateTime.Now.Month.ToString();
			string year = DateTime.Now.Year.ToString();
			string hour = DateTime.Now.Hour.ToString();
			string minute = DateTime.Now.Minute.ToString();
			string datetime = dayname + "%20" + daynumber + "%20" + month + "%20" + year + ",%20at%20" + hour + ":" + minute;
			
			string player = p == null ? "(console)" : p.name.ToLower();
			AddEntry(player, who.ToLower(), reason, stealth.ToString(), datetime, oldrank);
		}
		
		static void AddEntry(string pl, string who, string reason, string stealth, string datetime, string oldrank) {
			if (!File.Exists("text/bans.txt"))
				File.CreateText("text/bans.txt").Close();
			
			reason = CP437Writer.ConvertFromRaw(reason);
			File.AppendAllText("text/bans.txt", pl + " " + who + " " + reason + " " + stealth + " " + datetime + " " + oldrank + "\r\n");
		}
		
		/// <summary> Returns whether the given user is banned. </summary>
		public static bool IsBanned(string who) {
			who = who.ToLower();
			foreach (string line in File.ReadAllLines("text/bans.txt"))
				if (line.Split(' ')[1] == who) return true;
			return false;
		}
		
		/// <summary> Gives info about the ban of user, as a string array of
		/// { banned by, ban reason, stealth ban, date and time, previous rank }. </summary>
		public static string[] GetBanData(string who) {
			who = who.ToLower();
			string bannedby = "", reason = "", timedate = "", oldrank = "", stealth = "";
			foreach (string line in File.ReadAllLines("text/bans.txt")) {
				string[] parts = line.Split(' ');
				if (parts[1] == who) {
					bannedby = parts[0];
					reason = CP437Reader.ConvertLine(parts[2]);
					stealth = parts[3];
					timedate = parts[4];
					oldrank = parts[5];
				}
			}
			return new[] { bannedby, reason, timedate, oldrank, stealth };
		}
		
		/// <summary> Unbans the given user, returning whether the player was originally banned.
		public static bool DeleteBan(string name) {
			name = name.ToLower();
			bool success = false;
			StringBuilder sb = new StringBuilder();
			
			foreach (string line in File.ReadAllLines("text/bans.txt")) {
				if (line.Split(' ')[1] != name)
					sb.Append(line + "\r\n");
				else
					success = true;
			}
			File.WriteAllText("text/bans.txt", sb.ToString());
			return success;
		}
		
		/// <summary> Change the ban reason for the given user. </summary>
		public static string EditReason(string who, string reason) {
			who = who.ToLower();
			bool found = false;
			StringBuilder sb = new StringBuilder();
			
			foreach (string line in File.ReadAllLines("text/bans.txt")) {
				string[] parts = line.Split(' ');
				if (parts[1] == who) {
					parts[2] = CP437Writer.ConvertFromRaw(reason);				
					found = true;
					sb.Append(String.Join(" ", parts) + "\r\n");
				} else {
					sb.Append(line + "\r\n");
				}
			}
			
			if (found) {
				File.WriteAllText("text/bans.txt", sb.ToString());
				return "";
			}
			return "This player isn't banned!";
		}
	}
}