import React, { Component } from 'react';
import usersService from './UsersService';
import { AvForm, AvField } from 'availity-reactstrap-validation';
import { FormGroup, Form, Label, Input, Button } from 'reactstrap';
import { withTranslation } from 'react-i18next';

class UsersAddPlain extends Component {

	constructor(props) {
		super(props);
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
			await usersService.addUser(values);
			history.push('/users');
		})();
	}

	render() {
		const { t, i18n } = this.props;
		return (
			<AvForm onValidSubmit={this.handleValidSubmit}>
				<AvField name="firstName" label={t('FirstName')} required errorMessage={t('FieldInvalid')} validate={{
					required: { value: true, errorMessage: t('FieldRequired') },
					minLength: { value: 2 }
				}} />
				<AvField name="lastName" label="Last name" required />
				<AvField name="email" type="email" label="Email" required />
				<AvField name="role" type="select" label="Role" required>
					<option value="">---Select Value---</option>
					<option value="administrator">Administrator</option>
					<option value="supervisor">Supervisor</option>
					<option value="user">User</option>
				</AvField>
				<AvField name="password" type="password" label="Password" required />
				<AvField name="confirmPassword" type="password" label="Confirm Password" required
					validate={{ match: { value: 'password' } }}
				/>
				<FormGroup>
					<Button>Save</Button>&nbsp;
					<Button onClick={this.handleClickCancel}>Cancel</Button>
				</FormGroup>
			</AvForm>
		);
	}
}

export const UsersAdd = withTranslation()(UsersAddPlain);

