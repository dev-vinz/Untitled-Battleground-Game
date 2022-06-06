using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Abilities
{
	public enum Ability
	{
		Faint,
		Hurt,
		StartOfBattle,
		StartOfTurn,
		EndOfTurn
	}

	public enum Side
	{
		Player,
		Opponent,
	}
}
