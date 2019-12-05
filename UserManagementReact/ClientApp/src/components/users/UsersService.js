import authService from '../api-authorization/AuthorizeService';
import { ApplicationPaths, QueryParameterNames } from '../api-authorization/ApiAuthorizationConstants'

export class UsersService {
	async getUsers(page, pageSize, sortOrder, searchString) {
		const token = await authService.getAccessToken();
		const offset = (page - 1) * pageSize;

		// Taken from here: https://fetch.spec.whatwg.org/#fetch-api
		let url = "api/users?" + "offset=" + offset + "&limit=" + pageSize + "&sortOrder=" + sortOrder;
		if (searchString !== "")
			url += "&searchString=" + encodeURIComponent(searchString);

		try {
			const response = await fetch(url, {
				headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
			});

			if (response.ok) {
				const jsonData = await response.json();
				return jsonData;
			}
			else {
				if (response.status == 401) { // Refresh access token if it is expired
					window.location.href =
						`${ApplicationPaths.Login}?${QueryParameterNames.ReturnUrl}=${encodeURI(window.location.href)}`;
				}

				throw new Error("HTTP error! Code: " + response.status);
			}
		}
		catch (error) {
			console.log(error);
			throw error;
		}
	}

	async deleteUser(userId) {
		const token = await authService.getAccessToken();

		try {
			const headers = { 'content-type': 'application/json' };
			if (token) headers['Authorization'] = `Bearer ${token}`;

			const response = await fetch('api/users/' + userId, {
				method: 'DELETE',
				headers: headers,
			});

			if (response.ok) {
				return;
			}
			else {
				throw new Error("HTTP error! Code: " + response.status);
			}
		}
		catch (error) {
			console.log(error);
			throw error;
		}
	}

	async getUser(userId) {
		const token = await authService.getAccessToken();

		try {
			const response = await fetch('api/users/' + userId, {
				headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
			});

			if (response.ok) {
				const jsonData = await response.json();
				return jsonData;
			}
			else {
				throw new Error("HTTP error! Code: " + response.status);
			}
		}
		catch (error) {
			console.log(error);
			throw error;
		}
	}

	async addUser(user) {
		const token = await authService.getAccessToken();

		try {
			const headers = { 'content-type': 'application/json' };
			if (token) headers['Authorization'] = `Bearer ${token}`;

			const response = await fetch('api/users', {
				method: 'POST',
				body: JSON.stringify(user),
				headers: headers,
			});

			if (response.ok) {
				return;
			}
			else {
				throw new Error("HTTP error! Code: " + response.status);
			}
		}
		catch (error) {
			console.log(error);
			throw error;
		}

	}

	async updateUser(userId, user) {
		const token = await authService.getAccessToken();

		try {
			const headers = { 'content-type': 'application/json' };
			if (token) headers['Authorization'] = `Bearer ${token}`;

			const response = await fetch('api/users/' + userId, {
				method: 'PUT',
				body: JSON.stringify(user),
				headers: headers,
			});

			if (response.ok) {
				return;
			}
			else {
				throw new Error("HTTP error! Code: " + response.status);
			}
		}
		catch (error) {
			console.log(error);
			throw error;
		}
	}

	async changeUserPassword(userId, userPasswords) {
		const token = await authService.getAccessToken();

		try {
			const headers = { 'content-type': 'application/json' };
			if (token) headers['Authorization'] = `Bearer ${token}`;

			const response = await fetch('api/users/' + userId + '/change-password', {
				method: 'POST',
				body: JSON.stringify(userPasswords),
				headers: headers,
			});

			if (response.ok) {
				return;
			}
			else {
				throw new Error("HTTP error! Code: " + response.status);
			}
		}
		catch (error) {
			console.log(error);
			throw error;
		}
	}

	static get instance() { return usersService; } // TODO: Check singleton/lifetime
}

const usersService = new UsersService();

export default usersService;