import React, { useRef, useState, useEffect } from 'react';
import Button from '../UI/Button';
import Input from "../UI/Input";
import Modal from "../UI/Modal";
import useHttps from '../hooks/use-https';
import styles from "./PlayerModalComponent.module.css";


const PlayerModalComponent = (props) => {
	const [ playerData, setPlayerData] = useState("NONE");
	const { isLoading, error, sendRequestToFetch: sendToController } = useHttps();
	const transformPlayerData = ((incomingData) => {
		setPlayerData(incomingData.playerPicks);
	});
	const playerId = props.getPlayerIdHandler();
	useEffect(() => {
		var urlString = "api/ReactProgram/GetPlayerData?playerId=" + playerId;
		sendToController({ url: urlString }, transformPlayerData);
		// eslint-disable-next-line
	}, []);

	if (isLoading) {
		return (
			<Modal onClose={props.onCloseHandler}>
				<p>Loading...</p>
			</Modal>
		);
	}

	return (
		<Modal onClose={props.onCloseHandler}>
			<span className={styles.playerData}><p>{playerData}</p></span>
		</Modal>
	);
};

export default PlayerModalComponent;