import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { Helmet } from "react-helmet";
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';
import Admin from './components/admin/index';

import './custom.css'
import Housing from "./pages/Housing";
import Items from "./pages/Items";

export default class App extends Component {
  static displayName = App.name;

    render () {
    return (
      <React.Fragment>
        <Helmet>
            <script src="https://kit.fontawesome.com/a076d05399.js" crossOrigin="anonymous"></script>
        </Helmet>
        <Route path='/admin' component={Admin} />
        <Route exact path='/' component={Home} />
        <Route path='/counter' component={Counter} />
        <Route path='/Housing' component={Housing} />
        <Route path={'/Items'} component={Items} />
        <AuthorizeRoute path='/fetch-data' component={FetchData} />
        <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
      </React.Fragment>
    );
  }
}
