import React, { Component } from 'react';
import usersService from './UsersService';
import { AvForm, AvField } from 'availity-reactstrap-validation';
import { FormGroup, Form, Label, Input, Button } from 'reactstrap';
import { withTranslation } from 'react-i18next';

class UsersPasswordChangePlain extends Component {

	constructor(props) {
		super(props);

		const { match } = this.props;
		this.userId = match.params.userId;
	}

	componentDidMount() {
	}

	handleClickCancel = () => {
		const { history } = this.props;

		history.push('/users');
	}

	handleValidSubmit = (event, values) => {
		const { history } = this.props;

		(async () => {
			await usersService.changeUserPassword(this.userId, values);
			history.push('/users');
		})();
	}

	render() {
		const { t, i18n } = this.props;
		return (
			<AvForm onValidSubmit={this.handleValidSubmit}>
				<AvField name="password" type="password" label="Password" required />
				<AvField name="confirmPassword" type="password" label="Confirm Password" required
					validate={{ match: { value: 'password' } }}
				/>
				<FormGroup>
					<Button>Submit</Button>&nbsp;
					<Button onClick={this.handleClickCancel}>Cancel</Button>
				</FormGroup>
			</AvForm>
		);
	}
}

export const UsersPasswordChange = withTranslation()(UsersPasswordChangePlain);

