const GameOfWeekComponent = (props) =>
{
	if (props.gameScore) {
		let displayTeam = ''
		if (props.side === 'away score')
			displayTeam = props.gameScore.awayTeam;
		else
			displayTeam = props.gameScore.homeTeam;
		return (
			<div>
				<div className="container">
					<div className="row">
						<div className="col-sm-2">
							<p>{displayTeam}</p>
						</div>
						<div className="col-sm-2">
							<input
								alt={displayTeam}
								type="text"
								pattern="[0-9]*"
								value={props.teamScore}
								onChange={props.SetScore}
								tabIndex={props.tabIndex}
								title={props.side}
							/>
						</div>
					</div>
				</div>
			</div>
		);
	}
	return ("<br/>");
};

export default GameOfWeekComponent;