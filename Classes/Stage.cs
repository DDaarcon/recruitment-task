using System;

namespace ZadanieRekrutacyjne.Classes {
	public partial class Stage {
		public static readonly char INTACT_CHAR = ' ';
		public static readonly char SHOT_CHAR = 'X';
		public static readonly char MISS_CHAR = 'O';

		public static readonly int STAGE_WIDTH = 10;
		public static readonly int STAGE_HEIGHT = 10;
		

		public enum ShipPresence {Empty, Ship}
		public enum ShotState {Intact, Shot, Miss}

		public Stage opponentsStage {get; set;}

		private ShipPresence[,] shipBoard;
		public ShotState[,] shotBoard {get; private set;}

		public char[] visibleCharacters {get; private set;}

		public bool recievedAttack {get; private set;}
		public bool allShipsSunk {get; private set;}


		private void PlaceShip(int x, int y, bool horizontal, int length) {
			for (int i = 0; i < length; i++) {
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

		private bool CheckIfAllShipsAreSunk(){
			for (int i = 0; i < STAGE_WIDTH; i++) {
				for (int j = 0; j < STAGE_HEIGHT; j++) {
					if (shipBoard[i, j] == ShipPresence.Ship && !(shotBoardLength[i, j] == ShotState.Shot)) {
						return false;
					}
				}
			}
			return true;
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
						PlaceShip(xPos, yPos, horizontal, shipsLengths[i]);
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
			else {
				opponentsStage.ReceiveAttack(x, y);
				allShipsSunk = CheckIfAllShipsAreSunk();
				recievedAttack = false;
				return true;
			}
		}


}
}