import React, { useState, useEffect } from 'react';
import useInput from '../hooks/use-input';
import useHttps from '../hooks/use-https';
import SelectionOptionComponent from "./SelectionOptionComponent";
import GameOfWeekComponent from './GameOfWeekComponent';
import TableComponent from './TableComponent';

const SelectionsComponent = (props) =>
{
	let returnValue = { playerKey: "", thursdayWinner: "", sundayWinner: "", mondayWinner: "", awayTeamScore: 0, homeTeamScore: 0 };

	const [data, setData] = useState([]);
	const transformData = ((incomingData) => {
		setData(incomingData);
	});

	const { isLoading, error, sendRequestToFetch: sendToController } = useHttps();
	useEffect(() => {
		var urlString = "api/ReactProgram/GetWeeklySelections";
		sendToController({ url: urlString }, transformData);
		// eslint-disable-next-line
	}, []);

	const resultHandler = (event) => {
		console.log(event);
		if (event.tabIndex === 1)
			returnValue.thursdayWinner = event.label;
		else if (event.tabIndex === 2)
			returnValue.sundayWinner = event.label;
		else if (event.tabIndex === 3)
			returnValue.mondayWinner = event.label;
	}

	const setScore = (value) => {
		returnValue.awayTeamScore = value;
	}

	const {
		value: enteredPlayerKey,
		isValid: playerKeyIsValid,
		hasError: playerKeyHasError,
		valueChangeHandler: playerKeyChangeHandler,
	} = useInput(value => value.trim() !== '');

	const UpdateSpreadsheetWithFileHandler = (event) => {
		event.preventDefault();
			//var urlString = "api/RegressionClaims/UpdateSpreadsheetWithFile?filepath=" + enteredFilepath
			//	+ "&filename=" + enteredFilename
			//	+ "&allowDuplicates=" + allowDuplicates;
			//sendToClaimsController({ url: urlString, method: 'Put' }, props.onReturnFunction);
	};

	if (isLoading) {
		return (
			<section>
				<p>Loading...</p>
			</section>
		);
	}

	if (error) {
		return (
			<section>
				<p>{error}</p>
			</section>
		);
	}

	return (
		<div>
			<div className="container">
				<div className="row">
					<div className="col-sm-2">
						<p>PlayerKey</p>
					</div>
					<div className="col-sm-3">
						<input type='text' id='playerKey' onChange={playerKeyChangeHandler} value={enteredPlayerKey}	/>
					</div>
				</div>
			</div>
			<br/>
			<SelectionOptionComponent label='Thursday:' gameScore={data[0]} changeHandler={resultHandler} tabIndex={1} />
			<SelectionOptionComponent label='Sunday:' gameScore={data[1]} changeHandler={resultHandler} tabIndex={2} />
			<SelectionOptionComponent label='Monday:' gameScore={data[2]} changeHandler={resultHandler} tabIndex={3} />
			<br />
			<GameOfWeekComponent gameScore={data[3]} side="away" teamScore={returnValue.awayScore} SetScore={setScore} />
			<GameOfWeekComponent gameScore={data[3]} side="home" teamScore={returnValue.homeScore} SetScore={setScore} />
			<div className="container">
				<br />
				<div className="row">
					<div className="col-sm-2" />
					<div className="col-sm-4">
						<button onClick={UpdateSpreadsheetWithFileHandler}>Submit</button>
					</div>
				</div>
			</div>
		</div>
	);
};

export default SelectionsComponent;