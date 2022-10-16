import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { LoginMenu } from '../api-authorization/LoginMenu';
import '../NavMenu.css';
import White from '../common/White';
// import "../common/PwCss.css"
// import '../../public/PW-logomaster.jpg';

interface NavMenuState {
  collapsed: boolean;
}

interface NavMenuProps {
  children?: JSX.Element | String | null | Number | undefined;
}

export default class NavMenu extends Component<NavMenuProps, NavMenuState> {
  static displayName = NavMenu.name;

  constructor (props: NavMenuProps | Readonly<NavMenuProps>) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true
    } as NavMenuState;
  }

  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render () {
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm bg-purple-admin border-bottom box-shadow mb-3" light>
          <Container>
            <NavbarBrand tag={Link} to="/">
              <img src={process.env.PUBLIC_URL + "/PW-logomaster.png"} alt="logo" className="logoImg"/>
            </NavbarBrand>
            <h1>
                  <White>
                      Admin
                  </White>
              </h1>
            <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
            <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
              <ul className="navbar-nav flex-grow">
                <NavItem>
                  <NavLink tag={Link} className="white" to="/">Home</NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="white" to="/counter">Help tickets</NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="white" to="/fetch-data">Source control</NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="white" to="/fetch-data">User control</NavLink>
                </NavItem>
                <LoginMenu>
                </LoginMenu>
              </ul>
            </Collapse>
          </Container>
        </Navbar>
      </header>
    );
  }
}
