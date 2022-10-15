import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import Button from '../UI/Button';
import { GiAmericanFootballHelmet, GiAmericanFootballBall } from 'react-icons/gi'
import { FaRegSun, FaFileDownload } from 'react-icons/fa';

export class NavMenu extends Component {
	static displayName = NavMenu.name;

	constructor(props) {
		super(props);

		this.toggleNavbar = this.toggleNavbar.bind(this);
		this.state = {
			collapsed: true
		};
	}

	toggleNavbar() {
		this.setState({
			collapsed: !this.state.collapsed
		});
	}

	render() {

		return (
			<header>
				<Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
					<Container>
						<NavbarBrand tag={Link} to="/">
							<GiAmericanFootballHelmet/><b>FOOTBALL Picks</b><GiAmericanFootballBall />       
						</NavbarBrand>
						<NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
						<Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
							<ul className="navbar-nav flex-grow">
								<NavItem>
									<NavLink tag={Link} className="text-dark" to="/">Picks</NavLink>
								</NavItem>
								<NavItem>
									<NavLink tag={Link} className="text-dark" to="/Selections">Selections</NavLink>
								</NavItem>
								<NavItem>
									<NavLink tag={Link} className="text-dark" to="/Standings">Standings</NavLink>
								</NavItem>
							</ul>
						</Collapse>
					</Container>
				</Navbar>
			</header>
		);
	}
}