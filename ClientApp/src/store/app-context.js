import { createContext } from "react";

const AppContext = createContext({
   numberOfClaims: 0,
   numberOfMismatches: 0,
   claimName: '',
   rowNumber: 0,
   loading: false,
   errorMessage: ''
});

export default AppContext;