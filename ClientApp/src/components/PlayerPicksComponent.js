import React, { useMemo, useState, useEffect } from "react";
import TableComponent from './TableComponent';
import useHttps from '../hooks/use-https';
import myStyles from "./PlayerPicksComponent.module.css";
import Select from 'react-select'

let winningPointDifference = 400;
let selectedGameString = '';

// Custom component to render Picks 
const Picks = ({ values }) => {
   return (
      <>
         {values.map((pick, idx) => {
            if (pick.winning === true)
               return (
                  <span key={idx} className={myStyles.pickWinning} >
                     {pick.pickString} <br/>
                  </span>
               );
            else if (pick.losing === true)
               return (
                  <span key={idx} className={myStyles.pickLosing} >
                     {pick.pickString} <br />
                  </span>
               );
            else if (pick.gofWeek === true && pick.pointDifferences === winningPointDifference)
               return (
                  <span key={idx} className={myStyles.pickWinning} >
                     {pick.pickString} <br />
                  </span>
               );
            else
               return (
                  <span key={idx} className={myStyles.pickTied} >
                     {pick.pickString} <br />
                  </span>
               );
         })}
      </>
   );
};

const TeamLogo = ({ values, props }) => {
   const theNameSlectedClick = (event) => {
      props.onShowModalHandler(event.currentTarget.tabIndex);
   } 

   return (
      <>
         {values.map((combo, idx) => {
            if (idx === 0)
               return (
                  <span key={idx} tabIndex={parseInt(values[2])} onClick={theNameSlectedClick} >
                     {combo} <br />
                  </span>
               );
            else if (idx === 1)
               return (
                  <span key={idx} >
                     <img src={combo} width="50" height="50" tabIndex={parseInt(values[2])} alt="FavoriteTeam?" onClick={theNameSlectedClick} />
                  </span>
               )
            else
               return null;
         })}
      </>
   );
};

const Points = ({ values }) => {
   if (values === 300)
      return (<>_</>);
   else
      return (<>{values}</>);
};

const PlayerPicksComponent = (props) => { 
   const [data, setData] = useState({ players: [] });

   const transformData = ((incomingData) => {
      setData(incomingData);
      winningPointDifference = incomingData.lowestPointDifference;
   });

   const { isLoading, error, sendRequestToFetch: getProgramData } = useHttps();
   useEffect(() => {
      var urlString = "api/ReactProgram/getProgramData?pickFilter=" + selectedGameString;
      getProgramData({ url: urlString }, transformData);
      // eslint-disable-next-line
   }, [selectedGameString]);


   const columns = useMemo(
      () => [
         {
            Header: "WEEKLY PICKS",
            columns: [
               {
                  Header: "Name",
                  accessor: "nameTeamCombo",
                  Cell: ({ cell: { value } }) => <TeamLogo values={value} props={props} />
               },
               {
                  Header: "Picks(s)",
                  accessor: "spreadsheetPicks",
                  Cell: ({ cell: { value } }) => <Picks values={value} />
               },
               {
                  Header: "Current",
                  accessor: "currentPlayerPoints"
               },
               {
                  Header: "ScoreDiff",
                  accessor: "gameOfWeekDifference",
                  Cell: ({ cell: { value } }) => <Points values={value} />

               }
            ]
         }
      ],
      [props]
   );

   const [selectedFromUI, setSelectedFromUI] = useState('');
   const [releaseChoices, setReleaseChoices] = useState([]);
   const [selectedOptions, setSelectedOptions] = useState([]);

   const updateSelections = (choices) => {
      setReleaseChoices(choices);
   };
   useEffect(() => {
      getProgramData({ url: "api/ReactProgram/getGameOfWeekChoices" }, updateSelections);
      // eslint-disable-next-line
   }, []);
   const handleChange = (options) => {
      setSelectedOptions(options);
      let tmp = '';
      let index = 0;
      for (; index < options.length; index++) {
         tmp += options[index].value;
         if (index + 1 < options.length)
            tmp += " , ";
      }
      setSelectedFromUI(tmp);
      selectedGameString = tmp;
   };

   if (isLoading) {
      return (
         <section className={myStyles.Loading}>
            <p>Loading...</p>
         </section>
      );
   }

   if (error) {
      return (
         <section className={myStyles.Error}>
            <p>{error}</p>
         </section>
      );
   }

   return (
      <div>
         <p className={myStyles.cssfix}>{data.scoreLine}</p>
         <Select
            isMulti
            name="releases"
            options={releaseChoices}
            className="basic-multi-select"
            classNamePrefix="select"
            isLoading={isLoading}
            isSearchable={true}
            closeMenuOnSelect={false}
            onChange={handleChange}
            selectResetinput={false}
         />
{/*         <Button onClick={doNothingHandler}></Button>*/}

         <div className={myStyles.table}>
            <TableComponent columns={columns} data={data.players} />
         </div>
      </div>
   );
}

export default PlayerPicksComponent;