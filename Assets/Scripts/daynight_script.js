var timecycle : float;
var cycleset : float;

timecycle = 10;
cycleset = 0.1 / timecycle *-1;

function Update() {
	transform.Rotate(0, 0, cycleset, Space.World);
}