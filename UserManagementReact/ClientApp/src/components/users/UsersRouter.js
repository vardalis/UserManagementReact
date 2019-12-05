import React from 'react';
import { Switch, Route, useRouteMatch } from 'react-router-dom';
import AuthorizeRoute from '../api-authorization/AuthorizeRoute';

import { UsersList } from './UsersList';
import { UsersAdd } from './UsersAdd';
import { UsersDelete } from './UsersDelete';
import { UsersEdit } from './UsersEdit';
import { UsersPasswordChange } from './UsersPasswordChange';

export function UsersRouter() {
	let match = useRouteMatch();

	return (
		<Switch>
			<AuthorizeRoute path={`${match.path}/add`} component={UsersAdd} />
			<Route path={`${match.path}/delete/:userId`} component={UsersDelete} />
			<AuthorizeRoute path={`${match.path}/edit/:userId`} component={UsersEdit} />
			<AuthorizeRoute path={`${match.path}/password-change/:userId`} component={UsersPasswordChange} />
			<AuthorizeRoute path={`${match.path}`} component={UsersList} />
		</Switch>
	);
}