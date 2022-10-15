import React, { useState, useEffect } from 'react';
import useInput from '../hooks/use-input'

const GameOfWeekComponent = (props) =>
{
	if (props.gameScore) {
		let displayTeam = ''
		if (props.side === 'away')
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
								type="text"
								pattern="[0-9]*"
								value={props.teamScore}
								onChange={(e) =>
									props.SetScore((v) => (e.target.validity.valid ? e.target.value : v))
								}
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