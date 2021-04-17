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

		// private int CompareCoords(Coords coords1, Coords coords2, Ship.Direction direction) {
		// 	if (direction == Ship.Direction.Horizontal) {
		// 		return coords1.x.CompareTo(coords2.x);
		// 	}
		// 	else if (direction == Ship.Direction.Vertical) {
		// 		return coords2.y.CompareTo(coords2.y);
		// 	}
		// }

		// private class Ship {
		// 	public enum Direction {Horizontal, Vertical, Unknown}
		// 	public Direction direction;
		// 	public bool destroyed;
		// 	public List<Coords> alreadyHit;
		// 	public List<Coords> possibleTargets;
			
		// }

		public Stage ownStage;
		// private Ship[] foundEnemyShips;
		private List<Coords> alreadyHit = new List<Coords>();
		private Queue<Coords> possibleTargets = new Queue<Coords>();
		public void DealBetterAttack() {
			if (ownStage == null) return;

			// foreach (Ship ship in foundEnemyShips) {
			// 	if (ship.destroyed) continue;

			// 	if (ship.possibleTargets.Count > 0) {
			// 		Coords coords = ship.possibleTargets[0];
			// 		ship.possibleTargets.RemoveAt(0);

			// 		Stage.ShotState result = ownStage.DealAttack(coords.x, coords.y);

			// 		// resolve given result
			// 		if (ship.direction == Ship.Direction.Unknown) {
			// 			Coords previousHit = ship.alreadyHit[0];

			// 		}
			// 	}
			// }

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
				Random random = new Random();

				do {
					coords.x = random.Next(10);
					coords.y = random.Next(10);
				} while (!ownStage.DealAttack(coords.x, coords.y));
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
				if (coords.x < 9) {
					possibleTargets.Enqueue(nextPossibleTarget);
				}
				nextPossibleTarget = new Coords(coords.x, coords.y + 1);
				if (coords.y < 9) {
					possibleTargets.Enqueue(nextPossibleTarget);
				}
			}

			return;
		}


		public AI(Stage ownStage) {
			this.ownStage = ownStage;
		}
	}
}