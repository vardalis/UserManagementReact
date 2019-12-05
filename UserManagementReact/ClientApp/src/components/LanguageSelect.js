import React, { Component } from 'react';
import { withTranslation } from 'react-i18next';
import { UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';

export class LanguageSelectPlain extends Component {
	// static displayName = NavMenu.name;

	constructor(props) {
		super(props);

		const { i18n } = this.props;
		// this.toggleNavbar = this.toggleNavbar.bind(this);
		this.state = {
			lang: i18n.language,
			collapsed: true
		};
	}



	toggleNavbar() {
		this.setState({
			collapsed: !this.state.collapsed
		});
	}

	handleLanguageChange = (lang) => {
		const { i18n } = this.props;
		i18n.changeLanguage(lang);
		this.setState({ lang: lang });
	}

	render() {
		const { t, i18n } = this.props;

		return (
			<UncontrolledDropdown nav inNavbar>
				<DropdownToggle nav caret>
					{ t(this.state.lang) }
				</DropdownToggle>
				<DropdownMenu right>
					<DropdownItem onClick={this.handleLanguageChange.bind(this, "en")}>
						English
					</DropdownItem>
					<DropdownItem onClick={this.handleLanguageChange.bind(this, "el")}>
						Ελληνικά
					</DropdownItem>
				</DropdownMenu>
			</UncontrolledDropdown>
		);
	}
}

export const LanguageSelect = withTranslation()(LanguageSelectPlain)
