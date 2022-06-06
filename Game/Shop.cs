using Entities.Characters;
using Entities.Characters.Tier1;
using Entities.Foods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
	public class Shop
	{
		private static readonly int DEFAULT_GOLD = 10;
		private static readonly int MAX_PET = 15;
		private static readonly int SHOP_SIZE = 4;
		private static readonly Character[][] GLOBAL_LOBBY;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private int currentGold;
		private int tier;
		private int antQuantity;
		private int nextCharacterPrice = 3;
		private int nextRollPrice = 1;
		private int turn;
		private Character[] currentShop;





		private Player player;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public int CurrentGold
		{
			get { return currentGold; }
			protected set { currentGold = value; }
		}

		public int Tier
		{
			get { return tier; }
			protected set { tier = value; }
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Shop(Player player)
		{
			this.player = player;
			//antQuantity = defineQuantity(Ant.tier);
			currentShop = new Character[SHOP_SIZE];
		}

		static Shop()
		{
			// Constructeur static
			// Instancier et créer le Lobby global
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public void TryBuyCharacter(Character character, int teamPosition)
		{
			if (currentGold >= nextCharacterPrice)
			{
				BuyCharacter(character, teamPosition);
			}
			else
			{
				Console.WriteLine("Not enough gold");
			}
		}
		public void TryBuyFood(Food food, int teamPosition)
		{
			if (currentGold >= nextCharacterPrice)
			{
				BuyFood(food, teamPosition);
			}
			else
			{
				Console.WriteLine("Not enough gold");
			}
		}
		public void TryRenewShop()
		{
			if(currentGold >= nextRollPrice)
			{
				RenewShop();
            }
            else
			{
				Console.WriteLine("Not enough gold");
			}

		}
		public void Sell(Character character)
		{
			player.Remove(character);
			currentGold += character.Level;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <summary>
		/// Called on the click of a character in the shop and draggin it to the team
		/// </summary>
		/// <param name="character"></param>
		/// <param name="teamPosition"></param>
		private void BuyCharacter(Character character, int teamPosition)
		{
			/** 3 cas de figures
			 * 1) la place est libre
			 * 2) la place est prise
			 * 3) la place est prise et c'est le même pet
			 */
			if (player.Characters.ElementAt(teamPosition) is null) //1)
			{
				player.Add(character, teamPosition);

			}
			/*
			else if (player.Characters.ElementAt(teamPosition) is characterType) //3)
			{
				bool leveledUp = player.Characters.ElementAt(teamPosition).LevelUp(); //Info level 2 or 3
                if (leveledUp)
				{
					AddLevelUpCharacter();
				}
			}*/
			else //2) Ne fait rien
			{
				return;
			}
			//Enelver un des characters disponible du shop
			currentGold -= nextCharacterPrice;
		}
		private void BuyFood(Food food, int teamPosition)
		{
			/** 2 cas de figures
			 * 1) la place a pas de pet
			 * 2) la place a un pet
			 */

			if (player.Characters.ElementAt(teamPosition) is null) //1)
			{
				return;
			}
			else
			{
				//player.Characters.ElementAt(teamPosition).AddFood(food);
			}

			currentGold -= nextCharacterPrice;
		}

		private void AddLevelUpCharacter()
		{
			Random rnd = new Random();
			int tier = this.tier + 1;
			int pet = rnd.Next(GLOBAL_LOBBY[tier].Length);
			currentShop[SHOP_SIZE-1] = GLOBAL_LOBBY[tier][pet].Clone();
		}
		private void RenewShop()
		{

			Random rnd = new Random();
			currentShop = new Character[SHOP_SIZE];


			for (int k = 0; k < SHOP_SIZE; k++)
			{
				int tier = rnd.Next(this.tier);
				int pet = rnd.Next(GLOBAL_LOBBY[tier].Length);

				currentShop[k] = GLOBAL_LOBBY[tier][pet].Clone();
			}
		}

		private void TierLevelUp()
		{
			if (turn % 3 == 0)
				tier++;
			Console.WriteLine("LEVEL UP");
		}
		private void NewTurn()
		{
			this.turn++;
			TierLevelUp();
			RenewShop();
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         PROTECTED METHODS                         *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		// Majuscule


		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          STATIC METHODS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         ABSTRACT METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                              INDEXERS                             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		// Majuscule

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         OPERATORS OVERLOAD                        *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		// Majuscule
	}
}
