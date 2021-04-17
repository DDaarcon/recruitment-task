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
		public void DealBetterAttack() {
			if (ownStage == null) return;

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
					coords.x = random.Next(Stage.STAGE_WIDTH);
					coords.y = random.Next(Stage.STAGE_HEIGHT);
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
				if (coords.x < Stage.STAGE_WIDTH - 1) {
					possibleTargets.Enqueue(nextPossibleTarget);
				}
				nextPossibleTarget = new Coords(coords.x, coords.y + 1);
				if (coords.y < Stage.STAGE_HEIGHT - 1) {
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