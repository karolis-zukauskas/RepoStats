import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import "../App.css";

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor (props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true,
      isLoggedIn: false,
    };
  }

  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  async doLogin() {
    return;

    // this.setState({ ...this.state, isLoggedIn: true });
    // const qParams = [
    //   "client_id=67383de9ff87ee12f691",
    //   `redirect_uri=http://localhost:53752/onlogin`,
    //   `scope=notifications`,
    //   `state=RepoStats`
    // ].join("&");

    // try {
    //   window.open(`https://github.com/login/oauth/authorize?${qParams}`, '_self');
    // } catch (e) {
    //   console.error(e);
    // }
  }

  render () {
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
          <Container>
            <NavbarBrand tag={Link} to="/">RepoStats</NavbarBrand>
            <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
            <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
              <ul className="navbar-nav flex-grow">
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/">Home</NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/settings">Settings</NavLink>
                </NavItem>
              </ul>
            </Collapse>
            <button
              class="btn-primary"
              style={{ marginLeft: "64px" }}
              onClick={this.doLogin.bind(this)}
            >
              {this.state.isLoggedIn ? "USER" : "Login with GitHub"}
            </button>
          </Container>
        </Navbar>
      </header>
    );
  }
}
