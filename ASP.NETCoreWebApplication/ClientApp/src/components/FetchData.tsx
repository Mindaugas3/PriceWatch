import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'
import { Layout } from './Layout';

interface IFetchDataState {
  loading: boolean,
  forecasts: any[]
}

export class FetchData extends Component<{}, IFetchDataState> {
  static displayName = FetchData.name;

  constructor(props: {} | Readonly<{}>) {
    super(props);
    this.state = { forecasts: [], loading: true };
  }

  componentDidMount() {
    this.populateWeatherData();
  }

  static renderForecastsTable(forecasts: any[]) {
    return (
        <table className='table table-striped' aria-labelledby="tabelLabel">
          <thead>
            <tr>
              <th>Date</th>
              <th>Temp. (C)</th>
              <th>Temp. (F)</th>
              <th>Summary</th>
            </tr>
          </thead>
          <tbody>
            {forecasts.map(forecast =>
              <tr key={forecast.date}>
                <td>{forecast.date}</td>
                <td>{forecast.temperatureC}</td>
                <td>{forecast.temperatureF}</td>
                <td>{forecast.summary}</td>
              </tr>
            )}
          </tbody>
        </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : FetchData.renderForecastsTable(this.state.forecasts);

    return (
      <Layout>
        <h1 id="tabelLabel" >Weather forecast</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
      </Layout>
    );
  }

  async populateWeatherData() {
    const token = await authService.getAccessToken();
    const response = await fetch('weatherforecast', {
      headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
    });
    const data = await response.json();
    this.setState({ forecasts: data, loading: false });
  }
}
