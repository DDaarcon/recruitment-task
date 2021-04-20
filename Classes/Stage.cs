using System;

namespace ZadanieRekrutacyjne.Classes {
	public partial class Stage {
		private class ShipData {
			public int startX, startY;
			public bool horizontal;
		}

		public static readonly char INTACT_CHAR = ' ';
		public static readonly char SHOT_CHAR = 'X';
		public static readonly char MISS_CHAR = 'O';

		public static readonly int STAGE_WIDTH = 10;
		public static readonly int STAGE_HEIGHT = 10;
		

		public enum ShipPresence {Empty, Ship}
		public enum ShotState {Intact, Shot, Miss}

		/**
		<value>Reference to opponents Stage</value>
		**/
		internal Stage opponentsStage {get; set;}

		private ShipData[] shipsData;
		/**
		<value>Array that store ship presence data</value>
		**/
		private ShipPresence[,] shipBoard;
		/**
		<value>Array that store information about recieved shots</value>
		**/
		internal ShotState[,] shotBoard {get; private set;}

		/**
		<value>Characters displayed on board, on screen</value>
		**/
		internal char[] visibleCharacters {get; private set;}

		/**
		<value>Lengths of ships present on board</value>
		**/
		internal int[] shipsLengths {get; private set;}
		/**
		<value>Information of ships that have sunk</value>
		**/
		internal bool[] shipsSank {get; private set;}
		/**
		<value>Have this Stage already recieved shot?</value>
		**/
		internal bool recievedAttack {get; private set;}
		/**
		<value>Lost game</value>
		**/
		internal bool allShipsSank {get; private set;}


		/**
		<summary>Place ship on the board and store data about it</summary>
		**/
		private void PlaceShip(int x, int y, bool horizontal, int index) {
			shipsData[index].startX = x;
			shipsData[index].startY = y;
			shipsData[index].horizontal = horizontal;
			
			for (int i = 0; i < shipsLengths[index]; i++) {
				if (horizontal) {
					shipBoard[x + i, y] = ShipPresence.Ship;
				} else {
					shipBoard[x, y + i] = ShipPresence.Ship;
				}
			}
		}
		/**
		<summary>Initialize array after start</summary>
		**/
		private void InitializeArrays() {
			shipBoard = new ShipPresence[STAGE_WIDTH, STAGE_HEIGHT];
			shotBoard = new ShotState[STAGE_WIDTH, STAGE_HEIGHT];
			visibleCharacters = new char[STAGE_WIDTH * STAGE_HEIGHT];

			for (int i = 0; i < STAGE_WIDTH; i++) {
				for (int j = 0; j < STAGE_HEIGHT; j++) {
					shipBoard[i, j] = ShipPresence.Empty;
					shotBoard[i, j] = ShotState.Intact;
					visibleCharacters[i * STAGE_WIDTH + j] = INTACT_CHAR;
				}
			}
			recievedAttack = false;
			allShipsSank = false;
		}

		/**
		<summary>Update shipsSank data</summary>
		<return>All ships have sunk?</return>
		**/
		private bool CheckIfShipsAreSank(){
			bool allAreSank = true;

			for (int i = 0; i < shipsLengths.Length; i++) {
				if (shipsSank[i]) continue;

				bool isSank = true;
				for (int j = 0; j < shipsLengths[i]; j++) {
					int x, y;
					x = shipsData[i].startX + (shipsData[i].horizontal ? j : 0);
					y = shipsData[i].startY + (!shipsData[i].horizontal ? j : 0);

					if (shotBoard[x, y] != ShotState.Shot) {
						isSank = false;
						break;
					}
				}

				shipsSank[i] = isSank;
				if (!isSank) allAreSank = false;
			}

			return allAreSank;
		}

		public Stage(int[] ships) {
			InitializeArrays();
			PlaceShips(ships);
		}

		public Stage(Stage opponent, int[] ships) {
			InitializeArrays();
			opponentsStage = opponent;
			PlaceShips(ships);
		}

		/**
		<summary>Randomly place ships on the board with given lengths of them</summary>
		**/
		internal void PlaceShips(int[] shipsLengths) {
			this.shipsLengths = shipsLengths;
			shipsSank = new bool[shipsLengths.Length];
			shipsData = new ShipData[shipsLengths.Length];

			var random = new Random();

			for (int i = 0; i < shipsLengths.Length; i++) {
				bool horizontal = random.Next(1) == 1 ? true : false;

				// pick random position until available is found
				while (true) {
					int xPos, yPos;
					xPos = random.Next(STAGE_WIDTH - (horizontal ? shipsLengths[i] : 0));
					yPos = random.Next(STAGE_HEIGHT - (!horizontal ? shipsLengths[i] : 0));

					bool collision = false;
					for (int sLen = 0; sLen < shipsLengths[i]; sLen++) {
						if (horizontal) {
							if (shipBoard[xPos + sLen, yPos] == ShipPresence.Ship) {
								collision = true;
								break;
							}
						} else {
							if (shipBoard[xPos, yPos + sLen] == ShipPresence.Ship) {
								collision = true;
								break;
							}
						}
					}

					if (!collision) {
						shipsData[i] = new ShipData();
						PlaceShip(xPos, yPos, horizontal, i);
						break;
					}
				}
			}
		}

		internal ShipPresence ReceiveAttack(int x, int y) {
			if (recievedAttack) throw new Exception("Already recieved attack");
			if (allShipsSank) return ShipPresence.Empty;

			if (shipBoard[x, y] == ShipPresence.Ship) {
				shotBoard[x, y] = ShotState.Shot;
				visibleCharacters[x * STAGE_WIDTH + y] = SHOT_CHAR;

			} else {
				shotBoard[x, y] = ShotState.Miss;
				visibleCharacters[x * STAGE_WIDTH + y] = MISS_CHAR;
			}
			recievedAttack = true;
			return shipBoard[x, y];
		}

		internal bool DealAttack(int x, int y) {
			if (allShipsSank) return false;
			// already shot here
			if (opponentsStage.shotBoard[x, y] != ShotState.Intact) {
				return false;
			}
			opponentsStage.ReceiveAttack(x, y);
			allShipsSank = CheckIfShipsAreSank();
			recievedAttack = false;
			return true;
		}


}
}