using System;
using System.Collections.Generic;

namespace ZadanieRekrutacyjne.Classes {
	public class AI {
		struct Coords {
			public int x;
			public int y;

			public Coords(int x, int y) {
				this.x = x;
				this.y = y;
			}
		}

		public Stage ownStage;
		private List<Coords> alreadyHit = new List<Coords>();
		private Queue<Coords> possibleTargets = new Queue<Coords>();


		private int GetShortestPossibleShipLength() {
			Stage opponentsStage = ownStage.opponentsStage;
			int shortest = opponentsStage.shipsLengths[0];

			for (int i = 0; i < opponentsStage.shipsLengths.Length; i++) {
				if (opponentsStage.shipsLengths[i] < shortest && !opponentsStage.shipsSunk[i])
					shortest = opponentsStage.shipsLengths[i];
			}

			return shortest;
		}

		private Coords ProbabilityDensityHelper(ref int[,] probabilityMap, int shipLength) {
			Stage opponentsStage = ownStage.opponentsStage;
			Coords highestProbabilityCoords = new Coords(0, 0);
			int highestProbabilityValue = 0;

			for (int x = 0; x < Stage.STAGE_WIDTH - shipLength; x++) {
				for (int y = 0; y < Stage.STAGE_HEIGHT - shipLength; y++) {
					bool canBePut = true;

					// check if can be placed horizontally
					for (int sX = x; sX < x + shipLength; sX++) {
						if (opponentsStage.shotBoard[sX, x] != Stage.ShotState.Intact) {
							canBePut = false;
							break;
						}
					}
					// increase probability
					if (canBePut) {
						for (int sX = x; sX < x + shipLength; sX++) {
							probabilityMap[sX, y]++;
							if (probabilityMap[sX, y] > highestProbabilityValue) {
								highestProbabilityCoords.x = sX;
								highestProbabilityCoords.y = y;
								highestProbabilityValue = probabilityMap[sX, y];
							}
						}
					}

					// check if can be placed vertically
					canBePut = true;
					for (int sY = y; sY < y + shipLength; sY++) {
						if (opponentsStage.shotBoard[x, sY] != Stage.ShotState.Intact) {
							canBePut = false;
							break;
						}
					}
					if (canBePut) {
						for (int sY = y; sY < y + shipLength; sY++) {
							probabilityMap[x, sY]++;
							if (probabilityMap[x, sY] > highestProbabilityValue) {
								highestProbabilityCoords.x = x;
								highestProbabilityCoords.y = sY;
								highestProbabilityValue = probabilityMap[x, sY];
							}
						}
					}
				}
			}

			return highestProbabilityCoords;
		}

		private Coords BetterRandomGuess() {
			Random random = new Random();
			Stage opponentsStage = ownStage.opponentsStage;
			Coords coords;

			while (true) {
				coords.x = random.Next(Stage.STAGE_WIDTH / 2) * 2;
				coords.y = random.Next(Stage.STAGE_HEIGHT);
				if (coords.y % 2 == 1) coords.x++;

				// if guessed cell has been already shot - guess next
				if (opponentsStage.shotBoard[coords.x, coords.y] != Stage.ShotState.Intact) continue;

				int shortestLen = GetShortestPossibleShipLength();
				int possibleLengthFound = 1;
				bool fits = false;

				// go left and count intact cells
				for (int xLeft = coords.x - 1; 
					xLeft >= 0 && opponentsStage.shotBoard[xLeft, coords.y] == Stage.ShotState.Intact;
					xLeft--) 
				{
					possibleLengthFound++;
					if (possibleLengthFound >= shortestLen) {
						fits = true;
						break;
					}
				}
				if (fits) break;
				
				// go right
				for (int xRight = coords.x + 1;
					xRight < Stage.STAGE_WIDTH && opponentsStage.shotBoard[xRight, coords.y] == Stage.ShotState.Intact;
					xRight++)
				{
					possibleLengthFound++;
					if (possibleLengthFound >= shortestLen) {
						fits = true;
						break;
					}
				}
				if (fits) break;

				possibleLengthFound = 1;

				// go up
				for (int yUp = coords.y - 1;
					yUp >= 0 && opponentsStage.shotBoard[coords.x, yUp] == Stage.ShotState.Intact;
					yUp--)
				{
					possibleLengthFound++;
					if (possibleLengthFound >= shortestLen) {
						fits = true;
						break;
					}
				}
				if (fits) break;

				// go down
				for (int yDown = coords.y + 1;
					yDown < Stage.STAGE_HEIGHT && opponentsStage.shotBoard[coords.x, yDown] == Stage.ShotState.Intact;
					yDown++)
				{
					possibleLengthFound++;
					if (possibleLengthFound >= shortestLen) {
						fits = true;
						break;
					}
				}
				if (fits) break;
			}

			return coords;
		}

		private Coords ProbabilityDensityGuess() {
			int[,] probabilityMap = new int[Stage.STAGE_WIDTH, Stage.STAGE_HEIGHT];
			Stage opponentsStage = ownStage.opponentsStage;
			Coords bestCoordsToPlace = new Coords(0, 0);
		
			for (int i = 0; i < Stage.STAGE_WIDTH; i++) {
				for (int j = 0; j < Stage.STAGE_HEIGHT; j++) {
					probabilityMap[i, j] = 0;
				}
			}

			for (int i = 0; i < opponentsStage.shipsLengths.Length; i++) {
				if (opponentsStage.shipsSunk[i]) continue;
				bestCoordsToPlace = ProbabilityDensityHelper(ref probabilityMap, opponentsStage.shipsLengths[i]);
			}

			return bestCoordsToPlace;
		}

		public bool DealBetterAttack() {
			if (ownStage == null) return false;

			Coords coords = new Coords(0, 0);
			Stage.ShotState result;
			bool gotFromPossibleTargets = false;

			if (possibleTargets.Count > 0) {
				do {
					if (possibleTargets.Count <= 0) break;
					coords = possibleTargets.Dequeue();
					gotFromPossibleTargets = ownStage.DealAttack(coords.x, coords.y);
				} while (!gotFromPossibleTargets);
			}

			if (!gotFromPossibleTargets) {
				coords = ProbabilityDensityGuess();
				ownStage.DealAttack(coords.x, coords.y);
			}
			result = ownStage.opponentsStage.shotBoard[coords.x, coords.y];

			// ship hit, add possible targets
			if (result == Stage.ShotState.Shot) {

				Coords nextPossibleTarget = new Coords(coords.x - 1, coords.y);
				if (coords.x > 0) {
					possibleTargets.Enqueue(nextPossibleTarget);
				}
				nextPossibleTarget = new Coords(coords.x, coords.y - 1);
				if (coords.y > 0) {
					possibleTargets.Enqueue(nextPossibleTarget);
				}
				nextPossibleTarget = new Coords(coords.x + 1, coords.y);
				if (coords.x < Stage.STAGE_WIDTH - 1) {
					possibleTargets.Enqueue(nextPossibleTarget);
				}
				nextPossibleTarget = new Coords(coords.x, coords.y + 1);
				if (coords.y < Stage.STAGE_HEIGHT - 1) {
					possibleTargets.Enqueue(nextPossibleTarget);
				}
			}

			return true;
		}


		public AI(Stage ownStage) {
			this.ownStage = ownStage;
		}
	}
}