import React, { Component } from 'react';
import { withRouter } from 'react-router';
import usersService from './UsersService';
import { AvForm, AvField } from 'availity-reactstrap-validation';
import { FormGroup, Form, Label, Input, Button } from 'reactstrap';
import { withTranslation } from 'react-i18next';


class UsersEditPlain extends Component {
	constructor(props) {
		super(props);
		this.state = { user: null, loading: true };

		const { match } = this.props;
		this.userId = match.params.userId;
	}

	componentDidMount() {
		this.retrieveFormData();
	}

	handleInputChange = (event) => {
		const target = event.target;
		const value = target.type === 'checkbox' ? target.checked : target.value;
		const name = target.name;

		this.state.user[name] = value;
		this.setState({ user: this.state.user });
	}

	handleClickCancel = () => {
		const { history } = this.props;

		history.push('/users');
	}

	handleValidSubmit = (event, values) => {
		const { history } = this.props;

		(async () => {
			await usersService.updateUser(this.userId, values);
			history.push('/users');
		})();
	}

	renderUserForm(user) {
		const { t, i18n } = this.props;
		return (
			<AvForm model={user} onValidSubmit={this.handleValidSubmit}>
				<AvField name="id" type="hidden" />
				<AvField name="rowVersion" type="hidden" />
				<AvField name="firstName" label={t('FirstName')} required errorMessage={t('FieldInvalid')} validate={{
					required: { value: true, errorMessage: t('FieldRequired') },
					minLength: { value: 2}
				}} />
				<AvField name="lastName" label="Last name" required />
				<AvField name="email" type="email" label="Email" required />
				<AvField name="role" type="select" label="Role" required>
					<option value="administrator">Administrator</option>
					<option value="supervisor">Supervisor</option>
					<option value="user">User</option>
				</AvField>
				<FormGroup>
					<Button>Save</Button>&nbsp;
					<Button onClick={this.handleClickCancel}>Cancel</Button>
				</FormGroup>
			</AvForm>
		);
	}

	render() {
		let contents = this.state.loading
			? <p><em>Loading...</em></p>
			: this.renderUserForm(this.state.user);

		return (
			<div>
				<h1 id="tabelLabel">Users</h1>
				{contents}
			</div>
		);
	}

	async retrieveFormData() {
		const data = await usersService.getUser(this.userId);
		this.setState({ user: data, loading: false });
	}
}

export const UsersEdit = withTranslation()(withRouter(UsersEditPlain));
