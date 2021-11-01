import React, { Component } from 'react';
import { Route } from 'react-router';
import { Login } from './Login'
import { Logout } from './Logout'
import { ApplicationPaths, LoginActions, LogoutActions } from './ApiAuthorizationConstants';
import { Layout } from '../Layout';

type LoginAction = keyof typeof LoginActions;
type LogoutAction = keyof typeof LogoutActions;

export default class ApiAuthorizationRoutes extends Component {

  render () {
    return(
      <Layout>
          <Route path={ApplicationPaths.Login} render={() => loginAction((LoginActions.Login as LoginAction))} />
          <Route path={ApplicationPaths.LoginFailed} render={() => loginAction((LoginActions.LoginFailed as LoginAction))} />
          <Route path={ApplicationPaths.LoginCallback} render={() => loginAction((LoginActions.LoginCallback as LoginAction))} />
          <Route path={ApplicationPaths.Profile} render={() => loginAction((LoginActions.Profile as LoginAction))} />
          <Route path={ApplicationPaths.Register} render={() => loginAction((LoginActions.Register as LoginAction))} />
          <Route path={ApplicationPaths.LogOut} render={() => logoutAction((LogoutActions.Logout as LogoutAction))} />
          <Route path={ApplicationPaths.LogOutCallback} render={() => logoutAction((LogoutActions.LogoutCallback as LogoutAction))} />
          <Route path={ApplicationPaths.LoggedOut} render={() => logoutAction((LogoutActions.LoggedOut as LogoutAction))} />
      </Layout>);
  }
}

function loginAction(name: LoginAction){
    return (<Login action={name}></Login>);
}

function logoutAction(name: LogoutAction) {
    return (<Logout action={name}></Logout>);
}
