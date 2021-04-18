using System;
using Microsoft.AspNetCore.Components;
using ZadanieRekrutacyjne.Classes;

namespace ZadanieRekrutacyjne.Pages {

	public class IndexClass : ComponentBase {

		bool firstPlayerTurn = true;
		public Stage stage1;
		public Stage stage2;
		private AI ai1, ai2;

		public bool useProbabilityDensityGuessing = false;
		public int turns;
		public int wonPlayer;

		private int[] ships = {
			5, 4, 3, 3, 2
		};
		protected override void OnInitialized() {
			
			RestartGame();

		}

		public void PlayTillEnd() {
			int safetyCounter = 0;
			while (safetyCounter < 200) {
				if (!NextMove()) break;
				safetyCounter++;
			}
			if (safetyCounter == 200) Console.WriteLine("safetyExit");
		}

		public void NextMoveBtn() {
			NextMove();
		}
		public bool NextMove() {
			if (stage1.allShipsSank) {
				wonPlayer = 2;
				return false;
			}
			if (stage2.allShipsSank) {
				wonPlayer = 1;
				return false;
			}

			if (firstPlayerTurn) {
				ai1.DealBetterAttack(useProbabilityDensityGuessing);
				turns++;
			} else {
				ai2.DealBetterAttack(useProbabilityDensityGuessing);
			}

			firstPlayerTurn = !firstPlayerTurn;
			return true;
		}

		public void CheckboxClicked(object checkedValue) {
			useProbabilityDensityGuessing = (bool) checkedValue;
		}

		public void RestartGame() {
			turns = 0;
			wonPlayer = 0;
			stage1 = new Stage(ships);
			stage2 = new Stage(stage1, ships);
			ai1 = new AI(stage1);
			ai2 = new AI(stage2);
			stage1.opponentsStage = stage2;
		}
	}

}
