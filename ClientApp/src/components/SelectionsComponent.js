import React, { useState, useEffect } from 'react';
import useInput from '../hooks/use-input';
import useHttps from '../hooks/use-https';
import SelectionOptionComponent from "./SelectionOptionComponent";
import GameOfWeekComponent from './GameOfWeekComponent';

const SelectionsComponent = (props) => {
	let playerSelections = [{}, {}, {}, {}, {}, {}, {}, {}, {}, {}, {}, {}, {}, {}, {}, {}]

	const [responseMessage, setResponseMessage] = useState("");

	const [data, setData] = useState([]);
	const [dataGOW, setDataGOW] = useState([]);
	const transformData = ((incomingData) => {
		setData(incomingData.standardGames);
		setDataGOW(incomingData.gofWeekGames);
	});

	const { isLoading, error, sendRequestToFetch: sendToController } = useHttps();
	useEffect(() => {
		var urlString = "api/ReactProgram/GetWeeklySelections";
		sendToController({ url: urlString }, transformData);
		// eslint-disable-next-line
	}, []);

	const resultHandler = (event) => {
		if (event.label === event.lookupKey) {
			playerSelections[event.tabIndex].homeTeam = event.lookupKey;
			playerSelections[event.tabIndex].homeScore = 100;
			playerSelections[event.tabIndex].awayScore = 0;
			playerSelections[event.tabIndex].awayTeam = event.opp;
		}
		else {
			playerSelections[event.tabIndex].homeTeam = event.lookupKey;
			playerSelections[event.tabIndex].homeScore = 0;
			playerSelections[event.tabIndex].awayScore = 100;
			playerSelections[event.tabIndex].awayTeam = event.label;
		}
	}

	const setScore = (e) => {
		if (e.target.title === "home score") {
			const v = playerSelections[e.target.tabIndex].homeScore;
			playerSelections[e.target.tabIndex].homeScore = e.target.validity.valid ? e.target.value : v
			playerSelections[e.target.tabIndex].homeTeam = e.target.alt;
		}
		else {
			const v = playerSelections[e.target.tabIndex].awayScore;
			playerSelections[e.target.tabIndex].awayScore = e.target.validity.valid ? e.target.value : v
			playerSelections[e.target.tabIndex].awayTeam = e.target.alt;
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
		sendToController({ url: urlString, method: 'Put', headers: headersValues, body: playerSelections }, selectionResponseHandler);
	};

	const SelectionComponents = ({ weeklySelections }) => (
		<div>
			{weeklySelections.map(weeklySelectedItem => (
				<div key={weeklySelectedItem.id}>
					<br />
					<SelectionOptionComponent label={weeklySelectedItem.label} gameScore={weeklySelectedItem} changeHandler={resultHandler} tabIndex={weeklySelectedItem.selectionNumber} />
				</div>
			))}
		</div>
	); 

	const GofWeekComponents = ({ gameOfWeekSelections }) => (
		<div>
			{gameOfWeekSelections.map(gofwSelectedItem => (
				<div key={gofwSelectedItem.id}>
					<br />
					<GameOfWeekComponent gameScore={gofwSelectedItem} side="away score" teamScore={playerSelections[gofwSelectedItem.selectionNumber].awayScore} SetScore={setScore} tabIndex={gofwSelectedItem.selectionNumber} />
					<GameOfWeekComponent gameScore={gofwSelectedItem} side="home score" teamScore={playerSelections[gofwSelectedItem.selectionNumber].homeScore} SetScore={setScore} tabIndex={gofwSelectedItem.selectionNumber} />
				</div>
			))}
		</div>
	);

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
			<div>
				<SelectionComponents weeklySelections={data} />
			</div>
			<div>
				<GofWeekComponents gameOfWeekSelections={dataGOW} />
			</div>
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