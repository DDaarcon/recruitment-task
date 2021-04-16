using System;


class Stage {

	public enum ShipPresence {Empty, Ship}
	public enum ShotState {Intact, Shot}

	public Stage oppnentsStage;

	private ShipPresence[,] shipBoard;
	public ShotState[,] shotBoard;

	bool ownTurn;


	public ShipPresence ReciveAttack(int x, int y) {
		return ShipPresence.Empty;
	}

}