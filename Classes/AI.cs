using System;
using System.Collections.Generic;

namespace ZadanieRekrutacyjne.Classes {
	class AI {
		struct Coords {
			public int x;
			public int y;

			public Coords(int x, int y) {
				this.x = x;
				this.y = y;
			}
		}

		internal Stage ownStage;
		private List<Coords> alreadyHit = new List<Coords>();
		private Queue<Coords> possibleTargets = new Queue<Coords>();


		private int GetShortestPossibleShipLength() {
			Stage opponentsStage = ownStage.opponentsStage;
			int shortest = opponentsStage.shipsLengths[0];

			for (int i = 0; i < opponentsStage.shipsLengths.Length; i++) {
				if (opponentsStage.shipsLengths[i] < shortest && !opponentsStage.shipsSank[i])
					shortest = opponentsStage.shipsLengths[i];
			}

			return shortest;
		}

		private int GetMinInt(params int[] values) {
			int min = values[0];
			for (int i = 1; i < values.Length; i++) {
				if (values[i] < min) min = values[i];
			}
			return min;
		}

		private Coords ProbabilityDensityHelper(int[,] probabilityMap, int[] shipsLengths) {
			Stage opponentsStage = ownStage.opponentsStage;
			int[] shipsLengthsCopy = shipsLengths.Clone() as int[];
			// sort ascending
			Array.Sort(shipsLengthsCopy);
			Coords highestProbabilityCoords = new Coords(0, 0);
			int highestProbabilityValue = 0;

			// go horizontally
			for (int line = 0; line < Stage.STAGE_HEIGHT; line++) {
				int countIntactFields = 0;

				for (int posInLine = 0; posInLine < Stage.STAGE_WIDTH; posInLine++) {
					Stage.ShotState fieldsShotState = opponentsStage.shotBoard[posInLine, line];

					// count intact fields in line
					if (fieldsShotState == Stage.ShotState.Intact) {
						countIntactFields++;
					}

					if (fieldsShotState != Stage.ShotState.Intact || posInLine == Stage.STAGE_WIDTH - 1) {
						// if found [not Intact] filed - count without it
						// if reached end (and is Intact) - count with it
						int prevPosInLine = posInLine - (posInLine == Stage.STAGE_WIDTH - 1 ? 0 : 1);

						foreach (int shipLength in shipsLengthsCopy) {
							if (shipLength > countIntactFields) break;
							// go through intact fields again
							for (int intactAgain = 0; intactAgain < countIntactFields; intactAgain++) {
								int valueToAdd = GetMinInt(
									intactAgain + 1,
									countIntactFields - intactAgain,
									shipLength);
								
								probabilityMap[prevPosInLine - countIntactFields + intactAgain + 1, line] += valueToAdd;

								if (probabilityMap[prevPosInLine - countIntactFields + intactAgain + 1, line] > highestProbabilityValue) {
									highestProbabilityCoords.x = prevPosInLine - countIntactFields + intactAgain + 1;
									highestProbabilityCoords.y = line;
									highestProbabilityValue = probabilityMap[prevPosInLine - countIntactFields + intactAgain + 1, line];
								}

							}
						}
					}
					
					if (fieldsShotState != Stage.ShotState.Intact) countIntactFields = 0;

				}
			}

			// go vertically
			for (int lineV = 0; lineV < Stage.STAGE_WIDTH; lineV++) {
				int countIntactFields = 0;

				for (int posInLineV = 0; posInLineV < Stage.STAGE_HEIGHT; posInLineV++) {
					Stage.ShotState fieldsShotState = opponentsStage.shotBoard[lineV, posInLineV];

					// count intact fields in line
					if (fieldsShotState == Stage.ShotState.Intact) {
						countIntactFields++;
					}
					
					if (fieldsShotState != Stage.ShotState.Intact || posInLineV == Stage.STAGE_HEIGHT - 1) {
						int prevPosInLineV = posInLineV - (posInLineV == Stage.STAGE_HEIGHT - 1 ? 0 : 1);

						foreach (int shipLength in shipsLengthsCopy) {
							if (shipLength > countIntactFields) break;
							// go through intact fields again
							for (int intactAgain = 0; intactAgain < countIntactFields; intactAgain++) {
								int valueToAdd = GetMinInt(
									intactAgain + 1,
									countIntactFields - intactAgain,
									shipLength);
								
								probabilityMap[lineV, prevPosInLineV - countIntactFields + intactAgain + 1] += valueToAdd;

								if (probabilityMap[lineV, prevPosInLineV - countIntactFields + intactAgain + 1] > highestProbabilityValue) {
									highestProbabilityCoords.x = lineV;
									highestProbabilityCoords.y = prevPosInLineV - countIntactFields + intactAgain + 1;
									highestProbabilityValue = probabilityMap[lineV, prevPosInLineV - countIntactFields + intactAgain + 1];
								}

							}
						}
					}

					if (fieldsShotState != Stage.ShotState.Intact) countIntactFields = 0;
				}
			}

			for (int i = 0; i < Stage.STAGE_WIDTH; i++) {
				for (int j = 0; j < Stage.STAGE_HEIGHT; j++) {
					Console.Write(probabilityMap[i, j] + " ");
				}
				Console.Write('\n');
			}
			Console.WriteLine("Highest possiblility coords: x: " + highestProbabilityCoords.x + " y: " + highestProbabilityCoords.y);

			return highestProbabilityCoords;
		}

		/**
		<summary>Get random guess from available</summary>
		**/
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

		/**
		<summary>Probability density guessing (creating probabilityMap and filling it with 0s, calling ProbabilityDensityHelper)</summary>
		**/
		private Coords ProbabilityDensityGuess() {
			int[,] probabilityMap = new int[Stage.STAGE_WIDTH, Stage.STAGE_HEIGHT];
			Stage opponentsStage = ownStage.opponentsStage;
			Coords bestCoordsToPlace = new Coords(0, 0);
		
			for (int i = 0; i < Stage.STAGE_WIDTH; i++) {
				for (int j = 0; j < Stage.STAGE_HEIGHT; j++) {
					probabilityMap[i, j] = 0;
				}
			}

			bestCoordsToPlace = ProbabilityDensityHelper(probabilityMap, opponentsStage.shipsLengths);

			return bestCoordsToPlace;
		}

		/**
		<summary>Main method for dealing attack to opponent, uses random/probability related guessing</summary>
		**/
		internal bool DealBetterAttack(bool useProbabilityDensityGuessing) {
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
				coords = useProbabilityDensityGuessing ? ProbabilityDensityGuess() : BetterRandomGuess();
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


		internal AI(Stage ownStage) {
			this.ownStage = ownStage;
		}
	}
}