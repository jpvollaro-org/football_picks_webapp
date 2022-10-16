import React, { useState, useEffect } from 'react';
import useInput from '../hooks/use-input';
import useHttps from '../hooks/use-https';
import SelectionOptionComponent from "./SelectionOptionComponent";
import GameOfWeekComponent from './GameOfWeekComponent';

const SelectionsComponent = (props) => {
	let playerSelectionArray = [{}, {}, {}, {}, {} ]

	const [responseMessage, setResponseMessage] = useState("");

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
		if (event.label === event.lookupKey) {
			playerSelectionArray[event.tabIndex].homeTeam = event.lookupKey;
			playerSelectionArray[event.tabIndex].homeScore = 100;
			playerSelectionArray[event.tabIndex].awayScore = 0;
			playerSelectionArray[event.tabIndex].awayTeam = event.opp;
		}
		else {
			playerSelectionArray[event.tabIndex].homeTeam = event.lookupKey;
			playerSelectionArray[event.tabIndex].homeScore = 0;
			playerSelectionArray[event.tabIndex].awayScore = 100;
			playerSelectionArray[event.tabIndex].awayTeam = event.label;
		}
	}

	const setScore = (e) => {
		if (e.target.title === "home score") {
			const v = playerSelectionArray[e.target.tabIndex].homeScore;
			playerSelectionArray[e.target.tabIndex].homeScore = e.target.validity.valid ? e.target.value : v
			playerSelectionArray[e.target.tabIndex].homeTeam = e.target.alt;
		}
		else {
			const v = playerSelectionArray[e.target.tabIndex].awayScore;
			playerSelectionArray[e.target.tabIndex].awayScore = e.target.validity.valid ? e.target.value : v
			playerSelectionArray[e.target.tabIndex].awayTeam = e.target.alt;
		}
	}

	const {
		value: enteredPlayerKey,
		valueChangeHandler: playerKeyChangeHandler,
	} = useInput(value => value.trim() !== '');

	const selectionResponseHandler = (resp) => {
		setResponseMessage(resp.responseMessage);
	}

	const SubmitButtonHandler = (event) => {
		event.preventDefault();
		const headersValues = { 'content-type': 'application/json', 'playerKey': enteredPlayerKey };
		var urlString = "api/ReactProgram/SendPlayerWeeklySelections";
		sendToController({ url: urlString, method: 'Put', headers: headersValues, body: playerSelectionArray }, selectionResponseHandler);
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
			<SelectionOptionComponent label='Thursday:' gameScore={data[0]} changeHandler={resultHandler} tabIndex={0} />
			<SelectionOptionComponent label='Sunday:' gameScore={data[1]} changeHandler={resultHandler} tabIndex={1} />
			<SelectionOptionComponent label='Monday:' gameScore={data[2]} changeHandler={resultHandler} tabIndex={2} />
			<br />
			<GameOfWeekComponent gameScore={data[3]} side="away score" teamScore={playerSelectionArray[3].awayScore} SetScore={setScore} tabIndex={3} />
			<GameOfWeekComponent gameScore={data[3]} side="home score" teamScore={playerSelectionArray[3].homeScore} SetScore={setScore} tabIndex={3} />
			<div className="container">
				<br />
				<div className="row">
					<div className="col-sm-2" />
					<div className="col-sm-4">
						<button onClick={SubmitButtonHandler}>Submit</button>
						<p>{responseMessage}</p>
					</div>
				</div>
			</div>
		</div>
	);
};

export default SelectionsComponent;