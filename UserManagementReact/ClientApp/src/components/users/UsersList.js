import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import usersService from './UsersService';
import { Pager } from '../Pager';
import { FormGroup, Form, Input, Button } from 'reactstrap';

export class UsersList extends Component {
	constructor(props) {
		super(props);
		this.state = { users: [], page: 1, pageSize: 10, maxPages: 5, sortOrder: "Fname", searchString: "", loading: true };
	}

	componentDidMount() {
		this.populateUserData();
	}

	handlePageChange = (page) => {
		this.setState({ page: page }, () => this.populateUserData() );
	}

	handleHeaderClick = (event, header) => {
		event.preventDefault();

		let newSortOrder = this.state.sortOrder;

		switch (header) {
			case 'Fname': {
				newSortOrder = this.state.sortOrder === 'Fname' ? 'Fname_desc' : 'Fname';
				break;
			}
			case 'Lname': {
				newSortOrder = this.state.sortOrder === 'Lname' ? 'Lname_desc' : 'Lname';
				break;
			}
			case 'Email': {
				newSortOrder = this.state.sortOrder === 'Email' ? 'Email_desc' : 'Email';
				break;
			}
		}

		this.setState({ page: 1, sortOrder: newSortOrder }, () => this.populateUserData());
		return false;
	}

	handleInputChange = (event) => {
		const target = event.target;
		const value = target.type === 'checkbox' ? target.checked : target.value;
		const name = target.name;

		this.setState({ [name]: value });
	}

	handleSearchFormSubmit = (event) => {
		event.preventDefault();
		this.populateUserData();
	}

	handleSearchFormReset = (event) => {
		event.preventDefault();
		this.setState({ page:1, searchString: "" }, () => this.populateUserData());
	}

	renderUsersTable() {
		const { users, totalUsers, sortOrder } = this.state;

		return (
			<div>
				<table className='table table-striped' aria-labelledby="tableLabel">
					<thead>
						<tr>
							<th>
								Id
							</th>
							<th>
								<a href="#" onClick={(e) => this.handleHeaderClick(e, 'Fname')} >
									First Name
									{sortOrder == 'Fname' && <span>&#8897;</span>}
									{sortOrder == 'Fname_desc' && <span>&#8896;</span>}
								</a>
							</th>
							<th>
								<a href="#" onClick={(e) => this.handleHeaderClick(e, 'Lname')} >
									Last Name
									{sortOrder == 'Lname' && <span>&#8897;</span>}
									{sortOrder == 'Lname_desc' && <span>&#8896;</span>}
								</a>
							</th>
							<th>
								<a href="#" onClick={(e) => this.handleHeaderClick(e, 'Email')} >
									Email
									{sortOrder == 'Email' && <span>&#8897;</span>}
									{sortOrder == 'Email_desc' && <span>&#8896;</span>}
								</a>
							</th>
							<th />
							<th />
							<th />
						</tr>
					</thead>
					<tbody>
						{users.map(user =>
							<tr key={user.id}>
								<td>{user.id}</td>
								<td>{user.firstName}</td>
								<td>{user.lastName}</td>
								<td>{user.email}</td>
								<td><Link to={'/users/delete/' + user.id}>Delete</Link></td>
								<td><Link to={'/users/edit/' + user.id}>Edit</Link></td>
								<td><Link to={'/users/password-change/' + user.id}>Change Password</Link></td>
							</tr>
						)}
					</tbody>
				</table>
				<Pager totalItems={totalUsers} page={this.state.page} pageSize={this.state.pageSize} maxPages={this.state.maxPages} handlePageChange={this.handlePageChange} />
			</div>
		);
	}

	render() {
		let contents = this.state.loading
			? <p><em>Loading...</em></p>
			: this.renderUsersTable();

		return (
			<div>
				<h1 id="tableLabel">Users</h1>
				<Link to='/users/add/'>Add User</Link>
				<Form inline onSubmit={this.handleSearchFormSubmit}>
					<FormGroup>
						<Input type="text" name="searchString" value={this.state.searchString} onChange={this.handleInputChange} placeholder="Last Name or Email" />
					</FormGroup>&nbsp;
					<Button>Search</Button>&nbsp;
					<Button onClick={this.handleSearchFormReset}>Reset</Button>
				</Form><br/>
				{contents}
			</div>
		);
	}

	async populateUserData() {
		try {

			const data = await usersService.getUsers(this.state.page, this.state.pageSize, this.state.sortOrder, this.state.searchString);
			this.setState({ totalUsers: data.totalUsers, users: data.users, loading: false });
		}
		catch (error) {
			// Handle error
		}
	}
}
