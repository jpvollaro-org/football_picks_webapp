import React, { useState, useEffect, useMemo } from 'react';
import TableComponent from './TableComponent';
import Modal from "../UI/Modal";
import useHttps from '../hooks/use-https';
import myStyles from "./PlayerModalComponent.module.css";

const PlayerModalComponent = (props) => {
	const [tabularPlayerData, setTabularPlayerData] = useState({ weeklyData:[]});
	const transformPlayerData = ((incomingData) => {
		setTabularPlayerData(incomingData);
	});

	const { isLoading, error, sendRequestToFetch: sendToController } = useHttps();
	const playerId = props.getPlayerIdHandler();
	useEffect(() => {
		var urlString = "api/ReactProgram/GetPlayerData?playerId=" + playerId;
		sendToController({ url: urlString }, transformPlayerData);
		// eslint-disable-next-line
	}, []);

	const columns = useMemo(
		() => [
			{
				Header: "--------------",
				columns: [
					{
						Header: "pts",
						accessor: "points"
					},
					{
						Header: "Picks",
						accessor: "picks",
					}
				]
			}
		],
		[]
	);

	if (isLoading) {
		return (
			<Modal onClose={props.onCloseHandler}>
				<p>Loading...</p>
			</Modal>
		);
	}

	if (error) {
		return (
			<Modal onClose={props.onCloseHandler}>
				<p>{error}</p>
			</Modal>
		);
	}

	return (
		<Modal onClose={props.onCloseHandler}>
			<div>
				<p>{tabularPlayerData.playerName}</p>
				<div className={myStyles.table}>
					<TableComponent className={myStyles.table1} columns={columns} data={tabularPlayerData.weeklyData} />
				</div>
				<p><b>--------------</b></p>
				<p><b>{tabularPlayerData.totalPoints}</b><pre>Weekly:{tabularPlayerData.weeklyPoints} Bonus:{tabularPlayerData.bonusPoints}</pre></p>
			</div>
		</Modal>
	);
};

export default PlayerModalComponent;