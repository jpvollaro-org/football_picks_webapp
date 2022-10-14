import { useState } from 'react';
import useKeyPress from '../hooks/use-keyPress';
import { FaSearch } from 'react-icons/fa';

const SearchComponent = (props) => {
	const [searchText, setSearchText] = useState("");
	const [searchState, setSearchState] = useState(false);

	const updateSearchTextHandler = ((event) => {
		event.preventDefault();
		setSearchText(event.target.value);
		props.SetSearchValue(event.target.value);
	});

	const toggleSearchState = (event) => {
		setSearchState(searchState !== true);
	}

	useKeyPress(['~'], toggleSearchState);
	let searchComponentText;
	if (searchState) {
		searchComponentText =
			<div align="right" >
				<input type="text" id="searchTextId" value={searchText} onChange={updateSearchTextHandler} />
				&nbsp;
				<FaSearch onClick={props.SearchMethodHandler} />
			</div>;
	}
	else { 
		searchComponentText = <div/>;
	}
	return searchComponentText;
};

export default SearchComponent;