using Entities.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Phase
{
	public class Battle
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private Character[] team1;
		private Character[] team2;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public event EventHandler FaintEvent;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Battle(Character[] team1, Character[] team2)
		{
			StartBattle();
			this.team1 = team1;
			this.team2 = team2;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
		private void StartBattle()
        {
			int index1 = 0, index2 = 0;
			//Check start of battle ?
			while (!IsTeamDead(team1) || !IsTeamDead(team2))
			{
				
				while(team1[index1].IsDead() || team1[index1] is null)
				{
					index1++;
				}
				while(team2[index2].IsDead() || team2[index2] is null)
				{
					index2++;
				}
				team2[index2].Health -= team1[index1].Damage;
				team1[index1].Health -= team2[index2].Damage;
				//Check hurt ability ?
				if (team1[index1].IsDead())
				{
					//Trigger Ability ?
				}
				if (team2[index2].IsDead())
                {
					//Trigger Ability ?
                }
			}
			if (IsTeamDead(team2) && IsTeamDead(team1))
			{

			}
			else if (IsTeamDead(team1))
			{

			}
			else
			{

			}
		}


		private bool IsTeamDead(Character[] Team)
		{
			bool IsDead = true;
			foreach (Character C in Team)
			{
				if (!C.IsDead())
				{
					IsDead = false;
				}
			}

			return IsDead;
		}


		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         PROTECTED METHODS                         *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          STATIC METHODS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         ABSTRACT METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */




		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                              INDEXERS                             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         OPERATORS OVERLOAD                        *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

	}
}
