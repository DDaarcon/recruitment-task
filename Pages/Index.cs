using System;
using Microsoft.AspNetCore.Components;
using ZadanieRekrutacyjne.Classes;

namespace ZadanieRekrutacyjne.Pages {

	public class IndexClass : ComponentBase {

		bool firstPlayerTurn = true;
		public Stage stage1;
		public Stage stage2;

		private int[] ships = {
			5, 4, 3, 3, 2
		};
		protected override void OnInitialized() {
			
			stage1 = new Stage(ships);
			stage2 = new Stage(stage1, ships);
			stage1.opponentsStage = stage2;
		}

		public void NextMove() {
			var random = new Random();
			int xPos, yPos;
			if (firstPlayerTurn) {
				do {
					xPos = random.Next(10);
					yPos = random.Next(10);
				} while (!stage1.DealAttack(xPos, yPos));
			} else {
				do {
					xPos = random.Next(10);
					yPos = random.Next(10);
				} while (!stage2.DealAttack(xPos, yPos));
			}
			firstPlayerTurn = !firstPlayerTurn;
		}
	}

}
