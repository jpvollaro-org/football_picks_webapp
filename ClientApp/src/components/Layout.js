import { Outlet } from "react-router-dom";
import { NavMenu } from './NavMenu';
import { Container } from 'reactstrap';


const Layout = () => {
   return (
		<Container>
			<NavMenu />
         <Outlet />
      </Container>
   )
};

export default Layout;
