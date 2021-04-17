
namespace ZadanieRekrutacyjne.Classes {
	public class AI {
		struct Coords {
			int x;
			int y;
		}
		private class Ship {
			enum Direction {Horizontal, Vertical, Unknown}
			public Direction direction;
			public bool destroyed;
			public Coords[] alreadyHit;
			public Coords[] possibleTargets;
			
		}

		public Stage ownStage;
		private Ship[] foundEnemyShips;
		public void DealBetterAttack() {
			foreach (Ship ship in foundEnemyShips) {
				if (ship.destroyed) continue;

				// if (ship.possibleTargets.Length > 0)
			}
		}
	}
}