using System;
using System.Collections.Generic;

namespace ZadanieRekrutacyjne.Classes {
	public class AI {
		struct Coords {
			public int x;
			public int y;
		}

		private int CompareCoords(Coords coords1, Coords coords2, Ship.Direction direction) {
			if (direction == Ship.Direction.Horizontal) {
				return coords1.x.CompareTo(coords2.x);
			}
			else if (direction == Ship.Direction.Vertical) {
				return coords2.y.CompareTo(coords2.y);
			}
		}

		// private class Ship {
		// 	public enum Direction {Horizontal, Vertical, Unknown}
		// 	public Direction direction;
		// 	public bool destroyed;
		// 	public List<Coords> alreadyHit;
		// 	public List<Coords> possibleTargets;
			
		// }

		public Stage ownStage;
		// private Ship[] foundEnemyShips;
		private List<Coords> alreadyHit;
		private List<Coords> possibleTargets;
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

			// have not found undestroyed enemy ship, shoot randomly

		}
	}
}