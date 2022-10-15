import Select from 'react-select';

const SelectionOptionComponent = (props) =>
{
	const setOption = (gameScore, uiIndex) => {
		if (gameScore) {
			let awayTeamLabel = gameScore.awayTeam;
			let awayTeamOption = { label: awayTeamLabel, value: 1, tabIndex: uiIndex };
			let homeTeamLabel = gameScore.homeTeam;
			let homeTeamOption = { label: homeTeamLabel, value: 2, tabIndex: uiIndex };
			let myOptions = [awayTeamOption, homeTeamOption];
			return myOptions;
		}
		else
			return [];

	}

	const teamOptions = setOption(props.gameScore, props.tabIndex);
	return (
		<div className="container">
			<div className="row">
				<div className="col-sm-2">
					<p>{props.label}</p>
				</div>
				<div className="col-sm-4">
					<Select options={teamOptions} onChange={props.changeHandler} tabIndex={props.tabIndex} />
				</div>
			</div>
		</div>
	);
};

export default SelectionOptionComponent;