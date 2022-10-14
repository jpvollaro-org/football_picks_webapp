import PlayerPicksComponent from './PlayerPicksComponent';

const PlayerPicksRoot = (props) => {
   return (
      <div>
         <PlayerPicksComponent onShowModalHandler={props.onShowModalHandler} />
      </div>
   );
};

export default PlayerPicksRoot;