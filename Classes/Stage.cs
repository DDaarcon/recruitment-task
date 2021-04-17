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

		public Stage opponentsStage {get; set;}

		private ShipData[] shipsData;
		private ShipPresence[,] shipBoard;
		public ShotState[,] shotBoard {get; private set;}

		public char[] visibleCharacters {get; private set;}

		public int[] shipsLengths {get; private set;}
		public bool[] shipsSunk {get; private set;}
		public bool recievedAttack {get; private set;}
		public bool allShipsSunk {get; private set;}


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
			allShipsSunk = false;
		}

		private bool CheckIfShipsAreSunk(){
			bool allAreSunk = true;

			for (int i = 0; i < shipsLengths.Length; i++) {
				if (shipsSunk[i]) continue;

				bool isSunk = true;
				for (int j = 0; j < shipsLengths[i]; j++) {
					int x, y;
					x = shipsData[i].startX + (shipsData[i].horizontal ? j : 0);
					y = shipsData[i].startY + (!shipsData[i].horizontal ? j : 0);

					if (shotBoard[x, y] != ShotState.Shot) {
						isSunk = false;
						break;
					}
				}

				shipsSunk[i] = isSunk;
				if (!isSunk) allAreSunk = false;
			}

			return allAreSunk;
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

		public void PlaceShips(int[] shipsLengths) {
			this.shipsLengths = shipsLengths;
			shipsSunk = new bool[shipsLengths.Length];
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

		public ShipPresence ReceiveAttack(int x, int y) {
			if (recievedAttack) throw new Exception("Already recieved attack");
			if (allShipsSunk) return ShipPresence.Empty;

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

		public bool DealAttack(int x, int y) {
			if (allShipsSunk) return false;
			// already shot here
			if (opponentsStage.shotBoard[x, y] != ShotState.Intact) {
				return false;
			}
			opponentsStage.ReceiveAttack(x, y);
			allShipsSunk = CheckIfShipsAreSunk();
			recievedAttack = false;
			return true;
		}


}
}