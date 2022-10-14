import React, { useState } from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./components/Layout";
import PlayerPicksRoot from './components/PlayerPicks_root';
import PlayerModalComponent from './components/PlayerModalComponent';
import StandingsRoot from './components/Standings_root';
import SelectionsRoot from './components/Selections_root';

import AppContext from './store/app-context';
import 'bootstrap/dist/css/bootstrap.css';

export default function App() {
	const [selectedPlayerId, setSelectedPlayerId] = useState(1);
	const getSelectedPlayerId = () => {
		return selectedPlayerId;
	}

	const [modalIsDisplayed, setModalIsDispalyed] = useState(false);
	const showModalHandler = (selectedPlayerId) => {
		setSelectedPlayerId(selectedPlayerId);
		setModalIsDispalyed(true);
	}
	const hideModalHandler = () => {
		setModalIsDispalyed(false);
	}

	return (
		<AppContext.Provider value={{selectedPlayerId}}>
			<BrowserRouter>
				{modalIsDisplayed && <PlayerModalComponent onCloseHandler={hideModalHandler} getPlayerIdHandler={getSelectedPlayerId} />}
				<Routes>
					<Route path="/" element={<Layout/>}>
						<Route index element={<PlayerPicksRoot onShowModalHandler={showModalHandler} />} />
						<Route path="Selections" element={<SelectionsRoot onShowModalHandler={showModalHandler} />} />
						<Route path="Standings" element={<StandingsRoot onShowModalHandler={showModalHandler} />} />
					</Route>
				</Routes>
			</BrowserRouter>
		</AppContext.Provider>
	);
}

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(<App />);
