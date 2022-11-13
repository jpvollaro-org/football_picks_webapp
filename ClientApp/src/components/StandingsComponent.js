import React, { useMemo, useState, useEffect } from "react";
import TableComponent from './TableComponent';
import useHttps from '../hooks/use-https';
import myStyles from "./StandingsComponent.module.css";
import AdminComponent from './AdminComponent';

const StandingsComponent = () => { 
   const [data, setData] = useState([]);

   const transformData = ((incomingData) => {
      setData(incomingData);
   });

   const { isLoading, error, sendRequestToFetch: sendToController } = useHttps();
   useEffect(() => {
      var urlString = "api/ReactProgram/GetPlayerStandings";
      sendToController({ url: urlString }, transformData);
      // eslint-disable-next-line
   }, []);

   const Picks = ({ values }) => {
      return (
         <>
            {values.map((pick, idx) => {
               return (
                  <span key={idx}>{pick.pickString}, </span>
               );
            })}
         </>
      );
   };
   const columns = useMemo(
      () => [
         {
            Header: "Standings",
            columns: [
               {
                  Header: "Name",
                  accessor: "name"
               },
               {
                  Header: "Points",
                  accessor: "currentPlayerPoints"
               },
               {
                  Header: "Picks",
                  accessor: "spreadsheetPicks",
                  Cell: ({ cell: { value } }) => <Picks values={value} />
					}
            ]
         }
      ],
      []
   );

   if (isLoading) {
      return (
            <p>Loading...</p>
      );
   }

   if (error) {
      return (
            <p>{error}</p>
      );
   }

   return (
      <div className={myStyles.table }>
         <TableComponent className={myStyles.table1} columns={columns} data={data} />
         <AdminComponent/>
      </div>
   );
}

export default StandingsComponent;