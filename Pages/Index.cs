using System;
using Microsoft.AspNetCore.Components;
using ZadanieRekrutacyjne.Classes;

namespace ZadanieRekrutacyjne.Pages {

	public class IndexClass : ComponentBase {

		bool firstPlayerTurn = true;
		public Stage stage1;
		public Stage stage2;
		private AI ai1, ai2;

		public int turns = 0;

		private int[] ships = {
			5, 4, 3, 3, 2
		};
		protected override void OnInitialized() {
			
			stage1 = new Stage(ships);
			stage2 = new Stage(stage1, ships);
			ai1 = new AI(stage1);
			ai2 = new AI(stage2);
			stage1.opponentsStage = stage2;

		}

		public void PlayTillEnd() {
			while (NextMove()) {}
		}

		public void NextMoveBtn() {
			NextMove();
		}
		public bool NextMove() {
			if (stage1.allShipsSunk || stage2.allShipsSunk) return false;

			if (firstPlayerTurn) {
				ai1.DealBetterAttack();
				turns++;
			} else {
				ai2.DealBetterAttack();
			}

			firstPlayerTurn = !firstPlayerTurn;
			return true;
		}
	}

}
