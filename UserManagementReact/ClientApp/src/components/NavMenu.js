import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { LoginMenu } from './api-authorization/LoginMenu';
import './NavMenu.css';
import { LanguageSelect } from './LanguageSelect';
import { withTranslation } from 'react-i18next';
import authService from './api-authorization/AuthorizeService'
import { UserRoles } from './api-authorization/ApiAuthorizationConstants';

class NavMenuPlain extends Component {
	static displayName = NavMenuPlain.name;

	constructor(props) {
		super(props);

		this.toggleNavbar = this.toggleNavbar.bind(this);
		this.state = {
			hasAdminRole: false,
			collapsed: true
		};
	}

	componentDidMount() {
		this._subscription = authService.subscribe(() => this.populateState());
		this.populateState();
	}

	async populateState() {
		const hasAdminRole = await authService.hasRole(UserRoles.Administrator);
		this.setState({
			hasAdminRole
		});
	}

	toggleNavbar() {
		this.setState({
			collapsed: !this.state.collapsed
		});
	}

	render() {
		const { t, i18n } = this.props;
		return (
			<header>
				<Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
					<Container>
						<NavbarBrand tag={Link} to="/">User Management React</NavbarBrand>
						<NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
						<Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
							<ul className="navbar-nav flex-grow">
								<NavItem>
									<NavLink tag={Link} className="text-dark" to="/">{t('Home')}</NavLink>
								</NavItem>
								<NavItem>
									<NavLink tag={Link} className="text-dark" to="/counter">{t('Counter')}</NavLink>
								</NavItem>
								<NavItem>
									<NavLink tag={Link} className="text-dark" to="/fetch-data">{t('Fetch data')}</NavLink>
								</NavItem>
								{
									this.state.hasAdminRole &&
									<NavItem>
										<NavLink tag={Link} className="text-dark" to="/users">{t('Users')}</NavLink>
									</NavItem>
								}
								<LoginMenu>
								</LoginMenu>
								<LanguageSelect>
								</LanguageSelect>
							</ul>
						</Collapse>
					</Container>
				</Navbar>
			</header>
		);
	}
}

export const NavMenu = withTranslation()(NavMenuPlain);