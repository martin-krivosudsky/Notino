import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { UploadFile } from './components/UploadFile';
import { AddFileFromUrl } from './components/AddFileFromUrl';
import { SendEmail } from './components/SendEmail';

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route exact path='/UploadFile' component={UploadFile} />
        <Route exact path='/AddFileFromUrl' component={AddFileFromUrl} />
        <Route exact path='/SendEmail' component={SendEmail} />
      </Layout>
    );
  }
}
