import React, { useState } from 'react';
import Select from 'react-select';
import useInput from '../hooks/use-input';
import useHttps from '../hooks/use-https';

const SelectionsComponent = (props) => {

	const { sendRequestToFetch: sendToClaimsController } = useHttps();

	const thursdayNightGame = [
		{ label: "COMMANDERS", value: 1 },
		{ label: "BEARS", value: 2 },
	];

	const sundayNightGame = [
		{ label: "BILLS", value: 1 },
		{ label: "CHIEFS", value: 2 },
	];

	const mondayNightGame = [
		{ label: "BRONCOS", value: 1 },
		{ label: "CHARGERS", value: 2 },
	];

	let awayTeam = "COWBOYS"
	let homeTeam = "EAGLES"
	const [awayScore, setAwayScore] = useState(0);
	const [homeScore, setHomeScore] = useState(0);

	const [returnValue, setReturnValue] = useState({ playerKey:"", thursdayWinner:"", sundayWinner:"", mondayWinner:"", awayTeamScore:0, homeTeamScore:0 });

	const {
		value: enteredPlayerKey,
		isValid: playerKeyIsValid,
		hasError: playerKeyHasError,
		valueChangeHandler: playerKeyChangeHandler,
	} = useInput(value => value.trim() !== '');

	let formIsValid = false
	if (playerKeyIsValid) {
		formIsValid = true;
	}

	const [thursdayWinner, setThursdayWinner] = useState("");
	const ThursdayGameHandler = (event) => {
		setThursdayWinner(event.label);
	}

	const [sundayWinner, setSundayWinner] = useState("");
	const SundayGameHandler = (event) => {
		setSundayWinner(event.label);
	}

	const [mondayWinner, setMondayWinner] = useState("");
	const MondayGameHandler = (event) => {
		setMondayWinner(event.label);
	}

	const UpdateSpreadsheetWithFileHandler = (event) => {
		event.preventDefault();
		setReturnValue({
			playerKey: {enteredPlayerKey},
			thursdayWinner: { thursdayWinner },
			sundayWinner: { sundayWinner },
			mondayWinner: { mondayWinner },
			awayTeamScore: { awayScore },
			homeTeamScore: { homeScore }
		});
		if (formIsValid) {
			//var urlString = "api/RegressionClaims/UpdateSpreadsheetWithFile?filepath=" + enteredFilepath
			//	+ "&filename=" + enteredFilename
			//	+ "&allowDuplicates=" + allowDuplicates;
			//sendToClaimsController({ url: urlString, method: 'Put' }, props.onReturnFunction);
		}
	};

	const nameInputClasses = playerKeyHasError ? 'form-control invalid' : 'form-control';
	return (
		<form onSubmit={UpdateSpreadsheetWithFileHandler}>
			<div className="container">
				<div className="row">
					<div className="col-sm-2">
						<p>PlayerKey</p>
					</div>
					<div className="col-sm-4">
						<input
							type='text'
							id='playerKey'
							onChange={playerKeyChangeHandler}
							value={enteredPlayerKey}
						/>
					</div>
					<div className="col-sm-2">
						<button onClick={UpdateSpreadsheetWithFileHandler}>Submit</button>
					</div>
				</div>
			</div>
			<br/>

			<div className="container">
				<div className="row">
					<div className="col-sm-2">
						<p>Thursday:</p>
					</div>
					<div className="col-sm-4">
						<Select options={thursdayNightGame} onChange={ThursdayGameHandler} />
					</div>
				</div>
			</div>
			<br />

			<div className="container">
				<div className="row">
					<div className="col-sm-2">
						<p>{awayTeam}</p>
					</div>
					<div className="col-sm-2">
						<input
							type="text"
							pattern="[0-9]*"
							value={awayScore}
							onChange={(e) =>
								setAwayScore((v) => (e.target.validity.valid ? e.target.value : v))
							}
						/>
					</div>
				</div>
			</div>

			<div className="container">
				<div className="row">
					<div className="col-sm-2">
						<p>{homeTeam}</p>
					</div>
					<div className="col-sm-1">
						<input
							type="text"
							pattern="[0-9]*"
							value={homeScore}
							onChange={(e) =>
								setHomeScore((v) => (e.target.validity.valid ? e.target.value : v))
							}
						/>
					</div>
				</div>
			</div>
			<br />

			<div className="container">
				<div className="row">
					<div className="col-sm-2">
						<p>Sunday:</p>
					</div>
					<div className="col-sm-4">
						<Select options={sundayNightGame} onChange={SundayGameHandler} />
					</div>
				</div>
			</div>
			<br />

			<div className="container">
				<div className="row">
					<div className="col-sm-2">
						<p>Monday:</p>
					</div>
					<div className="col-sm-4">
						<Select options={mondayNightGame} onChange={MondayGameHandler}/>
					</div>
				</div>
			</div>
		</form>
	);
};

export default SelectionsComponent;