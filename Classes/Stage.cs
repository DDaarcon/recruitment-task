using System;

namespace ZadanieRekrutacyjne.Classes {
	public partial class Stage {
		public readonly char INTACT_CHAR = ' ';
		public readonly char SHOT_CHAR = 'X';
		

		public enum ShipPresence {Empty, Ship}
		public enum ShotState {Intact, Shot}

		public Stage oppnentsStage;

		private ShipPresence[,] shipBoard;
		public ShotState[,] shotBoard;

		public char[] visibleCharacters = new char[100];


		private void SetAllCharacters(char c) {
			for (int i = 0; i < 100; i++) {
				visibleCharacters[i] = c;
			}
		}

		public Stage() {
			SetAllCharacters(INTACT_CHAR);
		}


		public ShipPresence ReciveAttack(int x, int y) {
			return ShipPresence.Empty;
		}



}
}